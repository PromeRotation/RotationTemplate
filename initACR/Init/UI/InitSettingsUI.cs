using System.Numerics;
using Init.Data;
using Init.Helper;
using Dalamud.Bindings.ImGui;
using PromeRotation.Data;

namespace Init.UI;

/// <summary>
/// 设置面板 UI
///
/// TODO: 根据你的职业设置项调整界面
/// </summary>
public static class InitSettingsUI
{
    public static void Draw()
    {
        if (!ImGui.BeginTabBar("Settings##Init")) return;

        if (ImGui.BeginTabItem("通用设置"))   { DrawGeneral(); ImGui.EndTabItem(); }
        if (ImGui.BeginTabItem("QT管理"))     { DrawQtManage(); ImGui.EndTabItem(); }
        if (ImGui.BeginTabItem("默认值管理")) { DrawDefaultsManage(); ImGui.EndTabItem(); }
        if (ImGui.BeginTabItem("开发用"))     { DrawDev(); ImGui.EndTabItem(); }

        ImGui.EndTabBar();
        InitSettings.Instance.Save();

        // 命令窗口
        var cmd = InitSettings.Instance.CommandWindowOpen;
        InitMacroManager.DrawCommandWindow(ref cmd);
        InitSettings.Instance.CommandWindowOpen = cmd;
    }

    // ============================================================
    private static void DrawGeneral()
    {
        var h = InitSettings.Instance.IsHighEnd;
        Hdr("⚙ 基础设置");
        ImGui.Text($"当前模式：{(h ? "高难" : "日随")}");
        ImGui.SameLine();
        if (ImGui.SmallButton(h ? "切换日随" : "切换高难"))
            InitSettings.Instance.SwitchMode(!h);

        ImGui.Separator();

        // TODO: 添加你的设置滑块/复选框
        // 示例:
        // var cd = InitSettings.Instance.Cdtolerance;
        // if (ImGui.SliderInt("CD容忍度(ms)", ref cd, 200, 1600)) InitSettings.Instance.Cdtolerance = cd;
        // var hs = InitSettings.Instance.HoldTime / 1000;
        // if (ImGui.SliderInt("停手时长(秒)", ref hs, 1, 10)) InitSettings.Instance.HoldTime = hs * 1000;

        ImGui.Separator();
    }

    // ============================================================
    private static void DrawQtManage()
    {
        var isHighEnd = InitSettings.Instance.IsHighEnd;
        var modeColor = isHighEnd
            ? new Vector4(1f, 0.8f, 0.2f, 1f)
            : new Vector4(0.2f, 1f, 0.4f, 1f);

        Hdr("🎮 QT 开关管理");
        ImGui.TextColored(modeColor, $"当前模式：{(isHighEnd ? "高难" : "日随")}");
        ImGui.SameLine();
        if (ImGui.SmallButton(isHighEnd ? "切换日随" : "切换高难"))
            InitSettings.Instance.SwitchMode(!isHighEnd);

        ImGui.Separator();

        // 通用 QT
        if (ImGui.CollapsingHeader("🔵 通用 QT", ImGuiTreeNodeFlags.DefaultOpen))
            DrawQtGroup(QTMode.Common, isHighEnd);

        // 高难专属 QT
        if (ImGui.CollapsingHeader("🟠 高难专属 QT"))
            DrawQtGroup(QTMode.HighEndOnly, isHighEnd);

        // 日随专属 QT
        if (ImGui.CollapsingHeader("🟢 日随专属 QT"))
            DrawQtGroup(QTMode.DailyOnly, isHighEnd);

        ImGui.Separator();
        Hdr("⌨ 聊天命令");
        ImGui.TextWrapped("使用 /Init <QT名称> 在聊天框切换QT。");  // TODO: 改命令名
        var co = InitSettings.Instance.CommandWindowOpen;
        if (ImGui.Button("📋 打开命令列表")) co = true;
        InitSettings.Instance.CommandWindowOpen = co;
    }

    private static void DrawQtGroup(QTMode category, bool isHighEnd)
    {
        foreach (var (key, _) in InitQT.All)
        {
            if (InitQT.IsMetaKey(key)) continue;
            if (InitQT.GetModeCategory(key) != category) continue;

            var visible = InitQT.IsVisibleInMode(key, isHighEnd);
            var v = ApiHelper.获取QT(key);

            if (!visible)
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.4f, 0.4f, 0.4f, 0.6f));

            if (ImGui.Checkbox($"{key}##qtmgr_{key}", ref v))
            {
                if (visible) ApiHelper.设置QT(key, v);
            }

            if (!visible)
                ImGui.PopStyleColor();
        }
    }

    private static void DrawDefaultsManage()
    {
        Hdr("📦 默认值管理");
        ImGui.TextWrapped("按模式分别管理 QT 开关的默认值。切换模式时自动保存/恢复。");

        ImGui.Separator();
        ImGui.Columns(2, "DefaultsCols", true);

        // 高难默认值
        Hdr("🟠 高难默认值", new Vector4(1f, 0.8f, 0.2f, 1f));
        foreach (var (key, _) in InitQT.All)
        {
            if (InitQT.IsMetaKey(key)) continue;
            var cat = InitQT.GetModeCategory(key);
            if (cat == QTMode.DailyOnly) continue;

            var dict = InitSettings.Instance.QtHighEndDefaults;
            var v = dict.TryGetValue(key, out var dv) ? dv : InitQT.Default(key);
            if (ImGui.Checkbox($"{key}##hdef_{key}", ref v))
                dict[key] = v;
        }
        if (ImGui.Button("💾 保存当前QT##hdef_save"))
        {
            InitSettings.Instance.SaveQtSnapshot(true);
            InitSettings.Instance.Save();
        }

        ImGui.NextColumn();

        // 日随默认值
        Hdr("🟢 日随默认值", new Vector4(0.2f, 1f, 0.4f, 1f));
        foreach (var (key, _) in InitQT.All)
        {
            if (InitQT.IsMetaKey(key)) continue;
            var cat = InitQT.GetModeCategory(key);
            if (cat == QTMode.HighEndOnly) continue;

            var dict = InitSettings.Instance.QtDailyDefaults;
            var v = dict.TryGetValue(key, out var dv) ? dv : InitQT.Default(key);
            if (ImGui.Checkbox($"{key}##ddef_{key}", ref v))
                dict[key] = v;
        }
        if (ImGui.Button("💾 保存当前QT##ddef_save"))
        {
            InitSettings.Instance.SaveQtSnapshot(false);
            InitSettings.Instance.Save();
        }

        ImGui.Columns(1);
        ImGui.Separator();

        if (ImGui.Button("📋 复制当前模式默认值到另一模式"))
        {
            InitSettings.Instance.CopyDefaultsToOtherMode();
            ApiHelper.提示("已复制默认值到另一模式", 2);
        }
    }

    private static void DrawDev()
    {
        Hdr("🔧 开发诊断");
        ImGui.Text($"配置路径: {InitSettings.FilePath}");
        ImGui.Separator();

        var p = ApiHelper.玩家;
        if (p == null) { ImGui.Text("玩家未加载"); return; }
        ImGui.Text($"等级:{p.Level}  职业:{p.ClassJob.RowId}");
        ImGui.Text($"GCD剩余:{ApiHelper.GCD剩余:F2}s  读条中:{ApiHelper.读条中}");
        ImGui.Text($"模式:{(InitSettings.Instance.IsHighEnd ? "高难" : "日随")}");
        ImGui.Separator();

        // TODO: 添加更多诊断信息
        // ImGui.Text($"某技能CD:{ApiHelper.技能冷却(InitSkill.某技能):F1}s");
        // ImGui.Text($"某Buff:{ApiHelper.玩家有状态(InitBuff.某Buff)}");
    }

    // ============================================================
    private static void Hdr(string t, Vector4? c = null) => ImGui.TextColored(c ?? new(0.2f, 0.8f, 1f, 1f), t);
}
