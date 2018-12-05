using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTunnelGenerator : MonoBehaviour
{
    public Tunnel tunnelPrefab;

    public float minAngleVariation;
    public float maxAngleVariation;
    public float minWidth;
    public float maxWidth;
    public float minLength;
    public float maxLength;

    private int tunnelCount = 0;
    private ItemSpawner itemSpawn;

    private void Start()
    {
        itemSpawn = GetComponent<ItemSpawner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tunnel tunnel = collision.GetComponent<Tunnel>();
        if (tunnel == null) return;

        if (tunnel.connections0.Count == 0) AddConnections0(tunnel);        
        if (tunnel.connections1.Count == 0) AddConnections1(tunnel);
                
        foreach (Tunnel connectedTunnel in tunnel.connections0)
        {
            if (connectedTunnel.connections0.Count == 0)
            {
                AddConnections0(connectedTunnel);
                itemSpawn.SpawnAlongTunnel(connectedTunnel);
            }
        }

        foreach (Tunnel connectedTunnel in tunnel.connections1)
        {
            if (connectedTunnel.connections1.Count == 0)
            {
                AddConnections1(connectedTunnel);
                itemSpawn.SpawnAlongTunnel(connectedTunnel);
            }
        }
    }

    private void AddConnections0(Tunnel tunnel)
    {
        Tunnel newTunnel = RandomTunnel(tunnelPrefab, tunnel);
        newTunnel.LinkTo(ref tunnel, false);
        tunnelCount++;
        newTunnel.name = "Tunnel " + tunnelCount;
    }

    private void AddConnections1(Tunnel tunnel)
    {
        Tunnel newTunnel = RandomTunnel(tunnelPrefab, tunnel);
        newTunnel.LinkTo(ref tunnel, true);
        tunnelCount++;
        newTunnel.name = "Tunnel " + tunnelCount;
    }

    private Tunnel RandomTunnel (Tunnel model, Tunnel connected)
    {
        Tunnel newTunnel = Instantiate<Tunnel>(model);
        newTunnel.segment = Random.Range(minLength, maxLength) * (Quaternion.Euler(0f, 0f, Random.Range (-1, 2) * Random.Range(minAngleVariation, maxAngleVariation)) * connected.segment.normalized);
        newTunnel.width = Random.Range(minWidth, maxWidth);
        newTunnel.Set();

        return newTunnel;
    }
}
