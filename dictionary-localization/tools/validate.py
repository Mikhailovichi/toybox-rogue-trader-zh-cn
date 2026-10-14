"""Check pinned field IDs and reviewed text formatting."""

import json
import re
from collections import Counter
from pathlib import Path

PROJECT = Path(__file__).resolve().parents[1]
TAG = re.compile(r"<(/?)(b|i|color|size)(?:=([^>]+))?>")
FORMAT = re.compile(r"(?<!\{)\{\d+(?:[^{}]*)\}(?!\})")


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


def validate_data(data, original, english, review, rebuilt):
    assert len(data) == 938 and set(data) == set(original) == set(english) == set(review), "Field ID coverage changed"
    assert data == rebuilt, "Output differs from offline rebuild"
    for key, entry in data.items():
        assert set(entry) == {"Original", "Translated"}, f"Interface changed: {key}"
        source, value = entry["Original"], entry["Translated"]
        assert source == original[key]["Original"] == english[key]["Original"], f"Original changed: {key}"
        if not source.strip():
            assert entry == original[key], f"Empty field purpose changed: {key}"
            assert review[key]["Decision"] == "empty-purpose"
            continue
        assert isinstance(value, str) and value.strip(), f"Blank translation: {key}"
        assert re.search(r"[\u3400-\u9fff]", value), f"Untranslated field: {key}"
        assert Counter(FORMAT.findall(source)) == Counter(FORMAT.findall(value)), f"Format mismatch: {key}"
        assert source.count("\n") == value.count("\n") and source.count("\r") == value.count("\r"), f"Newline mismatch: {key}"
        check_markup(value)
        assert [m.group() for m in TAG.finditer(source)] == [m.group() for m in TAG.finditer(value)], f"Rich text mismatch: {key}"
        expected = "retained" if entry == original[key] else "revised"
        assert review[key]["Decision"] == expected and review[key]["Note"], f"Review mismatch: {key}"


def validate():
    from build import build_data
    data = load(PROJECT / "Localization/zh-CN_lang.json")
    original = load(PROJECT / "reference/upstream-zh-CN-v2.0.16.json")
    english = load(PROJECT / "reference/upstream-en-v2.0.16.json")
    review = load(PROJECT / "docs/translation-review.json")
    validate_data(data, original, english, review, build_data())
    filled = sum(bool(v["Original"].strip()) and not (v["Translated"] or "").strip() for v in original.values())
    report = {
        "status": "passed", "target_toybox": "2.0.16", "package_version": "2.0",
        "total_entries": len(data), "reviewed_entries": len(review), "filled_translations": filled,
        "revised_entries": sum(data[k] != v for k, v in original.items()),
        "preserved_empty_originals": sum(not v["Original"].strip() for v in original.values()),
        "checks": ["duplicate keys", "field IDs", "unchanged Original", "nonempty Chinese",
                   "offline rebuild", "format arguments", "CR/LF counts", "preserved and balanced rich text", "review coverage"],
        "game_ui_acceptance": "pending",
    }
    (PROJECT / "docs/validation.json").write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"PASS: {len(data)} field IDs reviewed; {filled} blanks filled; offline rebuild matches")
    return report


if __name__ == "__main__":
    validate()
