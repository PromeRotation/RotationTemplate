using Init.Data;
using Init.Helper;
using PromeRotation.Rotation;

namespace Init;

/// <summary>
/// 事件回调 + 生命周期管理
///
/// TODO: 实现具体的战斗事件逻辑
/// TODO: 实现模式切换时的 QT 同步逻辑
/// TODO: 实现爆发提示等辅助功能
/// </summary>
public class InitRotationEventHandler : IRotationEventHandler, IRotationLifecycle
{
    // TODO: 添加需要的字段（如上次提示时间等）

    // === IRotationLifecycle ===

    /// <summary>切换到本 ACR 时调用</summary>
    public void OnEnterAcr()
    {
        // TODO: 伪代码
        // 1. 打日志 "[Init] ACR已加载"
        // 2. 注册所有 QT 开关: foreach (var (n, d) in InitQT.All) ApiHelper.添加QT(n, d);
        // 3. 重建 QT 可见性: ApiHelper.重建QT可见性();
        // 4. 恢复当前模式的 QT 默认值快照
        // 5. 初始化宏命令管理器: InitMacroManager.Init();
        // 6. 屏幕提示 "Init ACR 已加载"
    }

    /// <summary>从本 ACR 切走时调用</summary>
    public void OnExitAcr()
    {
        // TODO: 伪代码
        // 1. 退出宏命令管理器: InitMacroManager.Exit();
        // 2. 保存设置: InitSettings.Instance.Save();
        // 3. 屏幕提示 "Init ACR 已卸载"
    }

    // === IRotationEventHandler ===

    /// <summary>每帧调用（无论是否战斗中）</summary>
    public void OnUpdate()
    {
        // TODO: 伪代码
        // 1. 轮询 QT 联动变更: ApiHelper.轮询联动();
        // 2. 同步高难/日随模式切换（如果 PR 面板直接改了 QT 值）
    }

    /// <summary>非战斗状态每帧</summary>
    public void OnOutOfBattleUpdate()
    {
        // TODO: 非战斗逻辑（如自动速行等）
    }

    /// <summary>进入战斗</summary>
    public void OnBattleStarted()
    {
        // TODO: 初始化本场战斗状态
    }

    /// <summary>战斗中每帧</summary>
    public void OnBattleUpdate()
    {
        // TODO: 战斗中逻辑
        // 如: 爆发提示、特定机制检测、自动停手等
    }

    /// <summary>战斗结束</summary>
    public void OnBattleEnded()
    {
        // TODO: 伪代码
        // 1. 如果设置了自动重置: InitBattleData.Instance.Reset();
        // 2. 如果设置了自动重置 QT: InitSettings.Instance.ResetQt();
        // 3. 复位起手标记: ApiHelper.起手已执行 = false;
    }

    /// <summary>切换地图</summary>
    public void OnTerritoryChanged(ushort territoryId)
    {
        // TODO: 重置战斗数据 + 起手标记
        // InitBattleData.Instance.Reset();
        // ApiHelper.起手已执行 = false;
    }

    /// <summary>无目标时</summary>
    public void OnNoTarget()
    {
        // TODO: 无目标时的处理（通常留空）
    }
}
