using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(WordlGenerator))]
public class WorldGeneratorInspector : Editor
{
    private Vector2Int size;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WordlGenerator wg = target as WordlGenerator;
        size = EditorGUILayout.Vector2IntField("Select cell", size);
        if (GUILayout.Button ("Generate cell"))
        {
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    wg.GenerateCell(x, y);
        }
    }
}
