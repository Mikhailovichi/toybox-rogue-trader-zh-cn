# English Nexus Copy

## Title

ToyBox Simplified Chinese Localization - Revised

## Summary

Revised Simplified Chinese dictionary for ToyBox 2.0.20 prerelease. Fills 86 missing translations and improves blueprint patch, retraining, field-of-view and search instructions. Includes translations for the new feature-rank setting. Requires the original ToyBox mod.

## Description

Localization **2.1 candidate** targets **ToyBox 2.0.20 prerelease**, with terminology checked against game **1.6.1.514**. It retains 216 revisions by field ID, including 86 missing translations, and adopts the upstream translations for the two new fields.

### Main features

- Covers 940 fields for menus, buttons and settings.
- Uses consistent game terminology for archetypes, story states, Profit Factor, Navigator's Insight and psychic mechanics.
- Clarifies blueprint patches, search, saves and retraining, including what each option does and how related settings affect it.
- Distinguishes camera field of view from fog-of-war reveal range and explains dialogue visibility, selection and conviction requirements and changes.

### Installation and restoration

Install the original ToyBox 2.0.20 first, then close the game. Back up `0ToyBox0\Localization\zh-CN_lang.json`. Extract `toybox-zh-cn-v2.1.zip` into `0ToyBox0`, replacing that file. The default mod root on Windows is

```text
%USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\0ToyBox0
```

Select Simplified Chinese (`zh-CN`) in ToyBox. To revert, restore the backup. Localization 2.0 targets ToyBox 2.0.16, and localization 1.0 targets ToyBox 1.7.34.

### Optional display addon

**ToyBox Display Enhancements 0.5.0** adds bilingual dialogue conditions and results, including configured checks, dialogue history, item and currency changes, and colony traits. It installs separately in UMM and can be used independently.

### Scope and status

Only registered ToyBox fields are translated. Internal identifiers and unregistered buttons may remain in English. Game object names follow the game's language setting. The package contains no ToyBox binary or game assets.

Field coverage, source text, placeholders, line breaks, rich text and offline rebuilding passed validation. The new feature-rank label and description were checked in the game’s ToyBox settings and feature search.

### Credits and license

ToyBox: Narria, ADDB and upstream contributors. Original Simplified Chinese: ohZbs, Kirie, lhr7324925, icls1337 and devilmandare. Revision: mitcheese. This revision uses AI-assisted translation and editing. The MIT license and complete credits are included.
