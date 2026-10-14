"""Rebuild ToyBox 2.0.16 translations by stable field ID, offline."""
import json
from validate import PROJECT, load


def build_data():
    baseline = load(PROJECT / "reference/upstream-zh-CN-v2.0.16.json")
    revisions = load(PROJECT / "tools/revisions.json")
    assert set(revisions) <= set(baseline), "Unknown revision field IDs"
    return {key: {"Original": entry["Original"],
                  "Translated": revisions.get(key, entry)["Translated"]}
            for key, entry in baseline.items()}


def build():
    data = build_data()
    output = PROJECT / "Localization/zh-CN_lang.json"
    output.write_text(json.dumps(data, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"Built {len(data)} field IDs: {output}")


if __name__ == "__main__":
    build()
