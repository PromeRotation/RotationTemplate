using Init.Action.Gcd;
using Init.Action.OffGcd;
using Init.Data;
using Init.Opener;
using Init.UI;
using ECommons.ExcelServices;
using ECommons.Logging;
using PromeRotation.Data;
using PromeRotation.Resolvers;
using PromeRotation.Rotation;

namespace Init;

/// <summary>
/// ★ 核心协调器 —— ACR 入口类
///
/// TODO: 替换 (uint)Job.XXX 为你的目标职业枚举值
/// TODO: 替换 "Init" 为你的 ACR 名称（与输出目录名一致）
/// TODO: 在构造函数中按优先级注册你的 GCD / oGCD Resolver
/// TODO: 在 Openers 字典中注册你的起手类型
/// </summary>
[RotationMetadata((uint)Job.MCH, "Init", "Init", "1.0.0.0")]  // TODO: 改 Job 枚举和名称
public class InitRotation : IRotation
{
    public string RotationName => "Init";           // TODO: 改为你的 ACR 名
    public uint JobId => (uint)Job.MCH;             // TODO: 改为目标职业

    private readonly IRotationEventHandler _eventHandler = new InitRotationEventHandler();
    public IRotationEventHandler GetEventHandler() => _eventHandler;

    private readonly List<IDecisionResolver> _gcdResolvers = new();
    private readonly List<IDecisionResolver> _offGcdResolvers = new();

    // ============================================================
    // === QT & Opener 注册表 ===
    // ============================================================
    public static IReadOnlyDictionary<string, bool> QtList => InitQT.All;

    public static IReadOnlyDictionary<string, Type> Openers { get; } = new Dictionary<string, Type>
    {
        // TODO: 替换为你的起手类型
        // 格式: { "起手名称", typeof(你的起手类) },
        { "Init_Opener", typeof(OpenerTemplate) },
    };

    // ============================================================
    // === 构造：注册 Resolver（优先级 = 注册顺序，先注册先判定）===
    // ============================================================
    public InitRotation()
    {
        // TODO: 按优先级从高到低注册 GCD Resolver（参考 Action/Gcd/GcdTemplate.cs）
        // _gcdResolvers.Add(new GcdTemplate());
        // _gcdResolvers.Add(new 另一个Gcd());  // 兜底连击

        // TODO: 按优先级从高到低注册 oGCD Resolver（参考 Action/OffGcd/OffGcdTemplate.cs）
        // _offGcdResolvers.Add(new OffGcdTemplate());
        // _offGcdResolvers.Add(new 另一个OffGcd());

        // 注册所有 QT 开关（从 InitQT.All 唯一数据源）
        foreach (var (n, d) in QtList) ApiHelper.添加QT(n, d);

        // TODO: 恢复当前模式的 QT 默认值快照
        // InitSettings.Instance.RestoreQtSnapshot(InitSettings.Instance.IsHighEnd);

        // TODO: 设置热键面板
        // InitHotkeyUI.Setup();
    }

    // ============================================================
    // === 起手调度 ===
    // ============================================================
    public IOpener? GetOpener()
    {
        // TODO: 根据 QT 开关 / 时间轴选择 / 等级 返回合适的起手
        // 伪代码:
        // if (!ApiHelper.获取QT(InitQT.启用起手)) return null;
        // var name = ApiHelper.时间轴起手名称 ?? ApiHelper.元数据起手名称;
        // if (有指定起手名) return 反射创建对应起手;
        // return InitOpenerManager.GetOpener();  // 按等级+设置自动选
        return null;
    }

    // ============================================================
    // === 循环出口 ===
    // ============================================================
    public PAction? NextAlways() => null;

    public PAction? NextGcd()
    {
        // 遍历 GCD resolver 列表，返回第一个 Check().Success 的
        foreach (var r in _gcdResolvers)
            if (r.Check().Success)
                return r.GetAction();
        return null;
    }

    public PAction? NextOffGcd()
    {
        // 遍历 oGCD resolver 列表，返回第一个 Check().Success 的
        foreach (var r in _offGcdResolvers)
            if (r.Check().Success)
                return r.GetAction();
        return null;
    }

    // ============================================================
    // === 调试状态 ===
    // ============================================================
    public void UpdateDebugStatus()
    {
        ApiHelper.清除GCD调试状态();
        ApiHelper.清除oGCD调试状态();
        foreach (var r in _gcdResolvers)
        {
            var x = r.Check();
            ApiHelper.添加GCD调试状态(r.GetType().Name, x.Success, x.Message);
        }
        foreach (var r in _offGcdResolvers)
        {
            var x = r.Check();
            ApiHelper.添加oGCD调试状态(r.GetType().Name, x.Success, x.Message);
        }
    }

    // ============================================================
    // === UI 委托 ===
    // ============================================================
    public void DrawQTs() => InitQTUI.Draw();
    public void DrawSettings() => InitSettingsUI.Draw();
}
