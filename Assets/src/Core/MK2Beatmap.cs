
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;


// public enum MHitObjectType : int
// {
//     CATCH_BEAT = 0
// }

// public class MHitObject
// {
//     public MHitObjectType type;
//     public int x, y;
//     public int time;
//     public virtual string Dump() => "";
//     public static MHitObject Load(string _line)
//     {
//         string[] tokens = _line.Split(',');
//         MHitObject ho = null;
//         MHitObjectType t = (MHitObjectType)int.Parse(tokens[0]);
//         int x = int.Parse(tokens[1]);
//         int y = int.Parse(tokens[2]);
//         int time = int.Parse(tokens[3]);
//         switch (t)
//         {
//             case MHitObjectType.CATCH_BEAT:
//                 ho = new CatchBeat(x, y, time);
//                 break;
//         }
//         return ho;
//     }
// }

// public class CatchBeat : MHitObject
// {
//     public CatchBeat(int _x, int _y, int _time)
//     {
//         x = _x; y = _y; time = _time; type = MHitObjectType.CATCH_BEAT;
//     }
//     public override string Dump() => $"{(int)type},{x},{y},{time}";
// }

// public class MBeatmap
// {

//     // public List<>
// }
