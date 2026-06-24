using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using ECommons.DalamudServices;
using ECommons.Logging;
using Init.Data;
using PromeRotation.Helpers;

namespace Init.Helper;

/// <summary>
/// 聊天命令管理器
/// 注册 /Init 命令，通过聊天输入控制 QT 开关
///
/// TODO: 修改命令名（如 /Init 改为 /YourAcrName）
/// </summary>
public static class InitMacroManager
{
    private const string CmdHandle = "/Init";  // TODO: 改为你的命令名

    private static readonly Dictionary<string, string> QtKeyMap = new(StringComparer.OrdinalIgnoreCase);
    private static bool _initialized;

    /// <summary>在 OnEnterAcr 中调用</summary>
    public static void Init()
    {
        if (_initialized) return;

        try { Svc.Commands.RemoveHandler(CmdHandle); }
        catch { /* 首次注册 */ }

        Svc.Commands.AddHandler(CmdHandle, new CommandInfo(OnCommand)
        {
            HelpMessage = "Init ACR 命令。用法: /Init <QT键名> — 切换QT; /Init list — 列出所有命令"
        });

        // 构建 QT 键映射（从 InitQT.All 唯一数据源）
        QtKeyMap.Clear();
        foreach (var (key, _) in InitQT.All)
        {
            if (InitQT.IsMetaKey(key)) continue;
            QtKeyMap[key] = key;
        }

        _initialized = true;
    }

    /// <summary>在 OnExitAcr 中调用</summary>
    public static void Exit()
    {
        try { Svc.Commands.RemoveHandler(CmdHandle); }
        catch { /* 可能已移除 */ }
        _initialized = false;
    }

    private static void OnCommand(string command, string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            ApiHelper.提示($"{CmdHandle} <QT名> 切换开关 | {CmdHandle} list 查看列表", 3, HintHelper.HintType.Info);
            return;
        }

        var processed = args.Trim();

        // list — 列出所有命令
        if (processed.Equals("list", StringComparison.OrdinalIgnoreCase))
        {
            var qtList = string.Join(", ", QtKeyMap.Keys);
            ApiHelper.提示($"可用QT: {qtList}", 5, HintHelper.HintType.Info);
            return;
        }

        // 尝试匹配 QT 键
        if (QtKeyMap.TryGetValue(processed, out var qtKey))
        {
            var current = ApiHelper.获取QT(qtKey);
            ApiHelper.设置QT(qtKey, !current);
            ApiHelper.提示($"QT [{qtKey}] → {!current}", 2, HintHelper.HintType.Info);
            return;
        }

        // 特殊命令
        if (processed.Equals("save", StringComparison.OrdinalIgnoreCase))
        {
            InitSettings.Instance.Save();
            ApiHelper.提示("设置已保存", 2, HintHelper.HintType.Info);
            return;
        }
        if (processed.Equals("high", StringComparison.OrdinalIgnoreCase))
        {
            InitSettings.Instance.SwitchMode(true);
            ApiHelper.提示("已切换高难模式", 2, HintHelper.HintType.Info);
            return;
        }
        if (processed.Equals("normal", StringComparison.OrdinalIgnoreCase))
        {
            InitSettings.Instance.SwitchMode(false);
            ApiHelper.提示("已切换日随模式", 2, HintHelper.HintType.Info);
            return;
        }

        ApiHelper.提示($"未知命令: {args}，使用 {CmdHandle} list 查看", 2, HintHelper.HintType.Warning);
    }

    /// <summary>绘制命令帮助窗口（在设置面板中调用）</summary>
    public static void DrawCommandWindow(ref bool open)
    {
        if (!open) return;

        var mainViewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowSize(new Vector2(mainViewport.Size.X / 2f, mainViewport.Size.Y / 1.5f), ImGuiCond.Always);
        ImGui.Begin("Init 宏命令帮助", ref open);

        ImGui.TextWrapped($"通过 {CmdHandle} 使用快捷指令切换 QT。结合游戏内宏可方便手柄用户。");
        ImGui.Separator();

        ImGui.Columns(2, "CmdCols", true);
        ImGui.Text("命令"); ImGui.NextColumn();
        ImGui.Text("说明"); ImGui.NextColumn();
        ImGui.Separator();

        foreach (var (key, _) in QtKeyMap)
        {
            var cmd = $"{CmdHandle} {key}";
            ImGui.Text(key);
            ImGui.NextColumn();
            if (ImGui.Button($"复制##{key}")) ImGui.SetClipboardText(cmd);
            ImGui.SameLine(); ImGui.Text(cmd);
            ImGui.NextColumn();
        }

        DrawCmdRow("list", $"{CmdHandle} list", "列出所有可用命令");
        DrawCmdRow("save", $"{CmdHandle} save", "保存当前设置");
        DrawCmdRow("high", $"{CmdHandle} high", "切换高难模式");
        DrawCmdRow("normal", $"{CmdHandle} normal", "切换日随模式");

        ImGui.Columns(1);
        ImGui.Separator();
        if (ImGui.Button("关闭")) open = false;
        ImGui.End();
    }

    private static void DrawCmdRow(string name, string cmd, string desc)
    {
        ImGui.Text(name); ImGui.NextColumn();
        if (ImGui.Button($"复制##{name}")) ImGui.SetClipboardText(cmd);
        ImGui.SameLine(); ImGui.Text($"{cmd}  — {desc}");
        ImGui.NextColumn();
    }
}
