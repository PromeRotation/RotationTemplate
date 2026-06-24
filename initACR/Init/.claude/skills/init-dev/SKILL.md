---
name: init-dev
description: Init ACR 开发与维护——基于 PromeRotation 的 ACR 模板。用于从零开发新职业 ACR 时使用。
---

# Init ACR 开发模板

Init 是一个基于 **PromeRotation** 框架的 FFXIV ACR 开发模板。

> 从 `LccMch` (机工士 ACR) 提炼而来，保留了完整的项目架构和 API 壳，
> 将职业逻辑替换为 TODO 注释和伪代码，方便快速启动新职业开发。

## 项目结构

```
Init/
├── Init.csproj                  # 动态取最新 PromeRotation 版本，输出到 ACR\Init
├── InitRotation.cs              # ★ 核心协调器：Resolver 注册 + 循环 + UI 委托
├── InitRotationEventHandler.cs  # 事件回调 + IRotationLifecycle（OnEnterAcr/OnExitAcr）
├── JobHelper.cs                 # 职业辅助方法：连击链/AoE判断/高价值GCD选择/量谱/免疫检测
├── ApiHelper.cs                 # ★ 中文 API 壳（完整保留，全量 API）
├── Data/
│   ├── InitSkill.cs             # 技能 ID 常量（TODO: 替换为你的职业）
│   ├── InitBuff.cs              # Buff/Debuff ID 常量（TODO: 替换）
│   ├── InitQT.cs                # ★ QT 唯一数据源（键名 + 默认值）
│   ├── InitBattleData.cs        # 单场战斗缓存
│   └── InitSettings.cs          # 运行期设置 + JSON 持久化 + QT 默认值管理
├── Action/
│   ├── Gcd/
│   │   └── GcdTemplate.cs       # ★ GCD Resolver 模板（复制此文件添加新 GCD）
│   └── OffGcd/
│       └── OffGcdTemplate.cs    # ★ oGCD Resolver 模板（复制此文件添加新 oGCD）
├── Opener/
│   ├── InitOpenerManager.cs     # 起手调度（按等级 + 设置选择）
│   ├── InitOpenerChecker.cs     # 起手爆发技能就绪校验
│   └── OpenerTemplate.cs        # ★ 起手模板（复制此文件添加新起手）
├── UI/
│   ├── InitQTUI.cs              # QT 面板 + 联动回调
│   ├── InitSettingsUI.cs        # 设置面板（通用/QT管理/默认值管理/Dev）
│   └── InitHotkeyUI.cs          # 热键面板
└── Helper/
    └── InitMacroManager.cs       # 聊天命令 /Init（切换QT/保存/模式切换）
```

## 快速开始：从模板到你的职业 ACR

### 第 1 步：改名字

全局搜索替换以下内容:
- `Init` → 你的 ACR 名称（如 `Bard`, `Smn`, `Whm`）
- `/Init` → 你的聊天命令（如 `/Bard`, `/Smn`）
- `Init.Settings.json` → 你的设置文件名

csproj 中的输出路径:
```xml
<OutputPath>...\ACR\Init</OutputPath>  → 改为你的 ACR 目录名
```

### 第 2 步：填数据常量

**InitSkill.cs** — 替换为你的职业技能 ID:
```csharp
public const uint 基础连击1 = 你的ID;
public const uint 基础连击2 = 你的ID;
// ...
```

**InitBuff.cs** — 替换为你的职业 Buff/Debuff ID:
```csharp
public const uint 某Buff = 你的ID;
```

### 第 3 步：写 Resolver

每个技能一个 Resolver 文件，实现 `IDecisionResolver`:

```csharp
public class 你的技能Gcd : IDecisionResolver
{
    public CheckResult Check()
    {
        // 标准前检查链（复制 GcdAction1 的模板）
        // + 技能特定条件
        return new(true/false, "原因");
    }

    public PAction GetAction()
    {
        return new(InitSkill.某技能, ActionType.Gcd, ActionTargetType.Target);
    }
}
```

然后在 `InitRotation` 构造函数中按优先级注册:
```csharp
_gcdResolvers.Add(new 你的技能Gcd());      // 优先级高 → 先注册
_gcdResolvers.Add(new 基础连击Gcd());      // 优先级低 → 后注册（兜底）
```

### 第 4 步：配 QT 开关

只需改 `InitQT.cs`:
```csharp
public const string 新功能 = "新功能";
// 在 All 字典加: { 新功能, true },
// 在 ModeCategories 加: { 新功能, QTMode.Common },
```

然后在 `InitQTUI.cs` 对应 Tab 加一行:
```csharp
QtToggle(InitQT.新功能, "新功能");
```

### 第 5 步：写起手

参考 `Opener1.cs`，实现 `IOpener`:
- `InitializeCountdown` — 倒计时技能
- `InCombatSequence` — 开怪后 GCD+oGCD 序列

### 第 6 步：构建验证

```powershell
cd Init
dotnet build
# 输出 → %APPDATA%\XIVLauncherCN\pluginConfigs\PromeRotation\ACR\Init\Init.dll
```

进游戏 → 切目标职业 → 打开 PromeRotation 面板 → 确认 ACR 列表中显示你的名字 → 打木人验证。

## 核心开发流程

```
PromeRotation 每帧调用:
  OnEnterAcr()              → 切换到本 ACR 时（IRotationLifecycle）
  NextAlways()               → 立即执行（无视 GCD）
  NextGcd()                  → GCD 转好时调用 → 遍历 _gcdResolvers
  NextOffGcd()               → GCD 转着时调用 → 遍历 _offGcdResolvers
  GetOpener()                → 进战时调用，返回起手序列
  OnExitAcr()                → 从本 ACR 切走时（IRotationLifecycle）

每个 Resolver:
  Check()    → "能不能/该不该放？" → 返回 CheckResult(Success, Message)
  GetAction() → "放什么？" → 返回 PAction(技能ID, 类型, 目标)

优先级 = 注册顺序。先 Add 的 Resolver 先被 Check，第一个成功的就执行。
```

## API 速查（全部走 ApiHelper 壳）

| 需求 | 调用 |
|------|------|
| 玩家/目标 | `ApiHelper.玩家` `ApiHelper.目标` |
| 技能可用 | `ApiHelper.技能可用(id)` |
| 技能冷却 | `ApiHelper.技能冷却(id)` |
| 技能充能 | `ApiHelper.技能充能(id)` |
| Buff 判断 | `ApiHelper.玩家有状态(buffId)` |
| Buff 剩余 | `ApiHelper.状态剩余(player, buffId)` |
| GCD 剩余 | `ApiHelper.GCD剩余` |
| 连击剩余 | `ApiHelper.连击剩余` |
| 是否读条 | `ApiHelper.读条中` |
| 血量 | `ApiHelper.玩家血量` |
| 周围敌人 | `ApiHelper.周围敌人(range)` |
| 扇形敌人 | `ApiHelper.扇形敌人(me, target, range, angle)` |
| 最佳AoE目标 | `ApiHelper.最佳AoE目标(spellId, minTargets, angle)` |
| 攻击距离 | `ApiHelper.攻击距离(baseRange)` |
| QT 读写 | `ApiHelper.获取QT(key)` `ApiHelper.设置QT(key, val)` |
| 量谱 | `ApiHelper.XXX.量谱属性`（如 `ApiHelper.MCH.热量`） |
| 爆发药 | `ApiHelper.最佳爆发药` |
| 防重复 | `ApiHelper.最近用过(id, ms)` |
| 屏幕提示 | `ApiHelper.提示(msg, sec)` |
| 构建 PAction | `ApiHelper.自我能力(id)` → OffGcd+Self |
| 切换目标 | `ApiHelper.切换目标(target)` |
| 技能排程 | `ApiHelper.排入队列(action, 高优先?)` |
| 动画锁定 | `ApiHelper.动画锁定` |
| 扩展方法 | `角色.血量()` `角色.距玩家()` `对象.是敌人()` |

## 设计原则

1. **单一数据源**：QT 键名+默认值只在 `InitQT.All` 一处定义
2. **API 壳隔离**：所有 PR 框架调用走 `ApiHelper`，框架升级只改壳
3. **Resolver 模式**：每个技能一个 Resolver，Check → GetAction，优先级靠注册顺序
4. **UI 解耦**：QT/设置/热键 UI 在独立 `UI/` 目录，InitRotation 只做协调
5. **起手动态**：`InCombatSequence` 是 getter，运行时检测条件

## 常见问题

| 问题 | 解决 |
|------|------|
| `dotnet build` 失败 | 检查 PR 版本目录是否存在 `PromeRotation.dll` |
| ACR 不加载 | 输出目录名必须和 `RotationMetadata` 的 author 一致 |
| 技能不放但 Debug 显示成功 | 等级不够 / 技能 ID 错 / GCD 窗口不足 |
| 新增 QT 不生效 | 确认在 `InitRotation` 构造函数中调了 `ApiHelper.添加QT` |
| ApiHelper 找不到某方法 | 查看 `ApiHelper.cs` 文件，全量 API 已包含 |
