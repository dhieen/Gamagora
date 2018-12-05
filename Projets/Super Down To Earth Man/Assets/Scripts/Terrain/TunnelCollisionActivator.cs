using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelCollisionActivator : MonoBehaviour
{
    public int activatedLayer;
    public int deactivatedLayer;
    public float activatedZ;
    public float deactivatedZ;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tunnel tunnel = collision.GetComponent<Tunnel>();
        if (tunnel == null) return;
        
        foreach (Tunnel t in tunnel.connections0)
        {
            SetClose(t);

            foreach (Tunnel tt in t.connections0)
                if (tt != tunnel)
                    SetFar(tt);

            foreach (Tunnel tt in t.connections1)
                if (tt != tunnel)
                    SetFar(tt);
        }

        foreach (Tunnel t in tunnel.connections1)
        {
            SetClose(t);

            foreach (Tunnel tt in t.connections0)
                if (tt != tunnel)
                    SetFar(tt);

            foreach (Tunnel tt in t.connections1)
                if (tt != tunnel)
                    SetFar(tt);
        }
    }

    private void SetClose(Tunnel t)
    {
        t.SetCollisionsLayer(activatedLayer);
        t.SetItemsActive(true);
        t.SetVisualLayer(activatedZ);
    }
    
    private void SetFar (Tunnel t)
    {
        t.SetCollisionsLayer(deactivatedLayer);
        t.SetItemsActive(false);
        t.SetVisualLayer(deactivatedZ);
    }
}
