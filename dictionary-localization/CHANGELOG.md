# Changelog

## 2.0

Compatibility target: ToyBox **2.0.16 prerelease**, game **1.6.1.514**. Candidate; game UI acceptance pending.

- Reviewed all 938 field IDs; revised 216 entries, including all 86 missing nonempty-source translations.
- Preserved four empty-source fields, every Original value and the new `{ID: {Original, Translated}}` interface.
- Pinned official English and Chinese baselines; offline rebuilding now uses field-ID revisions and review records.
- Retained accurate new translations, including 百宝袋, 便利功能 and 搜索与选取.
- Clarified blueprint patches, search, saves, field of view and retraining; aligned conviction and check terminology with the local game.
- Package installs `Localization/zh-CN_lang.json`; retired old source-key extraction, finite enum expansion and old installation JSON.
- Kept the 1.0 ZIP for ToyBox 1.7.34. Credits and compatibility are recorded outside the dictionary.

## 1.0

Compatibility target: ToyBox **1.7.34**. Candidate package; game UI acceptance pending.

- Reviewed all 680 original translations and revised 345.
- Added 276 exact dictionary keys, bringing the total to 956.
- Standardized Archetypes as 职业, Etudes as 剧情状态, Respec as 重置加点 and Search 'n Pick as 搜索与添加.
- Aligned skill, conviction, faction and voidship terms with the local official Simplified Chinese localization.
- Corrected Rogue Trader references previously translated as Wrath of the Righteous, and corrected RT Specific and Fellowship.
- Added psychic phenomena/perils settings, blueprint patch tool labels, character editor text and update settings.
- Added rich-text color variants and finite display enum labels.
- Preserved every original key and all original contributors; appended mitcheese.
- Changed dictionary metadata compatibility version from 1.5.13 to 1.7.34.

No ToyBox binaries or personal settings are included.
