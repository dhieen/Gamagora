using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StalkersManager : MonoBehaviour {

    public Stalker stalkerPrefab;

    public List<Stalker> stalkers { get; private set; }

    private GameBoard board;
    private UnityAction<Stalker> OnStalkerDeathAction;

    private void Start()
    {
        stalkers = new List<Stalker>();
        Stalker.target = FindObjectOfType<Agent>();
        Stalker.secondaryTargets = new List<Stalker>();
        board = FindObjectOfType<GameBoard>();
        OnStalkerDeathAction = new UnityAction<Stalker>(OnStalkerDeathEvent);

        Invoke("SpawnNewStalker", 2f);
    }

    public void SpawnNewStalker ()
    {
        Vector2Int coordinates = board.GetRandomCell(true, Stalker.target.Coordinates, 2).coordinates;

        Stalker newStalker = Instantiate<Stalker>(stalkerPrefab, null);
        newStalker.Init();
        newStalker.SetCoordinates(coordinates);
        newStalker.dieEvent.AddListener(OnStalkerDeathAction);
        newStalker.StartCoroutine(newStalker.PlayTurnCoroutine());

        stalkers.Add(newStalker);
    }

    private void OnStalkerDeathEvent (Stalker deadStalker)
    {
        if (deadStalker.isDead)
        {
            Stalker.secondaryTargets.Add(deadStalker);
            SpawnNewStalker();
        }
        else
        {
            Stalker.secondaryTargets.Remove(deadStalker);
        }        
    }
}
