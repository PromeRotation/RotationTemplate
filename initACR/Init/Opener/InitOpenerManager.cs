using Init.Data;
using PromeRotation.Rotation;

namespace Init.Opener;

/// <summary>
/// 起手调度管理器 — 根据等级 + 设置自动选择起手
///
/// TODO: 根据你的起手列表实现调度逻辑
/// </summary>
public static class InitOpenerManager
{
    /// <summary>
    /// 根据当前玩家等级和设置选择起手
    /// </summary>
    public static IOpener? GetOpener()
    {
        // TODO: 实现起手选择逻辑
        // 伪代码:
        // var me = ApiHelper.玩家;
        // if (!ApiHelper.获取QT(InitQT.启用起手)) return null;
        // if (me == null) return null;
        // var lv = me.Level;
        // if (!InitSettings.Instance.IsHighEnd) return null;  // 日随模式不使用起手
        //
        // // 按等级段选择:
        // if (lv >= 100) return InitSettings.Instance.Opener switch
        // {
        //     0 => new OpenerTemplate(),
        //     _ => new OpenerTemplate(),
        // };
        // if (lv >= 90) return new OpenerTemplate();  // 降级起手
        //
        // return null;  // 等级不够，无起手

        return null;
    }
}
