using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelCollisionActivator : MonoBehaviour
{
    public int activatedLayer;
    public int deactivatedLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tunnel tunnel = collision.GetComponent<Tunnel>();

        if (tunnel != null)
        {
            foreach (EdgeCollider2D e in new List<EdgeCollider2D> (tunnel.GetComponentsInChildren<EdgeCollider2D>()))
                e.gameObject.layer = activatedLayer;

            foreach (Tunnel t in tunnel.connections0)
            {
                Collider2D trigger = t.GetComponent<Collider2D>();
                if (trigger != null) trigger.enabled = true;

                foreach (Tunnel tt in t.connections0)
                {
                    if (tt != tunnel)
                    {
                        Collider2D ttrigger = tt.GetComponent<Collider2D>();
                        if (trigger != null) ttrigger.enabled = false;
                    }
                }

                foreach (Tunnel tt in t.connections1)
                {
                    if (tt != tunnel)
                    {
                        Collider2D ttrigger = tt.GetComponent<Collider2D>();
                        if (trigger != null) ttrigger.enabled = false;
                    }
                }
            }

            foreach (Tunnel t in tunnel.connections1)
            {
                Collider2D trigger = t.GetComponent<Collider2D>();
                if (trigger != null) trigger.enabled = true;

                foreach (Tunnel tt in t.connections0)
                {
                    if (tt != tunnel)
                    {
                        Collider2D ttrigger = tt.GetComponent<Collider2D>();
                        if (trigger != null) ttrigger.enabled = false;
                    }
                }

                foreach (Tunnel tt in t.connections1)
                {
                    if (tt != tunnel)
                    {
                        Collider2D ttrigger = tt.GetComponent<Collider2D>();
                        if (trigger != null) ttrigger.enabled = false;
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tunnel tunnel = collision.GetComponent<Tunnel>();

        if (tunnel != null)
        {
            foreach (EdgeCollider2D e in new List<EdgeCollider2D> (tunnel.GetComponentsInChildren<EdgeCollider2D>()))
                e.gameObject.layer = deactivatedLayer;
        }
    }
}
