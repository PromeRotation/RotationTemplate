# Init 模版使用教程：从零到第一个 ACR

**适用版本**：Init 模版（2026-06-24），PromeRotation 外部 ACR 接口。

**读者画像**：你想写一个新职业的 ACR，不想从空项目搭脚手架。Init 模版已把项目结构、API 壳、QT 系统、起手框架全部准备好，你只需填职业逻辑。

---

## 快速开始（30 秒）

双击运行 `setup.bat`，按提示输入。假设输入了以下信息：

```
ACR 名称：Smn
作者名：  MySmn
显示名：  新手召唤
版本号：  1.0.0.0
职业：    19 (召唤师)         ← 选了 SMN，职业缩写就是 SMN
```

脚本自动替换完成后：

```powershell
cd MySmnSmn            # 目录已重命名为 {作者名}{ACR名}
dotnet build
```

---

## setup.bat 改了什么（对照表）

下表用 `{acr}` = 你输入的 ACR 名、`{author}` = 作者名、`{JOB}` = 职业枚举（大写），说明每个文件替换前后的对应关系。以 `acr=Smn, author=MySmn, JOB=SMN` 为实例：

| 替换前（模版） | 替换规则 | 替换后实例 | 说明 |
|------|------|------|------|
| `Init.csproj` | `{acr}.csproj` | `Smn.csproj` | 输出路径 `ACR\Init` → `ACR\{author}` |
| `ApiHelper.cs` | 不改名 | `ApiHelper.cs` | namespace / using 全部替换 |
| `InitRotation.cs` | `{acr}Rotation.cs` | `SmnRotation.cs` | 核心协调器，`RotationMetadata` 自动更新 |
| `InitRotationEventHandler.cs` | `{acr}RotationEventHandler.cs` | `SmnRotationEventHandler.cs` | 事件回调 |
| `JobHelper.cs` | `{JOB}Helper.cs` | `SMNHelper.cs` | 职业辅助，类名同步改为 `SMNHelper` |
| `Data/InitSkill.cs` | `Data/{acr}Skill.cs` | `Data/SmnSkill.cs` | 技能 ID 常量 |
| `Data/InitBuff.cs` | `Data/{acr}Buff.cs` | `Data/SmnBuff.cs` | Buff/Debuff ID 常量 |
| `Data/InitQT.cs` | `Data/{acr}QT.cs` | `Data/SmnQT.cs` | QT 唯一数据源 |
| `Data/InitSettings.cs` | `Data/{acr}Settings.cs` | `Data/SmnSettings.cs` | JSON 持久化设置 |
| `Data/InitBattleData.cs` | `Data/{acr}BattleData.cs` | `Data/SmnBattleData.cs` | 单场战斗缓存 |
| `Opener/InitOpenerManager.cs` | `Opener/{acr}OpenerManager.cs` | `Opener/SmnOpenerManager.cs` | 起手调度 |
| `Opener/InitOpenerChecker.cs` | `Opener/{acr}OpenerChecker.cs` | `Opener/SmnOpenerChecker.cs` | 起手条件校验 |
| `UI/InitQTUI.cs` | `UI/{acr}QTUI.cs` | `UI/SmnQTUI.cs` | QT 面板 |
| `UI/InitSettingsUI.cs` | `UI/{acr}SettingsUI.cs` | `UI/SmnSettingsUI.cs` | 设置面板 |
| `UI/InitHotkeyUI.cs` | `UI/{acr}HotkeyUI.cs` | `UI/SmnHotkeyUI.cs` | 热键面板 |
| `Helper/InitMacroManager.cs` | `Helper/{acr}MacroManager.cs` | `Helper/SmnMacroManager.cs` | 命令 `/Init` → `/{acr}` |
| `Action/Gcd/GcdTemplate.cs` | 不改名 | `Action/Gcd/GcdTemplate.cs` | 📋 GCD 模板 |
| `Action/OffGcd/OffGcdTemplate.cs` | 不改名 | `Action/OffGcd/OffGcdTemplate.cs` | 📋 oGCD 模板 |
| `Opener/OpenerTemplate.cs` | 不改名 | `Opener/OpenerTemplate.cs` | 📋 起手模板 |
| 项目目录 `Init/` | `{author}{acr}/` | `MySmnSmn/` | 目录改名 |
| 技能目录 `init-dev` | `{acr小写}-dev` | `smn-dev` | `.claude/skills/` 下 |

> **注意**：`JobHelper.cs` 的重命名用的是职业枚举（如 `SMN`、`BRD`、`MCH`），不是 ACR 名。所以 `{acr}Helper` 和 `{JOB}Helper` 可能不同（如上例 `SmnRotation.cs` 但 `SMNHelper.cs`）。

**你要做的只有三件事：填数据常量 → 复制模板写 Resolver → 注册。**

---

## 0. ACR 加载规则

1. 主类实现 `IRotation`，标注 `[RotationMetadata(job, "显示名", "作者名", "版本")]`
2. **输出目录最后一级 = 作者名**。模版默认 `ACR\Init\`；setup.bat 改后输出到 `ACR\{author}\`
3. 同一职业同一作者名不能重复注册
4. 编译输出路径示例（假设 author=MySmn）：
   ```
   %APPDATA%\XIVLauncherCN\pluginConfigs\PromeRotation\ACR\MySmn\Smn.dll
   ```

---

## 1. 第一步：全局改名

> **说明**：下面所有代码示例以 `acr=Smn, author=MySmn, JOB=SMN` 为例，请替换为你的实际输入。

### 方式 A：用 setup.bat（推荐）

双击运行 → 输入参数 → 自动完成。包括：
- 文件内容全部替换（namespace / 类名 / 字符串 / 命令名）
- `Init*.cs` → `{acr}*.cs`（如 `SmnRotation.cs`）
- `JobHelper.cs` → `{JOB}Helper.cs`（如 `SMNHelper.cs`）
- `Init/` → `{author}{acr}/`（如 `MySmnSmn/`）
- `.csproj` 输出路径 `ACR\Init` → `ACR\{author}`
- `RotationMetadata` 更新职业枚举和元数据
- 聊天命令 `/Init` → `/{acr}`

### 方式 B：手动改（参考上表）

改完 `dotnet build`，通过则继续。

---

## 2. 🏆 里程碑 1：打出第一个 GCD

**目标**：填技能 ID → 实现连击链 → 写 GCD Resolver → 进游戏验证。

### 2.1 填 `Data/SmnSkill.cs`

用你的职业技能 ID 替换：

```csharp
public static class SmnSkill
{
    // === GCD 技能 ===
    public const uint 基础连击1 = 你的ID;   // 例：骑士 快剑 = 9
    public const uint 基础连击2 = 你的ID;
    public const uint 基础连击3 = 你的ID;

    // === 职能技能（已内置，不用改） ===
    public const uint 内丹 = 7541;
    public const uint 冲刺 = 3;
    public const uint 伤头 = 7551;
    public const uint 速行 = 7557;
}
```

> **怎么查技能 ID？** 游戏内 Dalamud 插件 → DataCenter；或 XIVAPI / Teamcraft 网站。

### 2.2 实现 `SMNHelper.GetBaseComboAction()`

打开 `SMNHelper.cs`（原 `JobHelper.cs`），把 `GetBaseComboAction()` 的 `return 0` 替换为：

```csharp
public static uint GetBaseComboAction()
{
    var last = ApiHelper.上个连击;
    if ((last == SmnSkill.基础连击1) && ApiHelper.技能已解锁(SmnSkill.基础连击2))
        return ApiHelper.调整技能ID(SmnSkill.基础连击2);
    if ((last == SmnSkill.基础连击2) && ApiHelper.技能已解锁(SmnSkill.基础连击3))
        return ApiHelper.调整技能ID(SmnSkill.基础连击3);
    return ApiHelper.调整技能ID(SmnSkill.基础连击1);
}
```

### 2.3 复制 `GcdTemplate.cs` 写第一个 Resolver

复制 `Action/Gcd/GcdTemplate.cs` → `Action/Gcd/基础连击Gcd.cs`。

模板里已有标准前检查链（玩家/目标/距离/读条/停手QT），只需改：

```csharp
public class 基础连击Gcd : IDecisionResolver
{
    public CheckResult Check()
    {
        var r = ApiHelper.攻击距离(25);   // 远程=25，近战=3
        var p = ApiHelper.玩家;
        if (p == null) return new(false, "玩家未加载");
        if (ApiHelper.目标 == null) return new(false, "无目标");
        if (ApiHelper.目标!.EntityId == p.EntityId) return new(false, "目标为自己");
        if (ApiHelper.目标.IsPlayer()) return new(false, "目标为玩家");
        if (p.DistanceToMe() > r) return new(false, $"过远(>{r}m)");
        if (ApiHelper.获取QT(SmnQT.停手)) return new(false, "停手");

        return new(true, "基础连击");
    }

    public PAction GetAction()
    {
        return new(SMNHelper.GetBaseComboAction(), ActionType.Gcd, ActionTargetType.Target);
    }
}
```

### 2.4 在 `SmnRotation.cs` 构造函数中注册

```csharp
public SmnRotation()
{
    _gcdResolvers.Add(new 基础连击Gcd());   // ← 加这一行

    foreach (var (n, d) in QtList) ApiHelper.添加QT(n, d);
}
```

### 2.5 构建验证

```powershell
dotnet build
```

| 步骤 | 期望 |
|------|------|
| PromeRotation 切你的职业 | 列表显示你的 ACR |
| 选中木人开怪 | 自动打 1-2-3 连击 |
| Debug 面板 GCD 区 | `基础连击Gcd / Success=true` |

> **🎉 里程碑达成！** ACR 加载成功，角色开始自动打连击了。

---

## 3. 🏆 里程碑 2：加一个 oGCD

**目标**：GCD 间隙自动插能力技（以内丹为例）。

### 3.1 复制 `OffGcdTemplate.cs`

复制 `Action/OffGcd/OffGcdTemplate.cs` → `Action/OffGcd/内丹OffGcd.cs`：

```csharp
public class 内丹OffGcd : IDecisionResolver
{
    public CheckResult Check()
    {
        if (ApiHelper.玩家 == null) return new(false, "玩家未加载");
        if (ApiHelper.读条中) return new(false, "读条中");

        if (ApiHelper.GCD剩余 * 1000f < 600f) return new(false, "GCD窗口短");

        if (!ApiHelper.技能已解锁(SmnSkill.内丹)) return new(false, "未解锁");
        if (!ApiHelper.技能可用(SmnSkill.内丹)) return new(false, "冷却中");

        var hp = ApiHelper.玩家血量;
        if (hp > 40f) return new(false, $"血量({hp:F0}%)>40%");

        return new(true, $"血量{hp:F0}%");
    }

    public PAction GetAction() => ApiHelper.自我能力(SmnSkill.内丹);
}
```

### 3.2 注册

```csharp
_offGcdResolvers.Add(new 内丹OffGcd());
```

进游戏 → 脱装备压低血量 → GCD 间隙自动放内丹。

> **oGCD 标准写法：GCD 窗口检查 → 技能可用性 → 触发条件 → 返回 PAction。**

---

## 4. 🏆 里程碑 3：配 QT 开关

**目标**：一键开关某功能。

### 4.1 `Data/SmnQT.cs` — 加常量 + 默认值

```csharp
// 在常量区加：
public const string 自动内丹 = "自动内丹";

// 在 All 字典加一行：
{ 自动内丹, true },

// 在 ModeCategories 加一行：
{ 自动内丹, QTMode.Common },
```

> 模版内置了 5 个 QT（`启用起手` `停手` `AOE` `爆发` `高难模式`），你只需加自己的。

### 4.2 Resolver 里加 QT 检查

```csharp
if (!ApiHelper.获取QT(SmnQT.自动内丹)) return new(false, "QT未开");
```

### 4.3 `UI/SmnQTUI.cs` — 面板加按钮

在对应 Tab 下加：
```csharp
QtToggle(SmnQT.自动内丹, "自动内丹");
```

进游戏 → QT 面板出现「自动内丹」→ 关掉后内丹停发。

> **QT 设计原则**：键名 + 默认值只在 `SmnQT.All` 一处定义，面板 / 宏命令 / 默认值管理全部自动同步。

---

## 5. 🏆 里程碑 4：写一个起手

复制 `Opener/OpenerTemplate.cs` → `Opener/MyOpener.cs`：

```csharp
public class MyOpener : IOpener
{
    public string OpenerName => "Smn_标准起手";

    public void InitializeCountdown(CountDownHandler countdownHandler)
    {
        // 倒计时 3.5s 预放技能
        // countdownHandler.AddAction(3500, O(某技能, ActionTargetType.Self));
    }

    public List<PAction> InCombatSequence
    {
        get
        {
            var s = new List<PAction>();
            s.Add(G(SmnSkill.基础连击1));    // GCD
            s.Add(O(SmnSkill.某能力技));      // 插 oGCD
            s.Add(G(SmnSkill.基础连击2));    // GCD
            return s;
        }
    }

    private static PAction G(uint id) =>
        new(id, ActionType.Gcd, ActionTargetType.Target) { RequiresVerification = true };
    private static PAction O(uint id, ActionTargetType t = ActionTargetType.Target) =>
        new(id, ActionType.OffGcd, t);
}
```

在 `SmnRotation.cs` 的 `Openers` 字典注册：
```csharp
public static IReadOnlyDictionary<string, Type> Openers { get; } = new Dictionary<string, Type>
{
    { "Smn_标准起手", typeof(MyOpener) },
};
```

---

## 6. 扩展速查

| 需求 | 做法 |
|------|------|
| 加 GCD 技能 | 复制 `GcdTemplate.cs` → 写 Check/GetAction → `_gcdResolvers.Add` |
| 加 oGCD 技能 | 复制 `OffGcdTemplate.cs` → 同上 → `_offGcdResolvers.Add` |
| 加 QT 开关 | `SmnQT.cs` 加常量+默认值 → `SmnQTUI.cs` 加按钮 |
| 加设置项 | `SmnSettings.cs` 加字段 → `SmnSettingsUI.cs` 加控件 |
| 加热键按钮 | `SmnHotkeyUI.cs` 的 `Setup()` 里加 `p.AddHotkey(...)` |
| 加另一套起手 | 复制 `OpenerTemplate.cs` → Openers 字典注册 |
| 高难 / 日随分离 | `SmnQT.cs` 的 `ModeCategories` 标 `HighEndOnly` / `DailyOnly` |
| QT 联动（开A关B） | `SmnQT.cs` 的 `CascadeRules` 加规则 |
| 连击链以外的 GCD 选择 | 在 `SMNHelper.CheckHighValueGcd()` 实现等级段逻辑 |
| AoE 技能支持 | 实现 `SMNHelper.GetAoeAction()` / `ShouldUseAoe()` / `FindBestAoeTarget()` |
| 目标免疫检测 | 实现 `SMNHelper.IsTargetImmune()` |

---

## 7. 测试清单

| # | 测试项 | 期望 |
|---|--------|------|
| 1 | `dotnet build` | `Build succeeded` |
| 2 | ACR 加载 | PromeRotation 列表显示 |
| 3 | 基础 GCD | 进战自动打 1-2-3 |
| 4 | oGCD | GCD 间隙自动插能力技 |
| 5 | QT 开关 | 面板可开关功能 |
| 6 | 起手 | 开怪执行起手序列 |
| 7 | 设置面板 | 参数可调 |
| 8 | 聊天命令 | `/Smn list` 列出所有 QT |

---

## 8. 常见问题

| 问题 | 解决 |
|------|------|
| `dotnet build` 找不到 DLL | 检查 `PromeRotationPath` 是否指向正确版本目录 |
| ACR 不加载 | `OutputPath` 最后一级目录名必须和 `RotationMetadata` 的 author 一致 |
| 技能不放但 Debug 显示 Success | 等级不够 / 技能 ID 错 / GCD 窗口不足 |
| `ApiHelper.设置QT` 不生效 | QT key 必须在构造函数中调了 `ApiHelper.添加QT` |
| 新增文件 build 报找不到类型 | 检查 namespace 和类名是否一致 |
| Debug 面板空白 | `UpdateDebugStatus()` 是否遍历了 resolver 列表 |

---

## 9. ApiHelper 速查

全部走 `ApiHelper` 壳（不要直接调 PR 原生 API）：

| 需求 | 调用 |
|------|------|
| 玩家 / 目标 | `ApiHelper.玩家` `ApiHelper.目标` |
| 技能可用 / 冷却(s) / 充能 | `ApiHelper.技能可用(id)` `ApiHelper.技能冷却(id)` `ApiHelper.技能充能(id)` |
| 调整后技能 ID（连击替换） | `ApiHelper.调整技能ID(id)` |
| 上一连击 ID | `ApiHelper.上个连击` |
| 自己的 Buff | `ApiHelper.玩家有状态(buffId)` |
| Buff 剩余时间(s) | `ApiHelper.状态剩余(单位, buffId)` |
| GCD / 连击剩余(s) | `ApiHelper.GCD剩余` `ApiHelper.连击剩余` |
| 是否读条 / 动画锁定 | `ApiHelper.读条中` `ApiHelper.动画锁定中` |
| GCD 后半窗口（安全插 oGCD） | `ApiHelper.GCD后半安全` |
| 血量% | `ApiHelper.玩家血量` `角色.血量()`（扩展方法） |
| 周围敌人数 | `ApiHelper.周围敌人(范围)` |
| 扇形敌人数 | `ApiHelper.扇形敌人(自身, 目标, 半径, 角度)` |
| 最佳 AoE 目标 | `ApiHelper.最佳AoE目标(技能ID, 最少目标数, 角度)` |
| 攻击距离（含扩程修正） | `ApiHelper.攻击距离(基础距离)` |
| QT 读写 | `ApiHelper.获取QT(key)` `ApiHelper.设置QT(key, val)` |
| QT 批量操作 | `ApiHelper.添加QT(key, 默认值)` `ApiHelper.重建QT可见性()` |
| 量谱（全职业已配好） | `ApiHelper.MCH.热量` `ApiHelper.BRD.诗心` 等 |
| 最佳爆发药 | `ApiHelper.最佳爆发药` |
| 防重复施放 | `ApiHelper.最近用过(id, 毫秒)` |
| 屏幕提示 | `ApiHelper.提示(msg, 秒数)` |
| 构建 PAction | `ApiHelper.GCD技能(id)` `ApiHelper.自我能力(id)` |
| 切换目标 | `ApiHelper.切换目标(target)` |
| 技能排程 | `ApiHelper.排入队列(action, 高优先?)` |
| 距离判断 | `对象.距玩家()` `对象.在玩家范围内(r)` `对象.是敌人()` |

---

> **下一步**：需要 Timeline 节点、事件系统、自动减伤等进阶功能？参考 `LccMch` 项目的 `ACR 新手教程：从 Rider 到诗人实战.md`。
