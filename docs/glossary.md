# 术语对照

优先参考本机游戏 1.6.1.514 的 `enGB.json` 与 `zhCN.json`，通过同一文本 GUID 对照。下表“官方”表示找到直接对应；工具内部术语与枚举按实际界面功能译写。

## 核心界面与机制

| 原文／内部枚举 | 本修订 | 依据与说明 |
|---|---|---|
| Archetype / Archetypes | 职业 | 官方；替换“变体” |
| Career / Careers | 职业路径 | 工具描述的是各阶段职业路径 |
| Etude / Etudes | 剧情状态 | 控制剧情条件和进度；替换“研究” |
| Respec | 重置加点 | 操作用途明确；替换含糊的“重新分配” |
| Search 'n Pick | 搜索与添加 | 统一页签与按钮相关说明 |
| Bag of Tricks | 实用工具 | 面向玩家的功能总览页 |
| Fact / Facts | 状态项 | 引擎挂载在角色上的状态、能力等，不是“实物” |
| Buff / Buffs | 状态效果 | 同时包含正面和负面效果 |
| Momentum | 士气 | 官方；不把它直译成“动量” |
| Veil Degradation / UI Veil Thickness | 障壁恶化／障壁恶化值 | 官方；源码的 Thickness 在此显示恶化计数 |
| Resolve | 决心 | 官方 |
| Psy Rating | 灵能等级 | 官方 |
| Profit Factor | 利润因子 | 官方 |
| Navigator's Insight | 导航者洞察 | 官方 |
| Psychic Phenomena | 灵能异象 | 官方 |
| Perils of the Warp | 亚空间的威胁 | 官方 |
| MinorPerils / MajorPerils | 轻微／严重亚空间威胁 | 两组设置的说明性译法 |
| Soul Marks | 信念 | 当前角色信念系统，不是“灵魂伴侣” |
| Faith / Dogmatic | 教条主义 | UI 枚举 Faith 对应官方信念路线 |
| Hope / Iconoclast | 自由主义 | UI 枚举 Hope 对应官方信念路线 |
| Corruption / Heretical | 异端主义 | UI 枚举 Corruption 对应官方信念路线 |
| Reason | 理性 | 内部信念方向的直观名称 |
| Fellowship | 社交 | 官方；修正“魅力” |
| Ballistic Skill | 弹道技能 | 官方 |
| Willpower | 意志 | 官方 |
| SkillCarouse | 饮酒 | 官方；修正“狂欢” |
| SkillCoercion | 胁迫 | 官方 |
| SkillAwareness | 警觉 | 官方 |
| SkillPersuasion | 劝说 | 官方 |
| SkillTechUse | 科技使用 | 官方 |
| Lore (Imperium / Warp / Xenos) | 知识（帝国／亚空间／异形） | 官方 |
| Sanctioned Psyker | 合法灵能者 | 官方；身份覆盖功能 |
| Exemplar | 典范 | 官方；职业选择警告 |
| Patch Tool / Patch Manager | 蓝图补丁工具／补丁管理器 | 编辑对象为游戏蓝图 |

## 派系与虚空舰

| 原文 | 本修订 | 依据 |
|---|---|---|
| Drusians | 德鲁苏斯教派 | 官方；语境为声望派系，选“教派”而非单个教徒 |
| Explorators | 开拓者 | 官方 |
| Kasballica Mission / Kasballica | 卡斯巴利卡犯罪集团 | 官方 |
| Voidship / Ship | 虚空舰 | 官方，统一工具内相关名称 |
| Fore / Aft armour | 舰首／舰尾护甲 | 官方 |
| Port / Starboard armour | 左舷／右舷护甲 | 官方 |
| Armour Plating | 护甲船板 | 官方；条目显示为“虚空舰护甲船板” |
| Auger Array | 鸟卜仪阵列 | 采用战锤语境译法 |
| Geller Field Device | 盖勒力场装置 | 战锤专名；不改变内部 Geller 标识 |
| Warp Drives | 亚空间引擎 | 战锤航行机制；替换其他作品语境的“曲速” |
| Augmentation / Augment | 植入物 | 官方物品类别 |
| Cyber-Mastiff / Cyber-Eagle | 生化獒犬／生化猎鹰 | 官方；协议类别采用对应名称 |
| Servoskull | 伺服头骨 | 官方；宠物协议名称采用此译法 |

## 保留与备选

“星舰”可作为 Voidship 的自然泛称，但本项目使用官方“虚空舰”，避免同一物品类别有多种名称。“伺服颅骨”是可考虑的设定译法，本次仍按本地官方“伺服头骨”。Geller 可能存在“盖勒／盖拉”等不同音译；本次采用“盖勒力场”，后续若有明确官方文本对照，再统一调整。

“开拓者／探索者”“卡斯巴利卡／卡斯巴利加”等没有采用其他音译或惯用译法；当前官方词表能提供明确对应，优先保持玩家能在游戏里找到的名称。

蓝图 ID、类名、字段名、GUID 不进行术语替换。作为选项显示的有限枚举可译，词表键保留其原枚举字符串。

体型顺序采用极微型、超微型、微型、小型、中型、大型、超大型、巨型、超巨型，区分 Fine、Diminutive、Tiny 等原本容易混同的等级。
