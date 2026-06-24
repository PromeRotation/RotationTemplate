using Init.Data;
using PromeRotation.Data;
using PromeRotation.UI.HotKey;

namespace Init.UI;

/// <summary>
/// 热键面板 — 在 PR 界面中显示可点击的技能按钮
///
/// TODO: 根据你的职业替换热键列表
/// </summary>
public static class InitHotkeyUI
{
    /// <summary>在 ACR 构造函数中调用 Setup()</summary>
    public static void Setup()
    {
        var p = new HotkeyPanel(columns: 3);  // TODO: 调整列数

        // TODO: 添加你的职业技能热键
        // 示例:
        // p.AddHotkey("爆发药", new PAction(InitSkill.爆发药, ActionType.Item, ActionTargetType.Self));
        // p.AddHotkey("冲刺",   new PAction(3, ActionType.OffGcd, ActionTargetType.Self));
        // p.AddHotkey("内丹",   new PAction(InitSkill.内丹, ActionType.OffGcd, ActionTargetType.Self));
        // p.AddHotkey("防击退", new PAction(7548, ActionType.OffGcd, ActionTargetType.Self));
        // p.AddHotkey("策动",   new PAction(InitSkill.策动, ActionType.OffGcd, ActionTargetType.Self));
        // p.AddHotkey("速行",   new PAction(InitSkill.速行, ActionType.OffGcd, ActionTargetType.Self));

        HotkeyManager.Instance.AddHotkeyPanel(p);
    }
}
