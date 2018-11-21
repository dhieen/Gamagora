using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorldEditor : EditorWindow
{
    private GameObject blocPrefab;
    private float radiusParam;
    private float freqParam;
    private float angleParam;

    [MenuItem("SD2EM/World Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WorldEditor));
    }

    void OnGUI()
    {
        blocPrefab = EditorGUILayout.ObjectField("Bloc prefab", blocPrefab, typeof(GameObject), false) as GameObject;
        radiusParam = EditorGUILayout.FloatField("Radius setting", radiusParam);
        freqParam = EditorGUILayout.FloatField("Frequence setting", freqParam);
        angleParam = EditorGUILayout.FloatField("Angle setting", angleParam);

        if (blocPrefab != null && GUILayout.Button ("New Block"))
        {
            Object newBloc = Instantiate<Object>(blocPrefab);
            RandomPoly poly = new RandomPoly(newBloc as GameObject, radiusParam, freqParam, angleParam);
            poly.Randomize();
        }
    }
}
