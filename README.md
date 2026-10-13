# ToyBox Simplified Chinese Localization — Revised

[English](README.md) | [简体中文](README.zh-CN.md)

A revised Simplified Chinese localization for ToyBox in **Warhammer 40,000: Rogue Trader**.

Built on the original Simplified Chinese dictionary, this revision corrects inaccurate translations, standardizes game terminology, and adds Chinese labels and descriptions across multiple feature pages.

## Version and Requirements

- Localization release: **1.0**
- Supported ToyBox version: **1.7.34**
- Requires the original ToyBox mod. This addon only replaces the Chinese dictionary; it does not include the original mod.

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

## Main Improvements

### Interface Navigation and Common Actions

- Standardizes page names such as Etudes (“剧情状态”) and Search 'n Pick (“搜索与添加”) for consistent navigation.
- Corrects references to Rogue Trader previously mistranslated as Wrath of the Righteous.
- Adds or revises translations for settings, network and update messages, and common action buttons.

### Characters, Archetypes and Respec

- Uses “职业” for Archetypes and corrects attribute and skill names, including Fellowship (“社交”), Ballistic Skill (“弹道技能”) and Willpower (“意志”).
- Clarifies descriptions for respec from levels 0, 15 and 35, along with archetype selection warnings.
- Improves labels for portraits, appearance and character editing options.

### Psychic and Combat Mechanics

- Standardizes terms such as Momentum (“士气”), Psy Rating (“灵能等级”) and Veil Degradation (“障壁恶化”).
- Adds translations for Psychic Phenomena, minor and major Perils of the Warp, and related controls, including setting or fixing the Veil Degradation value.

### Story States and Convictions

- Replaces the misleading translation of Etudes as “研究” with “剧情状态” and clarifies descriptions of their hierarchy, conditions and status.
- Aligns conviction names with the game: Dogmatic (“教条主义”), Iconoclast (“自由主义”) and Heretical (“异端主义”).

### Blueprint Search and Patch Management

- Improves translations for search categories and display options.
- Adds labels and descriptions for the patch manager, field types and editing actions.
- Clarifies warnings about high-risk changes, keeping backups and restarting the game after creating a patch.

### Warhammer Terminology and Display Options

- Standardizes faction, voidship and equipment names, including Warp Drives (“亚空间引擎”), Geller Field Device (“盖勒力场装置”) and Servoskull (“伺服头骨”).
- Distinguishes size categories more clearly and improves Chinese names for rarity and color-coded options.

## Restore and Update

To restore the original translation, close the game and copy your backed-up `zh-CN.json` back into place. If you have no backup, extract `Localization\zh-CN.json` from the original ToyBox 1.7.34 package.

ToyBox updates or reinstalls may overwrite this translation. After updating ToyBox, verify compatibility before reinstalling the dictionary. Localization release 1.0 and ToyBox version 1.7.34 are separate version numbers.

## Translation Coverage and Limitations

Some dialogue and character-page toggles, level-up instructions, and patch editor operation statuses may still appear in English. These are outside the text this dictionary addon can replace.

Blueprint IDs, internal names, field and type names, GUIDs, paths and exception messages remain unchanged so that they can be identified and used in editing tools. Brand names such as ToyBox, Unity and GitHub, key names such as Ctrl, Shift and F11, and coordinate axes X/Y/Z also remain unchanged.

Unity Mod Manager's own UI is not translated by this addon. Game object names and descriptions follow the game's own language setting; select Simplified Chinese in the game to view that content in Chinese.

## Credits and License

ToyBox: Narria, ADDB and upstream contributors. The upstream MIT notice also retains credit to Hsinyu.

Original Simplified Chinese contributors: ohZbs, Kirie, lhr7324925. Revised localization: mitcheese.

Distributed under the MIT license. See [LICENSE.txt](LICENSE.txt) and [CREDITS.md](CREDITS.md).
