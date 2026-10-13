"""Complete finite display keys after the AST and installed enum extraction."""

import json
import re

from extract import PROJECT, SOURCE, PARSER, load_rgba, require_source, html_color, evaluate, text, walk


def finite():
    require_source()
    path = PROJECT / "reference/finite-keys.json"
    extracted = json.loads((PROJECT / "reference/extracted-keys.json").read_text(encoding="utf-8"))
    keys = set(extracted["named_filters"])
    enums = json.loads((PROJECT / "reference/game-enums.json").read_text(encoding="utf-8"))
    for data in enums.values():
        keys.update(data["names"])
    tree = PARSER.parse((SOURCE / "Classes/MainUI/Main.cs").read_bytes())
    for node in walk(tree.root_node):
        if node.type == "object_creation_expression" and text(node.child_by_field_name("type")) == "NamedAction":
            arguments = node.child_by_field_name("arguments")
            keys.add(evaluate(arguments.named_children[0].named_children[-1], "raw"))
    keys.difference_update(("Playground", "Enchantment"))  # Disabled main tabs.
    skeleton = (SOURCE / "Classes/MainUI/SkeletonReplacer.cs").read_text(encoding="utf-8-sig")
    keys.update(re.findall(r'\{\s*"((?:OF|SC|SZ|IO|IS)_\w+)"\s*,', skeleton))
    for relative, enum in (
        ("Classes/Infrastructure/BaseUnitDataUtils.cs", "UnitSelectType"),
        ("Classes/ModKit/ModKit/ModKit.cs", "LogLevel"),
    ):
        tree = PARSER.parse((SOURCE / relative).read_bytes())
        for node in walk(tree.root_node):
            if node.type == "enum_declaration" and text(node.child_by_field_name("name")) == enum:
                for member in node.child_by_field_name("body").named_children:
                    if member.type == "enum_member_declaration":
                        keys.add(text(member.child_by_field_name("name")))
    # KeyBindButton localizes the title represented by these source constants.
    for relative in (
        "Classes/MainUI/BagOfTricks.cs",
        "Classes/MainUI/EnhancedUI/PhatLoot.cs",
        "Classes/MainUI/EnhancedUI/EnhancedCamera.cs",
    ):
        tree = PARSER.parse((SOURCE / relative).read_bytes())
        for node in walk(tree.root_node):
            if node.type == "field_declaration" and re.search(r"\bconst\s+string", text(node)):
                for child in walk(node):
                    if child.type == "string_literal":
                        keys.add(evaluate(child, "raw"))
    rarity = (SOURCE / "Classes/MainUI/EnhancedUI/ItemRarity.cs").read_text(encoding="utf-8-sig")
    names = re.findall(r"^\s*(\w+),", rarity.split("public enum RarityType {", 1)[1].split("}", 1)[0], re.M)
    colors = re.findall(r"RGBA\.(\w+)", rarity.split("DarkModeRarityColors = {", 1)[1].split("};", 1)[0])
    rgba = load_rgba()
    for name, color in zip(names, colors, strict=True):
        keys.add(name)
        for mode in ("srgb", "linear"):
            keys.add(f"<color=#{html_color(rgba[color], mode, True)}>{name}</color>")
    keys.update(("Containers", "Units", "Contentment", "Efficiency", "Security"))
    path.write_text(json.dumps(sorted(keys), ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Finite display keys: {len(keys)}")


if __name__ == "__main__":
    finite()
