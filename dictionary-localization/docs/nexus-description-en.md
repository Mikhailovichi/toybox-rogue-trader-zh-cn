# English Nexus Copy

## Title

ToyBox Simplified Chinese Localization — Revised

## Summary

Revised Simplified Chinese dictionary for ToyBox 1.7.34. Corrects existing translations, standardizes game terms and adds psychic settings, blueprint patch tool and character editor text. Requires the original ToyBox mod.

## Description

This is a Simplified Chinese translation addon for **ToyBox 1.7.34** in Warhammer 40,000: Rogue Trader. The localization release version is **1.0**.

It preserves all 680 original dictionary keys, revises 345 existing translations and adds 276 keys, bringing the total to 956. Added keys include new UI phrases, finite display enum names and exact rich-text variants.

**Changes**

- Standardizes Archetypes as 职业, Etudes as 剧情状态, Respec as 重置加点 and Search 'n Pick as 搜索与添加.
- Corrects old mistranslations, including references that named Wrath of the Righteous instead of Rogue Trader.
- Aligns skills, convictions, Veil Degradation, factions and voidship terminology with the local official Chinese localization.
- Adds translations for psychic phenomena/perils settings, the blueprint patch manager, character editor and update options.
- Covers rich-text lookup variants produced by ToyBox 1.7.34.

**Requirements and installation**

Install the original [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1) first and verify version 1.7.34. Close the game before replacing the dictionary.

Back up `Localization\zh-CN.json` from the installed `0ToyBox0` directory. Extract this archive into that mod directory and overwrite the same file.

Default location:

```text
%USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\0ToyBox0
```

Start the game and select Simplified Chinese (`zh-CN`) in ToyBox settings. This addon does not contain ToyBox binaries and is intended for manual installation into the existing mod.

To restore the original translation, close the game and copy the backed-up JSON back into place. ToyBox updates may overwrite the localization; check compatibility before reinstalling this addon.

**Known English text**

Only text connected to ToyBox's localization interface can be translated by a dictionary. Some hardcoded switches, level-up explanations and patch operation states still appear in English. Internal blueprint names, field/type names, GUIDs, paths and Unity Mod Manager's own interface are outside this addon's scope. Brand names, keyboard names and coordinate axis labels are retained.

This addon changes only `zh-CN.json`. In-game object names and descriptions continue to follow the game's language settings.

**Testing**

Static dictionary validation has passed. This 1.0 candidate package is awaiting in-game UI acceptance; it has not completed a full interface playtest.

**Credits and license**

ToyBox: Narria, ADDB and upstream contributors. Original Simplified Chinese: ohZbs, Kirie, lhr7324925. Translation revision: mitcheese.

This revision uses AI-assisted translation and editing, checked against the pinned source and local official Chinese terminology. The upstream MIT license and copyright notice are included.
