"""Extract exact localization keys from the pinned ToyBox C# source."""

import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent / "vendor"))
from tree_sitter import Language, Parser
import tree_sitter_c_sharp

PROJECT = Path(__file__).resolve().parents[1]
SOURCE = PROJECT / "reference/source/ToyBox"
PARSER = Parser(Language(tree_sitter_c_sharp.language()))
COLORS = dict(
    White="#ededed", Grey="#A0A0A0FF", DarkGrey="#505050FF",
    Red="#C04040E0", Green="#00ff00ff", Blue="#0a1ce9",
    Cyan="#6aebee", Magenta="#dd3eea", Yellow="#efee3e", Orange="#f2a92d",
)
RGBA = {}


def require_source():
    if not (SOURCE / "Classes/ModKit/UI/RichText.cs").is_file():
        raise FileNotFoundError(
            "Optional ToyBox source is missing from reference/source. "
            "See docs/maintenance.md; ordinary build and validation use the tracked key files."
        )


def load_rgba():
    if not RGBA:
        require_source()
        source = (SOURCE / "Classes/ModKit/UI/RichText.cs").read_text(encoding="utf-8-sig")
        RGBA.update({
            name: int(value, 16)
            for name, value in re.findall(r"^\s*(\w+)\s*=\s*0x([0-9a-fA-F]+),", source, re.M)
        })
        RGBA["none"] = RGBA["silver"]
    return RGBA


def text(node):
    return node.text.decode("utf-8")


def walk(node):
    yield node
    for child in node.named_children:
        yield from walk(child)


def decode_string(value):
    if value.startswith('@"'):
        return value[2:-1].replace('""', '"')
    escapes = {
        "0": "\0", "a": "\a", "b": "\b", "f": "\f", "n": "\n",
        "r": "\r", "t": "\t", "v": "\v", "'": "'", '"': '"', "\\": "\\",
    }
    return re.sub(
        r"\\(u[0-9a-fA-F]{4}|U[0-9a-fA-F]{8}|x[0-9a-fA-F]{1,4}|.)",
        lambda m: chr(int(m[1][1:], 16)) if m[1][0] in "uUx" else escapes[m[1]],
        value[1:-1],
    )


def html_color(value, mode, enum=False):
    if enum:
        channels = [(value >> shift & 255) / 256 for shift in (24, 16, 8, 0)]
    else:
        if mode == "raw":
            return value
        digits = value.lstrip("#")
        if len(digits) == 6:
            digits += "ff"
        channels = [int(digits[i:i + 2], 16) / 255 for i in (0, 2, 4, 6)]
    if mode == "linear":
        channels[:3] = [
            12.92 * c if c <= 0.0031308 else 1.055 * c ** (1 / 2.4) - 0.055
            for c in channels[:3]
        ]
    # Unity's ToHtmlStringRGBA rounds to bytes and uses uppercase hex.
    return "#" + "".join(f"{max(0, min(255, round(c * 255))):02X}" for c in channels)


def evaluate(node, mode):
    if node.type in ("string_literal", "verbatim_string_literal"):
        return decode_string(text(node))
    if node.type == "integer_literal":
        return int(text(node))
    if node.type == "parenthesized_expression":
        return evaluate(node.named_children[0], mode)
    if node.type == "interpolated_string_expression":
        parts = []
        for child in node.named_children:
            if child.type in ("string_content", "escape_sequence"):
                parts.append(decode_string('"' + text(child) + '"'))
            elif child.type == "interpolation":
                expression = next(n for n in child.named_children if n.type != "interpolation_brace")
                parts.append(str(evaluate(expression, mode)))
        return "".join(parts)
    if node.type == "binary_expression":
        left = evaluate(node.child_by_field_name("left"), mode)
        right = evaluate(node.child_by_field_name("right"), mode)
        if isinstance(left, str) and isinstance(right, str):
            return left + right
    if node.type == "invocation_expression":
        function = node.child_by_field_name("function")
        if function.type != "member_access_expression":
            raise ValueError(text(node))
        name = text(function.child_by_field_name("name"))
        receiver = function.child_by_field_name("expression")
        arguments = node.child_by_field_name("arguments").named_children
        if text(receiver) == "RichText":
            value = evaluate(arguments[0].named_children[-1], mode)
            extra = arguments[1:]
        else:
            value = evaluate(receiver, mode)
            extra = arguments
        if name == "Bold":
            return f"<b>{value}</b>"
        if name in ("Size", "SizePercent"):
            size = evaluate(extra[0].named_children[-1], mode)
            return f"<size={size if name == 'Size' else str(100 + size) + '%'}>{value}</size>"
        if name in COLORS:
            return f"<color={html_color(COLORS[name], mode)}>{value}</color>"
        if name == "Color":
            color_node = extra[0].named_children[-1]
            color_text = text(color_node)
            if color_text.startswith("RGBA."):
                # ToyBox 1.7.34 prepends a second '#' in its Color overload.
                return f"<color=#{html_color(load_rgba()[color_text[5:]], mode, True)}>{value}</color>"
            return f"<color={html_color(evaluate(color_node, mode), mode)}>{value}</color>"
    raise ValueError(text(node))


def extract():
    require_source()
    keys = {}
    dynamic = []
    hardcoded = []
    names = set()
    for path in sorted((SOURCE / "Classes").rglob("*.cs")):
        relative = path.relative_to(SOURCE).as_posix()
        tree = PARSER.parse(path.read_bytes())
        for node in walk(tree.root_node):
            if node.type == "object_creation_expression":
                type_node = node.child_by_field_name("type")
                if type_node and text(type_node).startswith("NamedTypeFilter"):
                    arguments = node.child_by_field_name("arguments")
                    if arguments and arguments.named_children:
                        try:
                            names.add(evaluate(arguments.named_children[0].named_children[-1], "raw"))
                        except ValueError:
                            pass
            if node.type != "invocation_expression":
                continue
            function = node.child_by_field_name("function")
            name_node = function.child_by_field_name("name")
            name = text(name_node) if name_node else text(function)
            location = {"file": relative, "line": node.start_point.row + 1}
            if name == "localize":
                receiver = function.child_by_field_name("expression")
                try:
                    for mode in ("raw", "srgb", "linear"):
                        key = evaluate(receiver, mode)
                        if key:
                            keys.setdefault(key, []).append({**location, "mode": mode})
                except (ValueError, KeyError):
                    dynamic.append({**location, "expression": text(receiver)})
            elif name in ("Label", "HelpLabel", "ActionButton", "Toggle", "TitleLabel"):
                arguments = node.child_by_field_name("arguments")
                if not arguments or not arguments.named_children:
                    continue
                argument = arguments.named_children[0]
                if ".localize(" in text(argument):
                    continue
                literals = [n for n in walk(argument) if n.type in (
                    "string_literal", "verbatim_string_literal", "interpolated_string_expression",
                )]
                if literals and re.search(r"[A-Za-z]{3}", text(argument)):
                    hardcoded.append({**location, "expression": text(argument)})
    result = {
        "source_tag": "v1.7.34",
        "source_commit": "f044d0ccc90c4800aef0142fdd5147abaf7f5a4e",
        "keys": dict(sorted(keys.items())),
        "named_filters": sorted(names),
        "dynamic_calls": dynamic,
        "hardcoded_candidates": hardcoded,
    }
    target = PROJECT / "reference/extracted-keys.json"
    target.write_text(json.dumps(result, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    baseline = json.loads(
        (PROJECT / "reference/baseline-zh-CN.json").read_text(encoding="utf-8-sig")
    )["Strings"]
    missing = [key for key in keys if key not in baseline]
    print(json.dumps({
        "exact_static_keys": len(keys),
        "missing_from_original": len(missing),
        "dynamic_calls_to_review": len(dynamic),
        "named_filters": len(names),
    }, indent=2))


if __name__ == "__main__":
    extract()
