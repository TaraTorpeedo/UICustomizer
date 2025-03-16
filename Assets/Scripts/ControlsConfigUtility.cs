using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ControlsConfigUtility
{
    [Serializable]
    public class ButtonData
    {
        public string ButtonName;
        public int ButtonId;
        public Vector2 ButtonPosition;
        public Vector2 ButtonSize;
    }

    [Serializable]
    public class ConfigData
    {
        public string ConfigName;
        public int ConfigId;
        public List<ButtonData> ButtonsData = new List<ButtonData>();
    }

    [Serializable]
    public class ModeData
    {
        public string ModeName;
        public int ModeId;
        public List<ConfigData> ConfigsData = new List<ConfigData>();
    }

    [Serializable]
    public class ControlsConfig
    {
        public List<ModeData> ModesData = new List<ModeData>();
    }

    private static string m_filePath => Path.Combine(Application.streamingAssetsPath, "ControlsConfig.json");

    public static void SaveToJson(ControlsConfig config)
    {
        var json = JsonUtility.ToJson(config, true);
        File.WriteAllText(m_filePath, json);
    }

    public static ControlsConfig LoadFromJson()
    {
        if (!File.Exists(m_filePath))
        {
            return new ControlsConfig();
        }

        var json = File.ReadAllText(m_filePath);
        return JsonUtility.FromJson<ControlsConfig>(json);
    }

    public static ModeData GetCurrentModeData(List<ModeData> modes, int currentModeId) => modes.Find(mode => mode.ModeId == currentModeId);

    public static ConfigData GetCurrentConfigData(List<ConfigData> configs, int currentConfigId) => configs.Find(config => config.ConfigId == currentConfigId);
}