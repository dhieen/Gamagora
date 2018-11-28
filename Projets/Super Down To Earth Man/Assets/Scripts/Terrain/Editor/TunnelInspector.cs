using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tunnel))]
public class TunnelInspector : Editor
{
    private Tunnel otherRT;
    private bool linkDirection;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        otherRT = EditorGUILayout.ObjectField("Other Tunnel", otherRT, typeof(Tunnel), true) as Tunnel;
        linkDirection = EditorGUILayout.Toggle("Link Direction", linkDirection);

        Tunnel rt = target as Tunnel;
        if (GUILayout.Button ("Apply"))
        {
            rt.Set();
            if (otherRT != null) rt.LinkTo(ref otherRT, linkDirection);
        }
    }
}
