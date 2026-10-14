# ToyBox Display Enhancements

[中文说明](README.md)

Adds readable English and Simplified Chinese text to ToyBox dialogue previews: quest and objective names, dialogue quotations, vendor selection and custom companion recruitment requirements. It also supplies names or explanatory notes for eight internal feature entries.

## Version and language support

- Addon: **0.4.0 candidate**.
- Requires [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1) **2.0.16 prerelease**; checked against game **1.6.1.514**.
- Rogue Trader includes Unity Mod Manager (UMM), so no separate UMM installation is needed.
- Can be used independently or alongside **ToyBox Simplified Chinese Localization 2.0**, which installs separately.

**Addon labels, aliases, notes and technical details follow ToyBox's UI language. Official names and dialogue follow the game's language.** Supported ToyBox languages are English (`en`, including regional codes such as `en-GB` and `en-US`) and Simplified Chinese (`zh-CN`). There is no separate addon language setting.

| ToyBox language | Game language | Display |
| --- | --- | --- |
| English | English (`enGB`) | English addon text and official English text |
| Simplified Chinese | Simplified Chinese (`zhCN`) | Chinese addon text and official Chinese text |
| English | Simplified Chinese | English addon text, Chinese official names and dialogue |
| Simplified Chinese | English | Chinese addon text, English official names and dialogue |

Other ToyBox UI languages retain the original displays. Other game languages retain valid official text; missing names without a reviewed fallback retain their internal identity or original caption. No other addon languages are included in this release.

## Installation

Install ToyBox **2.0.16**, then close the game. Choose either method below. If you have the old `ToyBoxChineseDisplayEnhancements` addon, follow Updating and uninstalling below to transfer settings and remove its directory before starting the game.

### Using Modfinder

1. Download and open [Modfinder](https://www.nexusmods.com/warhammer40kroguetrader/mods/146).
2. Drag `toybox-display-enhancements-v0.4.zip` into **"Drag zips here to install"** at the bottom of its window. Wait for installation to finish.
3. Check that `ToyBoxDisplayEnhancements` is alongside `0ToyBox0` in the UMM mod directory.

Use the downloaded ZIP for this candidate; you do not need to find the addon in Modfinder's catalogue.

### Manual installation

Extract the ZIP into your existing UMM mod directory. The default Windows location is:

```text
%USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager
```

The resulting layout must include:

```text
UnityModManager\
  0ToyBox0\
  ToyBoxDisplayEnhancements\
    Info.json
    ToyBoxDisplayEnhancements.dll
    feature-aliases.zh-CN.json
    feature-aliases.en.json
    story-aliases.zh-CN.json
    story-aliases.en.json
```

### Enable and configure

1. Start the game, press `Ctrl + F10` to open UMM, and enable **ToyBox Display Enhancements**. Its mod ID and installation directory are `ToyBoxDisplayEnhancements`; its DLL is `ToyBoxDisplayEnhancements.dll`.
2. Set ToyBox's UI language to English (`en`) or Simplified Chinese (`zh-CN`). In Bag of Tricks, enable **Dialog Results** and **Dialog Conditions** in the Preview section.
3. Set the game's language to English for official English names and dialogue. Set both ToyBox and the game to Simplified Chinese for a fully Chinese display. Mixed settings use the rules in the table.
4. For original captions, internal names, GUIDs and source depth, enable **Show technical details** in this addon's UMM panel. The preference is saved automatically, and panel text updates when ToyBox's language changes.

## Main features

- **Dialogue results:** readable vendor and recruitment actions; objective names with their parent quest. **Result preview** identifies the current layer, while **Possible later results** identifies later possible results.
- **Conditions:** **Availability conditions** explains when an option appears; **Selection requirements** explains when it can be selected. AND/OR, nesting and whole-group negation are preserved. Dialogue-history conditions quote the related line and distinguish this conversation from previous records. Excerpts are limited to 120 characters in English, preserving whole words where possible, or 40 characters in Chinese, according to ToyBox's language.
- **Story states:** the registered Heresy condition reads `Story state "Heresy" is not running`, describing its current state without inferring whether it has occurred before.
- **Convictions and checks:** names use the game's localization where available, including Dogmatic, Iconoclast, Heretical, Persuasion and Awareness in English. Changes show signed values; point requirements show a minimum. Rank requirements and unknown types retain identifiable original text.
- **Internal features:** eight entries receive aliases or notes marked **Addon note** in English, or **汉化说明** in Chinese. Valid official names and descriptions take priority. Search supports English and Chinese aliases, internal names and GUIDs. Addon-note search follows ToyBox's description-search setting and uses only the current addon language.

With both ToyBox and the game set to English, examples include:

> Result preview: Open vendor selection
>
> Selection requirements: Profit Factor meets the requirement for recruiting a custom companion
>
> Possible later results: Begin custom companion creation

The eight entries cover three internal conviction records, base character features, automatic reload after combat, a custom character-creation path, `IsPlayer_Feature` and `FeatProgressionTest`. The last two keep their internal names because their full roles are not confirmed. Notes describe reviewed blueprint structure rather than recommending changes to these entries.

Later preview results depend on dialogue branches and checks. The addon uses ToyBox's existing collection rules and does not execute actions or add condition evaluations. Changing either ToyBox's or the game's language refreshes name, search, sort, description and browser caches. Disabling the addon or selecting an unsupported ToyBox language restores original displays; reopen a visible preview if needed.

## Updating and uninstalling

The mod ID, DLL and installation directory now use `ToyBoxDisplayEnhancements`. If upgrading from `ToyBoxChineseDisplayEnhancements`, including the earlier 0.4 candidate with the old ID, do not overwrite the old directory:

1. Confirm ToyBox is **2.0.16 prerelease**, close the game, and install the new package into `ToyBoxDisplayEnhancements`.
2. If the old directory contains `display-settings.json`, copy it into the new directory to keep your technical-details preference. Without that file, the addon uses its defaults.
3. Remove the old `ToyBoxChineseDisplayEnhancements` directory from UMM. Keep any backup outside the UMM directory; do not leave both addons installed.
4. Start the game and enable **ToyBox Display Enhancements** in UMM. Its new mod ID requires enabling it again.

For subsequent versions using `ToyBoxDisplayEnhancements`, close the game and replace files in that same directory. The package contains no `display-settings.json` and does not overwrite that directory's preferences. To uninstall, close the game and remove `ToyBoxDisplayEnhancements`.

The retained Chinese-only 0.3 ZIP targets **ToyBox 2.0.16**. The older 0.1/0.2 ZIPs target **ToyBox 1.7.34** and do not work with 2.0.16. Install one version of this addon at a time.

This candidate has passed 279 managed formatting/Harmony assertions and 81 actual-assembly compatibility checks. In-game loading, layout and language-switching acceptance are pending; new screenshots will follow that review.

## Credits and license

Addon: **mitcheese**. ToyBox: Narria, ADDB and upstream contributors. See [CREDITS.md](CREDITS.md), [CHANGELOG.md](CHANGELOG.md) and the [MIT license](LICENSE.txt).
