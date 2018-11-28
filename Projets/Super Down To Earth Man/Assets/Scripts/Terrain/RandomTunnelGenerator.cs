using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTunnelGenerator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tunnel tunnel = collision.GetComponent<Tunnel>();
        if (tunnel == null) return;

        if (tunnel.connections0.Count == 0)
        {
            Tunnel newTunnel = Instantiate <Tunnel>(tunnel);
            tunnel.Set();
            tunnel.LinkTo(ref tunnel, true);
        }

        if (tunnel.connections1.Count == 0)
        {
            Tunnel newTunnel = Instantiate<Tunnel>(tunnel);
            tunnel.Set();
            tunnel.LinkTo(ref tunnel, true);
        }
    }
}
