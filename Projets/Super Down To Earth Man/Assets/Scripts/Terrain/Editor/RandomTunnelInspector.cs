using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomTunnel))]
public class RandomTunnelInspector : Editor
{
    private RandomTunnel otherRT;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        otherRT = EditorGUILayout.ObjectField("Other Tunnel", otherRT, typeof(RandomTunnel), true) as RandomTunnel;

        RandomTunnel rt = target as RandomTunnel;
        if (GUILayout.Button ("Apply"))
        {
            rt.Set();
            if (otherRT != null) rt.LinkTo(ref otherRT);
        }
    }
}
