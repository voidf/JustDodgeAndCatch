
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData // 目前只有一个记上一次游玩关卡的需求
{
    public string lastStageSceneName = ""; // 用这个还是用int存好点？
}

[System.Serializable]
public class GameConfig
{
    public int judgementOffset = 0; // 毫秒级的判定偏移量
    public KeyBindingPreset keyBindingPreset = KeyBindingPreset.Preset1; // 按键绑定预设

    // 游戏配置
    public bool isUseDownbeat = false;
    public bool isReverseBeatSE = false;

    // 音量配置项
    public float musicVolume = .6f; // 音乐音量
    public float effectVolume = 1f; // 音效音量
    // 其他配置项
    public int frameLimit = 0; // 帧率限制，为0时表示不限制
    public bool isFullScreen = false; // 是否全屏
    public int resolutionConfigIndex = 0; // 分辨率配置索引
}

public enum KeyBindingPreset
{
    Preset1,
    Preset2,
    Preset3
}

[System.Serializable]
public class StageData
{
    public int highestScore = 0;
    public float bestTime = Mathf.Infinity;
}

public static class JsonConfigManager
{
    // %userprofile%\AppData\LocalLow\<companyname>\<productname>
    // C:\Users\Administrator\AppData\LocalLow\DefaultCompany\My project
    #region 配置、首选项
    private static string configFilePath = Path.Combine(Application.persistentDataPath, "gameConfig.json");

    public static void SaveConfig(GameConfig config)
    {
        string json = JsonUtility.ToJson(config);
        File.WriteAllText(configFilePath, json);
    }

    public static GameConfig LoadConfig()
    {
        if (File.Exists(configFilePath))
        {
            string json = File.ReadAllText(configFilePath);
            return JsonUtility.FromJson<GameConfig>(json);
        }
        else
        {
            return new GameConfig(); // 或者返回默认配置
        }
    }
    #endregion
    #region 关卡记录
    static string GetStageDataFilePath(string stageId) => Path.Combine(Application.persistentDataPath, $"stageData_{stageId}.json");

    public static void SaveStageData(StageData data, string stageId)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(GetStageDataFilePath(stageId), json);
    }

    public static StageData LoadStageData(string stageId)
    {
        var filePath = GetStageDataFilePath(stageId);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<StageData>(json);
        }
        else
        {
            return new StageData(); // 或者返回默认配置
        }
    }
    #endregion
    #region 存档
    private static string saveDataFilePath = Path.Combine(Application.persistentDataPath, "saveData.json");

    public static void SaveSaveData(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveDataFilePath, json);
    }
    public static SaveData LoadSaveData()
    {
        if (File.Exists(saveDataFilePath))
        {
            string json = File.ReadAllText(saveDataFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return new SaveData(); // 或者返回默认配置
        }
    }
    #endregion
}

public static class DiskLogger
{
    private static string logFilePath = Path.Combine(Application.persistentDataPath, "log.txt");

    public static void Log(string log)
    {
        File.AppendAllText(logFilePath, log + "\n");
    }
}
