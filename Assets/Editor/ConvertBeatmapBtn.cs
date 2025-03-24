using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelLoader))]
public class ConvertBeatmapBtn : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Load .osu Beatmap"))
        {
            LevelLoader.InitSongsFromSA();
            (target as LevelLoader).LoadBeatmap();
        }
        if (GUILayout.Button("Clear Beat Views"))
        {
            (target as LevelLoader).RemoveAllBeatView();
        }
        if (GUILayout.Button("Load .ini Shape Map"))
        {
            (target as LevelLoader).LoadShapeMap();
        }
        if (GUILayout.Button("Save Map"))
        {
            (target as LevelLoader).SyncToDisk();
        }
        if (GUILayout.Button("Extend Slider"))
        {
            (target as LevelLoader).ExtendSlider();
        }
    }
}

// [CustomEditor(typeof(HitObjectView), true)]
// public class HitObjectViewEditor : Editor
// {
//     SerializedProperty serializedObjectProp;

//     void OnEnable()
//     {
        
//         serializedObjectProp = serializedObject.FindProperty("seo");
//         var x = serializedObject.FindProperty("entity");
//         var y = serializedObject.FindProperty("bo");
//         Debug.Log($"{x} {x==null} {y} {y==null} {serializedObject.context} {serializedObjectProp==null}");
//     }

//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//         Debug.Log($"{serializedObject} serializedObjectProp {serializedObjectProp==null}");

//         EditorGUILayout.PropertyField(serializedObjectProp);

//         serializedObject.ApplyModifiedProperties();
//     }
// }