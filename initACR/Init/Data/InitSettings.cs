using System.Text.Json;
using PromeRotation.Helpers;

namespace Init.Data;

/// <summary>
/// 运行期设置（单例，支持 JSON 持久化）
/// 开关类配置走 QT；数值类配置走此 Settings
///
/// TODO: 根据你的职业需求替换设置项
/// </summary>
public class InitSettings
{
    private static InitSettings? _instance;
    public static InitSettings Instance => _instance ??= Load();

    // ============================================================
    // TODO: 数值类设置（替换为你的职业需要的设置）
    // ============================================================

    /// <summary>GCD 容忍度（毫秒），预判窗口用</summary>
    public int Cdtolerance = 250;

    /// <summary>停手持续时长（毫秒），到时自动解除</summary>
    public int HoldTime = 3000;

    /// <summary>高难模式 / 日随模式</summary>
    public bool IsHighEnd = true;

    // TODO: 添加更多设置项
    // public int MinGauge = 50;           // 最低资源阈值
    // public bool AutoDefense = true;     // 自动减伤
    // public float DefenseThreshold = 40f;// 减伤触发血量阈值(%)
    // public int GrabItLimit = 300;       // 抢开阈值（毫秒）
    // public bool UsePotionInOpener = false; // 起手爆发药
    // public int autoPotion = 10;         // 自动爆发药（CD<N秒时）
    // public bool AutoResetBattleData = true;

    // ============================================================
    // === QT 默认值持久化（按模式） ===
    // ============================================================

    /// <summary>用户自定义的 QT 默认值（通用回退）</summary>
    public Dictionary<string, bool> QtDefaultValues = new(InitQT.All);

    /// <summary>高难模式专用 QT 默认值</summary>
    public Dictionary<string, bool> QtHighEndDefaults = new();

    /// <summary>日随模式专用 QT 默认值</summary>
    public Dictionary<string, bool> QtDailyDefaults = new();

    // ============================================================
    // === 方法 ===
    // ============================================================

    public Dictionary<string, bool> GetCurrentModeDefaults()
        => IsHighEnd ? QtHighEndDefaults : QtDailyDefaults;

    /// <summary>保存当前 QT 状态到当前模式快照</summary>
    public void SaveQtSnapshot(bool toHighEnd)
    {
        var dict = toHighEnd ? QtHighEndDefaults : QtDailyDefaults;
        foreach (var key in InitQT.All.Keys)
        {
            if (InitQT.IsMetaKey(key)) continue;
            dict[key] = ApiHelper.获取QT(key);
        }
    }

    /// <summary>从指定模式快照恢复 QT 状态</summary>
    public void RestoreQtSnapshot(bool fromHighEnd)
    {
        var dict = fromHighEnd ? QtHighEndDefaults : QtDailyDefaults;
        if (dict.Count == 0)
        {
            foreach (var key in InitQT.All.Keys)
            {
                if (InitQT.IsMetaKey(key)) continue;
                ApiHelper.设置QT(key, InitQT.Default(key));
            }
        }
        else
        {
            foreach (var key in InitQT.All.Keys)
            {
                if (InitQT.IsMetaKey(key)) continue;
                ApiHelper.设置QT(key, dict.TryGetValue(key, out var v) ? v : InitQT.Default(key));
            }
        }
    }

    /// <summary>重置所有 QT 到默认值</summary>
    public void ResetQt()
    {
        foreach (var key in InitQT.All.Keys)
        {
            if (InitQT.IsMetaKey(key)) continue;
            ApiHelper.设置QT(key, InitQT.Default(key));
        }
    }

    /// <summary>模式切换</summary>
    public void SwitchMode(bool toHighEnd)
    {
        if (IsHighEnd == toHighEnd) return;
        SaveQtSnapshot(IsHighEnd);
        IsHighEnd = toHighEnd;
        ApiHelper.设置QT(InitQT.高难模式, toHighEnd);
        RestoreQtSnapshot(toHighEnd);
        Save();
    }

    /// <summary>复制当前模式默认值到另一模式</summary>
    public void CopyDefaultsToOtherMode()
    {
        var src = GetCurrentModeDefaults();
        var dst = IsHighEnd ? QtDailyDefaults : QtHighEndDefaults;
        if (src.Count == 0) return;
        foreach (var kv in src)
        {
            if (InitQT.IsMetaKey(kv.Key)) continue;
            dst[kv.Key] = kv.Value;
        }
        Save();
    }

    // ============================================================
    // === JSON 持久化 ===
    // ============================================================
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true,
    };

    public static string FilePath
    {
        get
        {
            try
            {
                var root = CachePathHelper.EnsureAcrCacheRoot();
                return System.IO.Path.Combine(root, "Init.Settings.json");  // TODO: 改名为你的 ACR 名
            }
            catch
            {
                return System.IO.Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
                    "XIVLauncherCN", "pluginConfigs", "PromeRotation", "Init.Settings.json");
            }
        }
    }

    public static InitSettings Load()
    {
        try
        {
            if (System.IO.File.Exists(FilePath))
            {
                var json = System.IO.File.ReadAllText(FilePath);
                var s = JsonSerializer.Deserialize<InitSettings>(json, JsonOptions);
                if (s != null)
                {
                    ApiHelper.设置QT(InitQT.高难模式, s.IsHighEnd);
                    return s;
                }
            }
        }
        catch (Exception e) { ECommons.Logging.PluginLog.Error($"[Init] 设置加载失败: {e.Message}"); }
        return new InitSettings();
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(this, JsonOptions);
            var dir = System.IO.Path.GetDirectoryName(FilePath);
            if (dir != null && !System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            System.IO.File.WriteAllText(FilePath, json);
        }
        catch { /* 写失败静默 */ }
    }
}
