# Init ACR 模版

基于 PromeRotation 框架的 FFXIV 外部 ACR 开发模版。从 [LccMch](https://github.com/kanyeishere/PRACR)（机工士 ACR）提炼而来，保留完整项目架构和中文 API 壳，职业逻辑清空为 TODO 模板。

## 快速开始

```
双击 setup.bat → 输入 ACR 名/作者名/选职业 → 自动完成替换 → dotnet build
```

执行后 `Init/` 目录被重命名为 `{作者名}{ACR名}/`，内部文件全部替换为你的命名。setup.bat 自动删除。

## 模版包含

| 组件 | 说明 |
|------|------|
| `ApiHelper.cs` | 中文 API 壳，600+ 行完整保留（玩家/目标/技能/GCD/小队/全职业量谱/Buff/QT/移动） |
| `{acr}Rotation.cs` | 核心协调器，Resolver 注册 + 循环出口 |
| `{acr}RotationEventHandler.cs` | 事件回调 + 生命周期 |
| `{JOB}Helper.cs` | 职业辅助方法（连击链/AoE/高价值GCD/免疫检测） |
| `Data/` | 技能 ID / Buff ID / QT 数据源 / 设置持久化 / 战斗缓存 |
| `Action/Gcd/GcdTemplate.cs` | GCD Resolver 模板（复制即用） |
| `Action/OffGcd/OffGcdTemplate.cs` | oGCD Resolver 模板 |
| `Opener/OpenerTemplate.cs` | 起手模板 |
| `UI/` | QT 面板 / 设置面板 / 热键面板 |
| `Helper/{acr}MacroManager.cs` | `/` 聊天命令 |

## 依赖

- .NET 10.0 Windows
- [PromeRotation](https://github.com/kanyeishere/PRACR)
- [Dalamud](https://github.com/goatcorp/Dalamud)
- [ECommons](https://github.com/NightmareXIV/ECommons)
- Lumina

## 目录结构

```
{author}{acr}/
├── {acr}.csproj
├── ApiHelper.cs
├── {acr}Rotation.cs
├── {acr}RotationEventHandler.cs
├── {JOB}Helper.cs
├── Data/
│   ├── {acr}Skill.cs
│   ├── {acr}Buff.cs
│   ├── {acr}QT.cs
│   ├── {acr}Settings.cs
│   └── {acr}BattleData.cs
├── Action/
│   ├── Gcd/GcdTemplate.cs
│   └── OffGcd/OffGcdTemplate.cs
├── Opener/
│   ├── OpenerTemplate.cs
│   ├── {acr}OpenerManager.cs
│   └── {acr}OpenerChecker.cs
├── UI/
│   ├── {acr}QTUI.cs
│   ├── {acr}SettingsUI.cs
│   └── {acr}HotkeyUI.cs
├── Helper/
│   └── {acr}MacroManager.cs
└── .claude/skills/{acr}-dev/SKILL.md
```

## 详细教程

参见 [Init 模版使用教程：从零到第一个 ACR](https://github.com/PromeRotation/RotationTemplate/blob/main/initACR/Init%20%E6%A8%A1%E7%89%88%E4%BD%BF%E7%94%A8%E6%95%99%E7%A8%8B%EF%BC%9A%E4%BB%8E%E9%9B%B6%E5%88%B0%E7%AC%AC%E4%B8%80%E4%B8%AA%20ACR.md)
