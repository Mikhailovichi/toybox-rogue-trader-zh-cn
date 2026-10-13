"""Focused regressions for exact localization keys and rich-text syntax."""

import unittest

from extract import PARSER, decode_string, evaluate, walk
from validate import check_markup, unique_object


def expression(code):
    tree = PARSER.parse(("class T { object x = " + code + "; }").encode("utf-8"))
    declarator = next(n for n in walk(tree.root_node) if n.type == "variable_declarator")
    return declarator.named_children[-1]


class ExactKeys(unittest.TestCase):
    def test_csharp_escapes(self):
        self.assertEqual(decode_string(r'"a\r\n\t\u0041\x42\\\""'), 'a\r\n\tAB\\"')
        self.assertEqual(decode_string('@"a""b\\n"'), 'a"b\\n')

    def test_concatenation_and_constant_interpolation(self):
        self.assertEqual(evaluate(expression('"A\\n" + $"B{15}"'), "raw"), "A\nB15")

    def test_dynamic_expression_is_not_a_key(self):
        with self.assertRaises(ValueError):
            evaluate(expression('$"Level: {character.Level}"'), "raw")

    def test_richtext_and_case(self):
        self.assertEqual(evaluate(expression('"elements".Orange().Bold()'), "srgb"), "<b><color=#F2A92DFF>elements</color></b>")
        self.assertNotEqual(evaluate(expression('"elements"'), "raw"), evaluate(expression('"Elements"'), "raw"))
        check_markup("<b><color=#00FF00FF>text</color></b>")
        for value in ("<b>text</color>", "<color=##00FF00FF>text</color>", "<size=no>text</size>"):
            with self.assertRaises(ValueError):
                check_markup(value)

    def test_duplicate_json_key(self):
        with self.assertRaises(ValueError):
            unique_object([("a", 1), ("a", 2)])


if __name__ == "__main__":
    unittest.main()
