# ToyBox Simplified Chinese Localization — Revised

[English](README.md) | [简体中文](README.zh-CN.md)

A revised Simplified Chinese localization for ToyBox in **Warhammer 40,000: Rogue Trader**.

## Compatibility

- Localization release: **1.0**
- Supported ToyBox version: **1.7.34**
- Current status: **in-game UI acceptance pending**
- Type: dictionary-only translation addon; requires the original ToyBox mod.

The dictionary preserves all 680 original keys, revises 345 existing translations, and adds 276 keys for a total of 956. Added keys include UI phrases, finite enum options and exact rich-text variants; they do not represent 276 separate new features.

## Main Improvements

- Standardizes core Chinese UI terms for Archetypes, Etudes, Respec and Search 'n Pick as “职业”, “剧情状态”, “重置加点” and “搜索与添加”.
- Aligns terminology for skills, convictions, Veil Degradation, sanctioned psykers, factions and voidships with the game's official Simplified Chinese localization.
- Corrects references to Rogue Trader mistranslated as Wrath of the Righteous, and the Fellowship stat mistranslated as Charisma.
- Adds translations for psychic phenomena settings, the blueprint patch manager, the character editor, and network and update settings.
- Adds exact rich-text lookup keys while preserving the original keys' capitalization, whitespace, line breaks and format placeholders.

## Installation

1. Install the original [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1), verify version 1.7.34, and close the game.
2. Locate the existing ToyBox mod directory. The default path is:

   ```text
   %USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\0ToyBox0
   ```

3. Copy the existing `Localization\zh-CN.json` from that directory to a backup location of your choice.
4. Copy this project's `Localization\zh-CN.json` into the same location in the mod directory, overwriting the existing file. If using the ZIP package, extract it into the **`0ToyBox0` mod directory**; do not add another nested `Localization` folder.
5. Start the game and select Simplified Chinese (`zh-CN`) in ToyBox settings. If Simplified Chinese is already selected, restarting the game loads the revised dictionary.

This package does not include the ToyBox DLL and cannot be imported as a standalone mod into Unity Mod Manager. Installation only requires replacing one JSON file.

## Restore and Update

To restore the original translation, close the game and copy your backed-up `zh-CN.json` back into place. If you have no backup, extract `Localization\zh-CN.json` from the original ToyBox 1.7.34 package.

ToyBox updates or reinstalls may overwrite this translation. After updating ToyBox, verify compatibility before reinstalling the dictionary. Localization release 1.0 and ToyBox version 1.7.34 are separate version numbers.

## Remaining English

Text that does not use ToyBox's translation interface may still appear in English, including some dialogue and character-page toggles, level-up instructions, and patch editor operation statuses. A dictionary-only addon cannot change this hardcoded text.

Internal blueprint names, field and type names, GUIDs, paths, exception messages and Unity Mod Manager's own UI are outside this addon's scope. Brand names such as ToyBox, Unity and GitHub, key names such as Ctrl, Shift and F11, and coordinate axes X/Y/Z remain unchanged. Game object names and descriptions follow the game's own language setting.

## Credits and License

ToyBox: Narria, ADDB and upstream contributors. The upstream MIT notice also retains credit to Hsinyu.

Original Simplified Chinese contributors: ohZbs, Kirie, lhr7324925. Revised localization: mitcheese.

Distributed under the MIT license. See [LICENSE.txt](LICENSE.txt) and [CREDITS.md](CREDITS.md).
