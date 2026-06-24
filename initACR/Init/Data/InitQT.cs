namespace Init.Data;

/// <summary>QT 模式归属：通用 / 高难专属 / 日随专属</summary>
public enum QTMode { Common, HighEndOnly, DailyOnly }

/// <summary>
/// ★ QT 开关 — 唯一数据源
///
/// 设计原则:
///   1. 所有 QT 键名 + 默认值 只在此定义
///   2. 新增 QT 只需加常量 + 在 All 字典加一行
///   3. UI / 宏命令 / 默认值管理全部自动同步
///
/// TODO: 根据你的职业需求替换 QT 开关
/// </summary>
public class InitQT
{
    // === TODO: 键名常量（替换为你的 QT 名称） ===
    public const string 启用起手 = "启用起手";
    public const string 停手 = "停手";
    public const string AOE = "AOE";
    public const string 爆发 = "爆发";
    public const string 高难模式 = "高难模式";

    // TODO: 添加更多 QT 开关常量
    // public const string 某技能开关 = "某技能开关";
    // public const string 自动减伤 = "自动减伤";
    // public const string 爆发药 = "爆发药";
    // public const string 攒资源 = "攒资源";

    // === 唯一数据源：键名 → 默认值 ===
    /// <summary>
    /// 所有 QT 开关及其默认值。
    /// 新增 QT: 在此加一行即可，UI/宏/默认值管理自动同步。
    /// </summary>
    public static readonly IReadOnlyDictionary<string, bool> All = new Dictionary<string, bool>
    {
        { 启用起手, true },
        { 停手, false },
        { AOE, false },
        { 爆发, true },
        { 高难模式, true },
        // TODO: 添加你的 QT 开关
        // { 某技能开关, true },
    };

    /// <summary>获取指定 key 的默认值（不存在返回 false）</summary>
    public static bool Default(string key) => All.TryGetValue(key, out var v) && v;

    /// <summary>是否是元数据 QT（非实际战斗 QT，不参与保存/恢复）</summary>
    public static bool IsMetaKey(string key) => key == 高难模式;

    // === QT 联动表 ===
    /// <summary>
    /// QT 联动规则: 当 trigger 键值变更时，自动同步 links 中的目标键。
    /// invert=true 表示目标值 = !触发值
    ///
    /// TODO: 根据你的需要添加联动规则
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (string key, bool invert)[]> CascadeRules =
        new Dictionary<string, (string, bool)[]>
        {
            // TODO: 示例联动规则（按需修改）
            // { 爆发, new[] { (某子技能1, false), (某子技能2, false) } },
            // { 攒资源, new[] {
            //     (爆发, true), (某技能1, true), (某技能2, true),
            // }},
        };

    // === 模式归属元数据 ===
    /// <summary>
    /// 每个 QT 的模式归属。
    /// Common = 两模式均可见
    /// HighEndOnly = 仅高难模式可见
    /// DailyOnly = 仅日随模式可见
    ///
    /// TODO: 根据你的需要设置每个 QT 的模式归属
    /// </summary>
    public static readonly IReadOnlyDictionary<string, QTMode> ModeCategories = new Dictionary<string, QTMode>
    {
        // 通用（两模式均可见）
        { 启用起手, QTMode.Common },
        { 停手, QTMode.Common },
        { AOE, QTMode.Common },
        { 爆发, QTMode.Common },
        { 高难模式, QTMode.Common },

        // TODO: 高难专属 QT
        // { 爆发药, QTMode.HighEndOnly },

        // TODO: 日随专属 QT
        // { 自动减伤, QTMode.DailyOnly },
    };

    /// <summary>判断指定 QT 在当前模式下是否可见</summary>
    public static bool IsVisibleInMode(string key, bool isHighEnd) =>
        ModeCategories.TryGetValue(key, out var cat) && cat switch
        {
            QTMode.Common => true,
            QTMode.HighEndOnly => isHighEnd,
            QTMode.DailyOnly => !isHighEnd,
            _ => true
        };

    /// <summary>获取指定 QT 的模式归属（不存在返回 Common）</summary>
    public static QTMode GetModeCategory(string key) =>
        ModeCategories.TryGetValue(key, out var cat) ? cat : QTMode.Common;
}
