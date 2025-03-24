using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class BeatEntity
{
    public int type;
    public int start_time;
    public int x, y; // osu pixel
    public string extra;
    public virtual string Dump()
    {
        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            // Unity 的一些属性无法序列化时，可设置如下避免报错：
            Error = (sender, args) =>
            {
                Debug.LogWarning("Json Serialization Warning: " + args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            },
            // Formatting = Formatting.Indented
        };
        return JsonConvert.SerializeObject(this, settings);
    }
}

[System.Serializable]
public class E_Catch : BeatEntity
{
    // public Color color = Color.green;
}

[System.Serializable]
public class E_ALinear : BeatEntity
{
    public int end_time;
    public int end_x, end_y;
}

[System.Serializable]
public class SMap
{
    public string filePath;
    // General
    public string audioFile;
    public float bpm = 130;
    // BeatObjects
    public List<BeatEntity> beatObjects = new();
    internal enum FileSections
    {
        None,
        General,
        BeatObjects,
    }
    static FileSections GetCurrentSection(string line)
    {
        FileSections parsedSection = FileSections.None;
        Enum.TryParse(line.Trim(new char[] { '[', ']' }), true, out parsedSection);
        return parsedSection;
    }

    public static List<string> BaseListFormat(string SectionName)
    {
        return new List<string>
            {
                string.Empty,
                $"[{SectionName}]",
            };
    }
    public List<string> Dump()
    {
        List<string> ret = new();
        BaseListFormat("General").ForEach(x => ret.Add(x));
        ret.AddRange(new List<string>
        {
            "AudioFilename: " + audioFile,
            "Bpm: " + bpm,
        });

        BaseListFormat("BeatObjects").ForEach(x => ret.Add(x));
        foreach (var b in beatObjects)
        {
            ret.Add(b.Dump());
        }


        return ret;
    }

    public void Dump2File() => Dump2File(filePath);
    public void Dump2File(string path)
    {
        File.WriteAllText(path, string.Join("\n", Dump()));
        Debug.Log($"ShapeMap Dumped to {path}");
    }

    public void LoadFromFile()
    {
        beatObjects.Clear();
        var currentSection = FileSections.None;
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
            {
                if (GetCurrentSection(line) != FileSections.None)
                    currentSection = GetCurrentSection(line);
                else if (IsLineValid(line, currentSection))
                {
                    switch (currentSection)
                    {
                        case FileSections.General:
                            int index = line.IndexOf(':');
                            string variable = line.Remove(index).Trim();
                            string value = line.Remove(0, index + 1).Trim();
                            switch (variable)
                            {
                                case "AudioFilename":
                                    audioFile = value;
                                    break;
                                case "Bpm":
                                    bpm = Convert.ToUInt32(value);
                                    break;
                            }
                            break;
                        case FileSections.BeatObjects:
                            BeatEntity bobase = JsonConvert.DeserializeObject<BeatEntity>(line);
                            Type targetType = HitViewBase.botypelut[bobase.type];
                            bobase = JsonConvert.DeserializeObject(line, targetType) as BeatEntity;
                            beatObjects.Add(bobase);
                            break;
                    }
                }
            }
        }
    }

    static bool IsLineValid(string line, FileSections currentSection)
    {
        switch (currentSection)
        {
            case FileSections.General:
                return line.Contains(":");
            case FileSections.BeatObjects:
                return line.Contains(",");
            default: return false;
        }
    }

}
