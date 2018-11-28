using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTunnelGenerator : MonoBehaviour
{
    public float minAngleVariation;
    public float maxAngleVariation;
    public float minWidth;
    public float maxWidth;
    public float minLength;
    public float maxLength;

    private int tunnelCount = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tunnel tunnel = collision.GetComponent<Tunnel>();
        if (tunnel == null) return;


        if (tunnel.connections0.Count == 0)
        {
            Tunnel newTunnel = RandomTunnel(tunnel);
            newTunnel.LinkTo(ref tunnel, false);

            tunnelCount++;
            newTunnel.name = "Tunnel " + tunnelCount;
            newTunnel.transform.position += tunnelCount* Vector3.forward;
        }        
        /*
        if (tunnel.connections1.Count == 0)
        {
            Tunnel newTunnel = RandomTunnel(tunnel);
            tunnel.LinkTo(ref tunnel, true);
        }
        */
    }

    private Tunnel RandomTunnel (Tunnel model)
    {
        Tunnel newTunnel = Instantiate<Tunnel>(model);
        newTunnel.segment = Random.Range(minLength, maxLength) * (Quaternion.Euler(0f, 0f, Random.Range (-1, 2) * Random.Range(minAngleVariation, maxAngleVariation)) * model.segment.normalized);
        newTunnel.width = Random.Range(minWidth, maxWidth);
        newTunnel.Set();

        return newTunnel;
    }
}
