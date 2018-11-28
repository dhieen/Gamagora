using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomTunnel))]
public class RandomTunnelInspector : Editor
{
    private RandomTunnel otherRT;
    private bool linkDirection;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        otherRT = EditorGUILayout.ObjectField("Other Tunnel", otherRT, typeof(RandomTunnel), true) as RandomTunnel;
        linkDirection = EditorGUILayout.Toggle("Link Direction", linkDirection);

        RandomTunnel rt = target as RandomTunnel;
        if (GUILayout.Button ("Apply"))
        {
            rt.Set();
            if (otherRT != null) rt.LinkTo(ref otherRT, linkDirection);
        }
    }
}
