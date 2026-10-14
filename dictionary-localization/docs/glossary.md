# 新版术语对照

适用 ToyBox **2.0.16**、词表 **2.0** 和显示增强 **0.3.0**。游戏术语以本机游戏 **1.6.1.514** 的 `enGB.json`／`zhCN.json` 同一文本 GUID 为依据。工具操作根据新版功能语境译写，保留已有准确译文。

## 界面与操作

| 原文 | 采用译法 | 说明 |
| --- | --- | --- |
| Bag of Tricks | 百宝袋 | 保留新版页签译法。 |
| Quality of Life | 便利功能 | 保留新版已有译法。 |
| Search 'n Pick | 搜索与选取 | 不仅用于添加，也用于对不同蓝图执行操作。 |
| Archetype / Career | 职业 | 游戏用语；各阶段职业仍按实际说明区分。 |
| Respec / Retrain | 重置加点 | 说明从 0、15、35 级开始时可重选哪些职业。 |
| Etude | 剧情状态 | 用于状态及进度控制；不译作“研究”。 |
| Fact | 状态项 | 角色上的状态、能力等引擎记录。 |
| Buff | 状态效果 | 可包含正面和负面效果。 |
| Patch Tool / Patch Manager | 蓝图补丁工具／补丁管理器 | 修改对象为蓝图；说明加载时应用与共享对象修改的区别。 |
| Field of View | 视野角度 | 影响视角拉近／拉远，不等同于迷雾揭示范围。 |
| Field Of View Reveal Multiplier | 视野揭示范围倍率 | 调整揭开战争迷雾的范围。 |
| Show Conditions / Select Conditions | 出现条件／选择要求 | 区分选项出现和允许选择的条件。 |
| SoulMarkRequired / SoulMarkShift | 信念要求／信念变化 | 不将信念等级要求一律写成点数门槛。 |

## 游戏机制与检定

| 原文或内部方向 | 采用译法 | 依据 |
| --- | --- | --- |
| Momentum | 士气 | 官方。 |
| Resolve | 决心 | 官方。 |
| Psy Rating | 灵能等级 | 官方。 |
| Veil Degradation | 障壁恶化值 | 官方；设置值说明保持相应计数语境。 |
| Profit Factor | 利润因子 | 官方。 |
| Navigator's Insight | 导航者洞察 | 官方。 |
| Psychic Phenomena | 灵能异象 | 官方。 |
| Perils of the Warp | 亚空间的威胁 | 官方；设置说明按轻微／严重威胁区分。 |
| Sanctioned Psyker | 合法灵能者 | 官方。 |
| Exemplar | 典范 | 官方。 |
| Faith / Dogmatic | 教条主义 | 游戏本地化优先。 |
| Hope / Iconoclast | 自由主义 | 游戏本地化优先。 |
| Corruption / Heretical | 异端主义 | 游戏本地化优先。 |
| Fellowship | 社交 | 官方，不写“魅力”。 |
| Ballistic Skill | 弹道技能 | 官方。 |
| Willpower | 意志 | 官方。 |
| SkillCarouse / SkillCoercion | 饮酒／胁迫 | 官方。 |
| SkillAwareness / SkillPersuasion | 警觉／劝说 | 官方。 |
| SkillTechUse | 科技使用 | 官方。 |
| Lore (Imperium / Warp / Xenos) | 知识（帝国／亚空间／异形） | 官方。 |

显示增强通过游戏 `UIUtility` 读取信念与检定名称。未知方向或检定保留可辨识的原名称，不按枚举名字臆译。`Heresy` 的“异端”是按已登记 GUID 提供的剧情状态别名；“未在运行”不等于“从未触发”。

## 派系与虚空舰

| 原文 | 采用译法 | 依据 |
| --- | --- | --- |
| Drusians | 德鲁苏斯教派 | 官方声望派系名称。 |
| Explorators | 开拓者 | 官方。 |
| Kasballica Mission / Kasballica | 卡斯巴利卡犯罪集团 | 官方。 |
| Voidship / Ship | 虚空舰 | 官方。 |
| Fore / Aft armour | 舰首／舰尾护甲 | 官方。 |
| Port / Starboard armour | 左舷／右舷护甲 | 官方。 |
| Armour Plating | 护甲船板 | 官方。 |
| Auger Array | 鸟卜仪阵列 | 战锤语境。 |
| Geller Field Device | 盖勒力场装置 | 设定译法，保留内部 Geller 标识。 |
| Warp Drives | 亚空间引擎 | 战锤航行语境。 |
| Augmentation / Augment | 植入物 | 官方物品类别。 |
| Cyber-Mastiff / Cyber-Eagle | 生化獒犬／生化猎鹰 | 官方。 |
| Servoskull | 伺服头骨 | 官方。 |

## 维护边界

本表用于修订和运行时显示，不是可新增的英文查找键清单。新版仅通过登记字段 ID 构建词表，未登记的枚举和按钮不计入覆盖；不再生成旧版有限枚举扩展或颜色变体键。蓝图内部名、类名、字段名和 GUID 保留原标识。
