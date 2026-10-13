[English](README.md) | [简体中文](README.zh-CN.md)

# ToyBox Chinese Display Enhancements

An optional companion mod for ToyBox's Simplified Chinese interface in **Warhammer 40,000: Rogue Trader**. It makes supported dialogue previews easier to understand and adds Chinese display names or explanatory notes to eight internal feature entries.

This addon changes display text only. It does not replace ToyBox, change feature effects, modify blueprints or saves, or execute dialogue actions.

## Version and Requirements

- Addon: **0.2** (UMM version `0.2.0`), candidate release.
- Requires the original **ToyBox 1.7.34** and Unity Mod Manager; build target: game **1.6.1.514**.
- Select **Simplified Chinese (`zh-CN`)** in ToyBox. Use Simplified Chinese as the game language to display official Chinese quest and objective titles.
- The dictionary-only Chinese localization **1.0** is a separate package. This addon can be used alongside it and does not install or overwrite that dictionary.

## Installation

1. Close the game. Install the original ToyBox first.
2. Extract this addon's ZIP into the existing game's UMM mod directory. Keep the `ToyBoxChineseDisplayEnhancements` folder intact, alongside `0ToyBox0`, **not inside it**. The default Windows directory is:

   ```text
   %USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\
   ├── 0ToyBox0\
   └── ToyBoxChineseDisplayEnhancements\
       ├── Info.json
       ├── ToyBoxChineseDisplayEnhancements.dll
       ├── feature-aliases.zh-CN.json
       └── story-aliases.zh-CN.json
   ```

3. Start the game and enable **ToyBox Chinese Display Enhancements** in UMM. Its panel reports whether the display patches loaded.
4. Select ToyBox's Simplified Chinese interface. Enable ToyBox's own dialogue result/condition previews to see the enhanced summaries; the addon does not turn those options on automatically.
5. Compact display is the default. Enable **Show technical details** in this addon's UMM panel to include original captions, internal identifiers and source depth. The preference saves automatically. Already displayed dialogue updates when the game refreshes it; do not advance the story just to force a refresh.

## Improvements

### Dialogue Results and Conditions

- Supported quest objectives are shown as "start objective", "complete objective" or "objective failed", using the game's localized objective and quest titles instead of blueprint IDs. The screenshot's three objectives resolve to **返回着陆区域**, **与总督谈谈** and **探寻星站，寻找答案**.
- **出现条件** (appearance conditions) and **选择要求** (selection requirements) distinguish when an option appears from when it can be chosen. Single conditions are stated directly; multiple conditions use indented "all conditions" or "any group" lists. Supported AND/OR groups are expanded, preserving whole-group negation.
- The specific `Heresy` story state receives the addon-provided alias **异端**, for example "the story state is not running". This does not mean it was never triggered or identify the player's conviction route. States without a registered alias retain their internal names.
- Current-depth summaries use **结果预览** (result preview); later depths use **后续可能涉及** (possible later outcomes). Numeric depths appear only in technical details. The long warning is no longer repeated on every option: previews follow ToyBox's branch collection rules, and later outcomes depend on branches and checks rather than being guaranteed.
- Chinese summaries cover merchant selection, the custom companion creation flow and its Profit Factor requirement. They do not evaluate whether the requirement is currently met or describe the threshold as a guaranteed deduction.
- "Dialogue already shown" conditions quote up to 40 characters of official dialogue where available, distinguishing the current conversation from prior records. Missing text falls back to the original identifier; "shown" is not described as "chosen".
- Conviction changes show localized direction names and signed values. Supported point requirements show their threshold; checks show the skill, difficulty and hidden-check status. The same formatting is used by the game's ToyBox previews and ToyBox's dialogue editor.

### Internal Feature Entries

- The three `SoulMark_Base` entries are identified as internal records for **异端主义 / 自由主义 / 教条主义**, rather than ordinary selectable talents.
- Chinese aliases cover the base-character feature bundle, post-combat reload entry and custom character-creation path. Supplemental notes appear in the existing description area and are labeled **【汉化说明】** (localization note), not official game text.
- `IsPlayer_Feature` and `FeatProgressionTest` keep their internal names: notes describe confirmed structure without inventing effects. Existing valid official names and descriptions always take priority.
- Search supports the Chinese alias, original internal name and GUID. Enable **Search Descriptions** to search supplemental notes. **Show Internal Names** retains the original identifier beside translated aliases; sorting follows the displayed name.

## Disable, Remove and Update

Disable this addon in UMM to restore native text. Refresh the ToyBox list after toggling the addon or changing language; if a previously opened list retains its order, close and reopen it. To uninstall, close the game and remove only `ToyBoxChineseDisplayEnhancements` from the UMM directory. Do not remove or replace `0ToyBox0`.

After updating ToyBox, use a compatible addon release. If the required ToyBox version or patch methods do not match, enhancements stay inactive and UMM logs the reason. Your existing dictionary localization is unaffected.

To upgrade from 0.1, close the game and overwrite this addon's files in the same `ToyBoxChineseDisplayEnhancements` folder with 0.2. Do not enable two copies. To roll back, close the game and overwrite the addon files with the retained 0.1 package. Version 0.2's `display-settings.json` contains display preferences only; it is not distributed or overwritten by the ZIP. Version 0.1 ignores it.

## Scope and Limitations

Version 0.2 covers the supported dialogue cases and eight feature entries described above, not every action, condition or blueprint. Unsupported types, dynamic story-state evaluation, rank-based conviction requirements and quests/objectives without usable official titles retain ToyBox's original summary. Some English internal identifiers therefore remain intentional.

The addon preserves ToyBox's existing preview traversal and branch decisions; it does not evaluate extra conditions or predict all possible outcomes. Official content names follow the game's language. Feature notes describe internal records and are not instructions to add or remove them. The 0.2 candidate's layout still needs confirmation in the game UI.

## Credits and License

Addon by **mitcheese**, built for ToyBox by Narria, ADDB and the upstream contributors. Original Chinese localization contributors are acknowledged in [CREDITS.md](CREDITS.md).

Distributed under the MIT license; see [LICENSE.txt](LICENSE.txt) and [CREDITS.md](CREDITS.md). Game assemblies and full game localization files are not included.
