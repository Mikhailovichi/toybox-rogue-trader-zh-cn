# ToyBox Display Enhancements: English Nexus Copy

## Title

ToyBox Display Enhancements

## Summary

English and Chinese dialogue previews for ToyBox 2.0.16: quest names, dialogue quotations, grouped conditions and recruitment requirements, plus notes for eight internal features. Standalone UMM addon. Addon text follows ToyBox's language; official text follows the game's language.

## Description

### About this addon

ToyBox Display Enhancements **0.4.0 candidate** targets **ToyBox 2.0.16 prerelease**, checked against game **1.6.1.514**.

It makes common dialogue conditions and results readable in English and Simplified Chinese, including vendor selection, custom companion recruitment, quest objectives and story states. Eight internal feature entries also receive aliases or explanatory notes.

**Supported ToyBox UI languages are English (`en`, including regional codes) and Simplified Chinese (`zh-CN`).** Labels, aliases, notes and technical details follow ToyBox. Official quest names, dialogue, conviction names and check names follow the game. Set both to English for a fully English display, or both to Simplified Chinese for a fully Chinese display. Mixed settings keep each source's language, with no separate addon language setting.

Other ToyBox languages retain the original displays. Other game languages retain valid official text. Missing names without a reviewed fallback retain their internal identity or original caption.

### Installation

Install [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1) **2.0.16**, then close the game. Rogue Trader includes Unity Mod Manager (UMM); no separate UMM installation is needed. If the old `ToyBoxChineseDisplayEnhancements` addon is installed, follow Updating and uninstalling below before starting the game.

**Using Modfinder**

Download and open [Modfinder](https://www.nexusmods.com/warhammer40kroguetrader/mods/146). Drag `toybox-display-enhancements-v0.4.zip` into **"Drag zips here to install"** and wait for installation to finish. Use the downloaded ZIP for this candidate; catalogue installation is not required.

**Manual installation**

Extract the ZIP into:

```text
%USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager
```

With either method, `ToyBoxDisplayEnhancements` must be alongside `0ToyBox0`. The addon DLL and `Info.json` belong directly inside `ToyBoxDisplayEnhancements`.

Start the game, press `Ctrl + F10` to open UMM, and enable **ToyBox Display Enhancements**. Set ToyBox to English (`en`) or Simplified Chinese (`zh-CN`), then enable **Dialog Results** and **Dialog Conditions** in the Preview section of Bag of Tricks. Set the game to the same language if you want matching official text.

The UMM ID and installation directory are `ToyBoxDisplayEnhancements`; the DLL is `ToyBoxDisplayEnhancements.dll`. Upgrading from the old `ToyBoxChineseDisplayEnhancements` ID requires transferring settings and removing the old directory, as described below.

### Main features

- **Results:** vendor selection and custom companion creation are described directly. Quest objectives include the parent quest name. Current results use **Result preview**; later possible results use **Possible later results**.
- **Conditions:** **Availability conditions** explains when an option appears; **Selection requirements** explains when it can be selected. AND/OR groups, nesting and individual or whole-group negation are preserved.
- **Dialogue history:** quotes the related line, distinguishes this conversation from previous records, and describes whether the line was shown rather than selected. Excerpts are limited to 120 characters for English ToyBox text, preserving whole words where possible, or 40 characters for Chinese.
- **Answer history:** `Answer Selected` quotes the official answer and states whether it was selected, in this conversation or previously. `Answer List Shown` describes whether an option group was shown and identifies it with a representative answer, without implying that answer was selected. Negation and technical identifiers are preserved.
- **Dialogue blueprint browser:** answer and answer-list titles quote the official answer text, with the full representative answer in the description. Search supports excerpts, internal names and GUIDs; description search also covers the full text. The internal-name setting adds the original identifier without changing the plain-name sort order. Official descriptions are retained, and missing text keeps the original title.
- **Dialogue lines and containers:** dialogue lines, dialogs, book pages, sequences and exits quote their first readable official line or answer, with its full text in the description. They share answer search and sorting rules. Check blueprints show the skill, base DC or preset difficulty, hidden status and conditional modifiers without calculating the effective DC.
- **Check and character conditions:** check conditions distinguish success and failure records in this conversation. Companion, item, ability and effect conditions retain state filters, quantity thresholds and target identities.
- **Answer records and story flags:** actions describe marking or clearing answer records and starting dialogs. Flag conditions retain unlock prerequisites and value requirements; actions explain unlocking, locking and value adjustments. Dynamic targets and increments retain their expressions, while unnamed flags keep their internal identifiers.
- **Story states, convictions and checks:** the registered condition reads `Story state "Heresy" is not running`, without inferring that it has never occurred. Conviction names, signed changes and check names use game localization where available, including Dogmatic, Iconoclast, Heretical, Persuasion and Awareness in English. Point requirements state a minimum; rank requirements retain the original summary.
- **Features and search:** eight internal entries have aliases or notes marked **Addon note** in English or **汉化说明** in Chinese. Valid official text takes priority. Search supports both English and Chinese aliases, internal names and GUIDs; addon-note search follows the description-search setting and uses the current addon language.

English examples:

> Result preview: Open vendor selection
>
> Selection requirements: Profit Factor meets the requirement for recruiting a custom companion
>
> Possible later results: Begin custom companion creation

The feature notes cover three internal conviction records, base character features, reload after combat and a custom character-creation path. `IsPlayer_Feature` and `FeatProgressionTest` retain their internal names because their full roles are not confirmed. Notes describe reviewed structure rather than recommending changes to the entries.

The addon retains ToyBox's branch collection and condition-evaluation behavior. It does not execute actions or calculate recruitment costs. Later preview results depend on dialogue branches and checks. Unsupported types retain an identifiable original summary.

For original captions, internal identifiers and source depth, enable **Show technical details** in this addon's UMM panel. The setting is saved automatically; follow the steps below to retain it when upgrading from the old mod ID.

### Optional localization package

**ToyBox Simplified Chinese Localization 2.0** provides Chinese menu and setting text. It replaces `0ToyBox0\Localization\zh-CN_lang.json`; this display addon installs in its own UMM directory. Either package can be used independently.

### Updating and uninstalling

The mod ID, DLL and directory now use `ToyBoxDisplayEnhancements`. To upgrade from `ToyBoxChineseDisplayEnhancements`, including the earlier 0.4 candidate with the old ID, confirm ToyBox is 2.0.16 and close the game. Install into the new directory, copy `display-settings.json` from the old directory if it exists, then remove the old `ToyBoxChineseDisplayEnhancements` directory from UMM. Keep any backup outside UMM to avoid loading both addons. Start the game and enable the new addon in UMM.

Subsequent versions with the same ID can replace files in `ToyBoxDisplayEnhancements`. The package contains no `display-settings.json`, so it does not overwrite that directory's technical-details preference.

Changing ToyBox's or the game's language refreshes name, search, sort, description and browser caches. Disabling the addon or selecting an unsupported ToyBox language restores original displays. Reopen a visible preview if needed. To uninstall, close the game and remove `ToyBoxDisplayEnhancements`.

The retained Chinese-only 0.3 package targets **ToyBox 2.0.16**. Older 0.1/0.2 packages target **ToyBox 1.7.34** and cannot be used with 2.0.16. Install one version at a time. This candidate has passed 780 managed assertions and 182 actual-assembly checks; in-game layout and language-switching acceptance are pending.

### Credits and license

Addon: **mitcheese**. ToyBox: Narria, ADDB and upstream contributors. The MIT license and full credits are included.

## Download file information

- Name: ToyBox Display Enhancements 0.4 Candidate
- Version: `0.4.0`
- File: `toybox-display-enhancements-v0.4.zip`
- Description: For ToyBox 2.0.16 prerelease and game 1.6.1.514. Install with Modfinder or extract into the UMM mod directory. Enable the addon, select ToyBox's English or Simplified Chinese UI, and enable dialogue previews. Addon text follows ToyBox; official text follows the game.
- Retained file: `toybox-chinese-display-enhancements-v0.3.zip` is Chinese-only and targets ToyBox 2.0.16.
- Legacy files: 0.1/0.2 ZIPs are for ToyBox 1.7.34 only.

## Images

Old screenshots are not presented as the 2.0.16 interface. Use the text examples until new screenshots are available after in-game acceptance.
