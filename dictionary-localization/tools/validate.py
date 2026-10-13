"""Check the dictionary contract and report exact pinned-source coverage."""

import json
import re
from collections import Counter
from pathlib import Path

PROJECT = Path(__file__).resolve().parents[1]
TAG = re.compile(r"<(/?)(b|i|color|size)(?:=([^>]+))?>")
FORMAT = re.compile(r"(?<!\{)\{\d+(?:[^{}]*)\}(?!\})")
ALLOWED_WORDS = {
    "ToyBox", "Unity", "Mod", "Manager", "Sh0dan", "GitHub", "Owlcat",
    "Discord", "Ctrl", "Shift", "ID", "GUID", "Weapon", "json",
    "Mono", "PNG", "PuzzlePiece", "Green3x1",
    "F", "F11", "X", "Y", "Z", "x1", "x2", "x4", "x6",
}


def unique_object(pairs):
    result = {}
    for key, value in pairs:
        if key in result:
            raise ValueError(f"Duplicate JSON key: {key!r}")
        result[key] = value
    return result


def load(path):
    return json.loads(path.read_text(encoding="utf-8-sig"), object_pairs_hook=unique_object)


def check_markup(value):
    stack = []
    for match in TAG.finditer(value):
        closing, name, argument = match.groups()
        if closing:
            if not stack or stack.pop() != name:
                raise ValueError(f"Mismatched closing tag in {value!r}")
        else:
            if name == "color" and not re.fullmatch(r"#[\da-fA-F]{6}(?:[\da-fA-F]{2})?|[A-Za-z]+", argument or ""):
                raise ValueError(f"Invalid color in {value!r}")
            if name == "size" and not re.fullmatch(r"\d+%?", argument or ""):
                raise ValueError(f"Invalid size in {value!r}")
            stack.append(name)
    if stack:
        raise ValueError(f"Unclosed tags in {value!r}")
    if re.search(r"</?(?:b|i|color|size)\b", TAG.sub("", value)):
        raise ValueError(f"Malformed tag in {value!r}")


def validate():
    data = load(PROJECT / "Localization/zh-CN.json")
    original = load(PROJECT / "reference/baseline-zh-CN.json")
    extracted = load(PROJECT / "reference/extracted-keys.json")
    finite = set(load(PROJECT / "reference/finite-keys.json"))
    additions = load(PROJECT / "tools/additions.json")
    strings = data["Strings"]
    assert set(data) == set(original), "Top-level JSON interface changed"
    assert data["LanguageCode"] == "zh-CN" and data["Version"] == "1.7.34"
    assert data["Contributors"] == original["Contributors"] + ", mitcheese"
    assert set(original["Strings"]) <= set(strings), "Original keys were lost"
    expected = set(extracted["keys"]) | set(extracted["named_filters"]) | finite
    missing = sorted(expected - set(strings))
    assert not missing, f"Missing exact keys: {missing!r}"
    residual = []
    for key, value in strings.items():
        assert isinstance(value, str) and value.strip(), f"Blank translation: {key!r}"
        assert re.search(r"[\u3400-\u9fff]", value) or value in ("<", ">", "GUID"), f"No Chinese text: {key!r}"
        assert Counter(FORMAT.findall(key)) == Counter(FORMAT.findall(value)), f"Format mismatch: {key!r}"
        assert key.count("\n") == value.count("\n"), f"Newline mismatch: {key!r}"
        assert key.count("\r") == value.count("\r"), f"CR mismatch: {key!r}"
        check_markup(value)
        words = set(re.findall(r"[A-Za-z][A-Za-z0-9]*", TAG.sub("", value)))
        words.discard("K")  # 4K resolution.
        unknown = words - ALLOWED_WORDS
        assert not unknown, f"Unreviewed English: {key!r}: {sorted(unknown)}"
        if words:
            residual.append({"key": key, "translation": value, "retained": sorted(words)})
    assert strings["Archetypes"] == "职业"
    assert strings["Etudes"] == "剧情状态"
    assert strings["Respec"] == "重置加点"
    assert strings["Search 'n Pick"] == "搜索与添加"
    assert strings["WarhammerFellowship"] == "社交"
    assert all("正义之怒" not in value for value in strings.values())
    assert "elements" in strings and "Elements" in strings, "Case-distinct keys lost"
    report = {
        "status": "passed",
        "target_toybox": "1.7.34",
        "source_commit": extracted["source_commit"],
        "baseline_entries": len(original["Strings"]),
        "changed_original_entries": sum(strings[k] != v for k, v in original["Strings"].items()),
        "added_entries": len(strings) - len(original["Strings"]),
        "total_entries": len(strings),
        "static_keys": len(extracted["keys"]),
        "named_filter_keys": len(extracted["named_filters"]),
        "finite_display_keys": len(finite),
        "combined_reference_keys": len(expected),
        "missing_reference_keys": missing,
        "new_plain_phrases": len(set(additions) - {TAG.sub("", k) for k in original["Strings"]}),
        "retained_english_entries": residual,
        "checks": ["duplicate keys", "metadata", "original key retention", "nonempty Chinese",
                   "exact source coverage", "format arguments", "CR/LF counts",
                   "balanced rich text", "color syntax", "reviewed English", "core terms"],
        "game_ui_acceptance": "pending",
    }
    (PROJECT / "docs/validation.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"PASS: {len(strings)} entries; {len(expected)} reference keys covered; game UI acceptance pending")
    return report


if __name__ == "__main__":
    validate()
