"""Regression checks for field-ID rebuilding and rejected translation damage."""

import copy
import json
import unittest
from unittest.mock import patch

import build
from validate import PROJECT, check_markup, load, unique_object, validate_data


class DictionaryTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.original = load(PROJECT / "reference/upstream-zh-CN-v2.0.16.json")
        cls.english = load(PROJECT / "reference/upstream-en-v2.0.16.json")
        cls.review = load(PROJECT / "docs/translation-review.json")
        cls.data = build.build_data()

    def validate_changed(self, data):
        validate_data(data, self.original, self.english, self.review, data)

    def test_current_dictionary(self):
        self.validate_changed(self.data)

    def test_same_original_independent_ids(self):
        baseline = {
            "first-id": {"Original": "Enabled", "Translated": "original first"},
            "second-id": {"Original": "Enabled", "Translated": "original second"},
        }
        revisions = {"second-id": {"Translated": "revised second", "Reason": "context"}}
        with patch.object(build, "load", side_effect=[baseline, revisions]):
            result = build.build_data()
        self.assertEqual(result["first-id"], baseline["first-id"])
        self.assertEqual(result["second-id"]["Original"], "Enabled")
        self.assertEqual(result["second-id"]["Translated"], "revised second")

    def test_unknown_revision_id(self):
        with patch.object(build, "load", side_effect=[self.original, {"unknown": {}}]):
            with self.assertRaisesRegex(AssertionError, "Unknown revision"):
                build.build_data()

    def test_duplicate_keys_at_both_levels(self):
        for text in ('{"id": {}, "id": {}}', '{"id": {"Original": "A", "Original": "B"}}'):
            with self.subTest(text=text), self.assertRaisesRegex(ValueError, "Duplicate JSON key"):
                json.loads(text, object_pairs_hook=unique_object)

    def test_missing_id(self):
        data = dict(self.data)
        data.pop(next(iter(data)))
        with self.assertRaisesRegex(AssertionError, "Field ID coverage"):
            self.validate_changed(data)

    def test_changed_original(self):
        data = copy.deepcopy(self.data)
        key = next(k for k, v in data.items() if v["Original"].strip())
        data[key]["Original"] += " changed"
        with self.assertRaisesRegex(AssertionError, "Original changed"):
            self.validate_changed(data)

    def test_empty_purposes_preserved(self):
        keys = [k for k, v in self.data.items() if not v["Original"].strip()]
        self.assertEqual(len(keys), 4)
        for key in keys:
            data = copy.deepcopy(self.data)
            data[key]["Translated"] = "changed"
            with self.subTest(key=key), self.assertRaisesRegex(AssertionError, "Empty field purpose"):
                self.validate_changed(data)

    def test_blank_translation(self):
        data = copy.deepcopy(self.data)
        key = next(k for k, v in data.items() if v["Original"].strip())
        data[key]["Translated"] = "  "
        with self.assertRaisesRegex(AssertionError, "Blank translation"):
            self.validate_changed(data)

    def test_placeholder_and_newline_damage(self):
        for token, message in (("{0}", "Format mismatch"), ("\n", "Newline mismatch")):
            data = copy.deepcopy(self.data)
            key = next(k for k, v in data.items() if (token in v["Original"] if token == "{0}" else v["Original"].strip()))
            if token == "{0}":
                data[key]["Translated"] = data[key]["Translated"].replace(token, "", 1)
            else:
                data[key]["Translated"] += token
            with self.subTest(token=token), self.assertRaisesRegex(AssertionError, message):
                self.validate_changed(data)

    def test_rich_text_damage(self):
        for text in ("<b>text</i>", "<color=#nothex>text</color>", "<size=big>text</size>", "<b>text"):
            with self.subTest(text=text), self.assertRaises(ValueError):
                check_markup(text)
        check_markup("<size=75%><color=#AABBCC><b>text</b></color></size>")

    def test_changed_rich_text(self):
        data = copy.deepcopy(self.data)
        key = next(k for k, v in data.items() if v["Original"].strip())
        data[key]["Translated"] = "<b>" + data[key]["Translated"] + "</b>"
        with self.assertRaisesRegex(AssertionError, "Rich text mismatch"):
            self.validate_changed(data)

    def test_stale_review(self):
        review = copy.deepcopy(self.review)
        key = next(k for k, v in review.items() if v["Decision"] == "revised")
        review[key]["Decision"] = "retained"
        with self.assertRaisesRegex(AssertionError, "Review mismatch"):
            validate_data(self.data, self.original, self.english, review, self.data)

    def test_rebuild_mismatch(self):
        with self.assertRaisesRegex(AssertionError, "Output differs"):
            validate_data(self.data, self.original, self.english, self.review, {})


if __name__ == "__main__":
    unittest.main()
