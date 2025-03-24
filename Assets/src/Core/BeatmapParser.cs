// // 先试试用OsuParser，不行再用这个手写
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;

// public class BeatmapParser
// {
//     public static __Beatmap Parse(string beatmapPath)
//     {
//         // 读取文件
//         string[] lines = File.ReadAllLines(beatmapPath);

//         // 存储不同类型数据的类
//         General general = new General();
//         Metadata metadata = new Metadata();
//         Difficulty difficulty = new Difficulty();
//         List<string> hitObjects = new();

//         string currentSection = null;

//         foreach (var line in lines)
//         {
//             // 忽略空行和注释行
//             if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith(";"))
//                 continue;

//             // 检查是否是新的块
//             if (line.StartsWith("[") && line.EndsWith("]"))
//             {
//                 currentSection = line.Trim('[', ']');
//                 continue;
//             }

//             // 处理不同的块
//             switch (currentSection)
//             {
//                 case "General":
//                     ParseGeneral(line, general);
//                     break;
//                 case "Metadata":
//                     ParseMetadata(line, metadata);
//                     break;
//                 case "Difficulty":
//                     ParseDifficulty(line, difficulty);
//                     break;
//                 case "HitObjects":
//                     hitObjects.Add(line.Trim());
//                     break;
//             }
//         }

//         // 打印解析结果
//         // Console.WriteLine(general);
//         // Console.WriteLine(metadata);
//         // Console.WriteLine(difficulty);
//         // Console.WriteLine("HitObjects:");
//         // foreach (var hitObject in hitObjects)
//         // {
//         //     Console.WriteLine(hitObject);
//         // }
//         return new __Beatmap(general, metadata, difficulty, hitObjects);
//     }

//     static void ParseGeneral(string line, General general)
//     {
//         var parts = line.Split(':', 2);
//         if (parts.Length != 2) return;

//         string key = parts[0].Trim();
//         string value = parts[1].Trim();

//         switch (key)
//         {
//             case "AudioFilename": general.AudioFilename = value; break;
//             case "AudioLeadIn": general.AudioLeadIn = int.Parse(value); break;
//             case "PreviewTime": general.PreviewTime = int.Parse(value); break;
//             case "Countdown": general.Countdown = int.Parse(value); break;
//             case "SampleSet": general.SampleSet = value; break;
//         }
//     }

//     static void ParseMetadata(string line, Metadata metadata)
//     {
//         var parts = line.Split(':', 2);
//         if (parts.Length != 2) return;

//         string key = parts[0].Trim();
//         string value = parts[1].Trim();

//         switch (key)
//         {
//             case "Title": metadata.Title = value; break;
//             case "TitleUnicode": metadata.TitleUnicode = value; break;
//             case "Artist": metadata.Artist = value; break;
//             case "ArtistUnicode": metadata.ArtistUnicode = value; break;
//             case "Creator": metadata.Creator = value; break;
//             case "Version": metadata.Version = value; break;
//             case "Source": metadata.Source = value; break;
//             case "Tags": metadata.Tags = value; break;
//             case "BeatmapID": metadata.BeatmapID = int.Parse(value); break;
//             case "BeatmapSetID": metadata.BeatmapSetID = int.Parse(value); break;
//         }
//     }

//     static void ParseDifficulty(string line, Difficulty difficulty)
//     {
//         var parts = line.Split(':', 2);
//         if (parts.Length != 2) return;

//         string key = parts[0].Trim();
//         string value = parts[1].Trim();

//         switch (key)
//         {
//             case "HPDrainRate": difficulty.HPDrainRate = float.Parse(value); break;
//             case "OverallDifficulty": difficulty.OverallDifficulty = float.Parse(value); break;
//             case "ApproachRate": difficulty.ApproachRate = float.Parse(value); break;
//         }
//     }
// }

// public class General
// {
//     public string AudioFilename { get; set; }
//     public int AudioLeadIn { get; set; }
//     public int PreviewTime { get; set; }
//     public int Countdown { get; set; }
//     public string SampleSet { get; set; }

//     public override string ToString() => $"General: {AudioFilename}, {AudioLeadIn}, {PreviewTime}, {Countdown}, {SampleSet}";
// }

// public class Metadata
// {
//     public string Title { get; set; }
//     public string TitleUnicode { get; set; }
//     public string Artist { get; set; }
//     public string ArtistUnicode { get; set; }
//     public string Creator { get; set; }
//     public string Version { get; set; }
//     public string Source { get; set; }
//     public string Tags { get; set; }
//     public int BeatmapID { get; set; }
//     public int BeatmapSetID { get; set; }

//     public override string ToString() => $"Metadata: {Title}, {TitleUnicode}, {Artist}, {ArtistUnicode}, {Creator}, {Version}, {Source}, {Tags}, {BeatmapID}, {BeatmapSetID}";
// }

// public class Difficulty
// {
//     public float HPDrainRate { get; set; }
//     public float OverallDifficulty { get; set; }
//     public float ApproachRate { get; set; }

//     public override string ToString() => $"Difficulty: {HPDrainRate}, {OverallDifficulty}, {ApproachRate}";
// }

// public enum HitObjectType : int
// {
//     HIT_CIRCLE = 1,
//     SILDER = 2,
//     NEW_COMBO = 4,
//     SPINNER = 8,
//     SKIP_COMBO_COLOR = 112, // 0b1110000
//     MANIA_HOLD = 128
// }

// public enum HitSoundType : int
// {
//     NORMAL = 1,
//     WHISTLE = 2,
//     FINISH = 4,
//     CLAP = 8
// }

// public class HitSample
// {
//     public int normalSet;
//     public int additionSet;
//     public int index;
//     public int volume;
//     public string filename;
// }
// public class HitObject
// {
//     public int x;
//     public int y;
//     public int time;
//     public HitObjectType type; // int
//     public HitSoundType hitSound; // int
//     public string objectParams;
//     public HitSample hitSample; // 最后一个 normalSet:additionSet:index:volume:filename
// }

// public class __Beatmap
// {
//     public General General { get; set; }
//     public Metadata Metadata { get; set; }
//     public Difficulty Difficulty { get; set; }
//     public List<string> HitObjects { get; set; }
//     public __Beatmap(General general, Metadata metadata, Difficulty difficulty, List<string> hitObjects)
//     {
//         General = general;
//         Metadata = metadata;
//         Difficulty = difficulty;
//         HitObjects = hitObjects;
//     }
//     public override string ToString()
//     {
//         return $"General: {General}\nMetadata: {Metadata}\nDifficulty: {Difficulty}\nHitObjects: {string.Join(", ", HitObjects)}";
//     }
// }