using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor (typeof (RandomPoly))]
public class RandomPolyInspector : Editor
{
    public override void OnInspectorGUI()
    {
        RandomPoly rp = target as RandomPoly;

        rp.radius = EditorGUILayout.FloatField("Radius", rp.radius);
        rp.verticesPerUnit = EditorGUILayout.FloatField("Vertex frequency", rp.verticesPerUnit);
        rp.randomRange = EditorGUILayout.FloatField("Angle amplitude", rp.randomRange);
        rp.smoothLevel = EditorGUILayout.IntField("Smooth Level", rp.smoothLevel);

        if (GUILayout.Button("Randomize")) rp.Randomize();
    }
}
