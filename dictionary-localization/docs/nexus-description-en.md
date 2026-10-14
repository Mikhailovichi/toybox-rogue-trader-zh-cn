# English Nexus Copy

## Title

ToyBox Simplified Chinese Localization - Revised

## Summary

Revised Simplified Chinese dictionary for ToyBox 2.0.16 prerelease. Fills 86 missing translations and improves blueprint patch, retraining, field-of-view and search instructions. Requires the original ToyBox mod.

## Description

Localization **2.0 candidate** targets **ToyBox 2.0.16 prerelease**, with terminology checked against game **1.6.1.514**. All 938 field IDs were reviewed; 216 entries were revised, including 86 missing translations. Four empty source fields retain their original purpose.

### Main features

- Covers 938 fields and fills 86 missing translations for menus, buttons and settings.
- Uses consistent game terminology for archetypes, story states, Profit Factor, Navigator's Insight and psychic mechanics.
- Clarifies blueprint patches, search, saves and retraining, including what each option does and how related settings affect it.
- Distinguishes camera field of view from fog-of-war reveal range and explains dialogue visibility, selection and conviction requirements and changes.

### Installation and restoration

Install the original ToyBox 2.0.16 first, then close the game. Back up `0ToyBox0\Localization\zh-CN_lang.json`. Extract `toybox-zh-cn-v2.0.zip` into `0ToyBox0`, replacing that file. The default mod root is:

```text
%USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\0ToyBox0
```

Select Simplified Chinese (`zh-CN`) in ToyBox. Restore the backed-up file to revert. The old 1.0 ZIP is only for ToyBox 1.7.34; it cannot restore the new dictionary format.

### Optional display addon

Chinese Display Enhancements **0.3.0** adds readable dialogue conditions and results, quest names, dialogue quotations and notes for eight internal features. It installs separately in UMM. Either package can be used independently.

### Scope and status

Only registered ToyBox fields are translated. Internal identifiers and unregistered buttons may remain in English. Game object names follow the game's language setting. The package contains no ToyBox binary or game assets.

Field coverage, unchanged source text, placeholders, line breaks, rich text and offline rebuilding have passed validation. Game UI acceptance is pending; old screenshots are not presented as the new interface.

### Credits and license

ToyBox: Narria, ADDB and upstream contributors. Original Simplified Chinese: ohZbs, Kirie, lhr7324925, icls1337 and devilmandare. Revision: mitcheese. This revision uses AI-assisted translation and editing. The MIT license and complete credits are included.
