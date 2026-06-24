using System.Numerics;
using System.Timers;
using Init.Data;
using Dalamud.Bindings.ImGui;
using PromeRotation.Data;
using Timer = System.Timers.Timer;

namespace Init.UI;

/// <summary>
/// QT 面板 UI — 绘制 QT 开关界面
///
/// TODO: 根据你的 QT 开关调整界面布局
/// </summary>
public static class InitQTUI
{
    private static Timer? _holdTimer;

    // ============================================================
    // === 主入口 ===
    // ============================================================
    public static void Draw()
    {
        // 渲染前先同步 QT 联动
        PollCascade();

        if (!ImGui.BeginTabBar("Init_QT")) return;

        // === Tab 1: 基础 ===
        if (ImGui.BeginTabItem("基础"))
        {
            ModeToggle();
            ImGui.Separator();

            QtToggle(InitQT.启用起手, "启用起手");
            QtToggle(InitQT.停手, "停手", OnHoldChanged);
            QtToggle(InitQT.AOE, "AOE 模式");
            // TODO: 添加你的 QT 开关
            // QtToggle(InitQT.某开关, "显示名");

            ImGui.EndTabItem();
        }

        // === Tab 2: 技能 (TODO: 添加技能开关) ===
        if (ImGui.BeginTabItem("技能"))
        {
            // TODO: 添加技能相关 QT
            // QtToggle(InitQT.某技能开关, "某技能", requiresSkillId: InitSkill.某技能);

            ImGui.EndTabItem();
        }

        // === Tab 3: 资源 (TODO: 添加资源管理开关) ===
        if (ImGui.BeginTabItem("资源"))
        {
            QtToggle(InitQT.爆发, "爆发");
            // TODO: 添加资源相关 QT
            // QtToggle(InitQT.某资源开关, "某资源");

            ImGui.EndTabItem();
        }

        ImGui.EndTabBar();

        // 渲染后：联动 + 模式同步
        PollCascade();
        SyncModeFromQt();
    }

    public static void PollCascade() => ApiHelper.轮询联动();

    /// <summary>直接对比 QuickToggles 和 IsHighEnd，不一致就同步</summary>
    private static void SyncModeFromQt()
    {
        var qt = PromeSettings.Instance.QuickToggles;
        if (!qt.TryGetValue(InitQT.高难模式, out var modeQt)) return;
        if (modeQt == InitSettings.Instance.IsHighEnd) return;
        InitSettings.Instance.IsHighEnd = modeQt;
        RebuildQtVisibility();
        ApiHelper.提示($"→ {(modeQt ? "高难" : "日随")}", 2);
    }

    /// <summary>模式切换后重建 QT 可见性</summary>
    public static void RebuildQtVisibility() => ApiHelper.重建QT可见性();

    // ============================================================
    // === 绘制辅助 ===
    // ============================================================
    /// <summary>模式感知的 QT 开关</summary>
    public static void QtToggle(string key, string label, Action<bool>? onChanged = null, uint requiresSkillId = 0)
    {
        if (requiresSkillId != 0 && !ApiHelper.技能已解锁(requiresSkillId)) return;
        if (!InitQT.IsVisibleInMode(key, InitSettings.Instance.IsHighEnd)) return;

        var v = ApiHelper.获取QT(key);
        var cat = InitQT.GetModeCategory(key);

        if (cat == QTMode.HighEndOnly)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.8f, 0.2f, 1f));
        else if (cat == QTMode.DailyOnly)
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.2f, 1f, 0.4f, 1f));

        var display = v ? $"☑ {label}" : $"☐ {label}";
        if (ImGui.Selectable($"{display}##{key}"))
        {
            v = !v;
            var qt = PromeSettings.Instance.QuickToggles;
            qt[key] = v;
            if (InitQT.CascadeRules.TryGetValue(key, out var links))
                foreach ((string lk, bool inv) in links)
                    qt[lk] = inv ? !v : v;
            onChanged?.Invoke(v);
        }

        if (cat != QTMode.Common)
            ImGui.PopStyleColor();
    }

    // ============================================================
    // === 模式切换 ===
    // ============================================================
    private static void ModeToggle()
    {
        var v = InitSettings.Instance.IsHighEnd;
        ImGui.PushStyleColor(ImGuiCol.Text, v
            ? new Vector4(1f, 0.8f, 0.2f, 1f)
            : new Vector4(0.2f, 1f, 0.4f, 1f));
        var display = v ? "☑ 高难模式" : "☐ 日随模式";
        if (ImGui.Selectable($"{display}##ModeSwitch"))
            OnModeSwitchChanged(!v);
        ImGui.PopStyleColor();
    }

    private static void OnModeSwitchChanged(bool isHighEnd)
    {
        ApiHelper.提示($"切换至{(isHighEnd ? "高难" : "日随")}模式", 2);
        InitSettings.Instance.SwitchMode(isHighEnd);
    }

    // ============================================================
    // === QT 联动回调 ===
    // ============================================================

    /// <summary>停手计时器回调</summary>
    public static void OnHoldChanged(bool isSet)
    {
        if (!isSet) return;
        _holdTimer?.Stop(); _holdTimer?.Dispose();
        _holdTimer = new Timer(InitSettings.Instance.HoldTime) { AutoReset = false };
        _holdTimer.Elapsed += (_, _) =>
        {
            ApiHelper.设置QT(InitQT.停手, false);
            _holdTimer?.Dispose(); _holdTimer = null;
        };
        _holdTimer.Start();
    }
}
