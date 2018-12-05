using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PrefabPicker
{
    [System.Serializable]
    public struct RandomPrefab
    {
        public GameObject prefab;
        public int probability;
    }

    public List<RandomPrefab> prefabs;

    public GameObject PickRandom()
    {
        int sumPrefabProbabilities = 0;
        foreach (RandomPrefab rp in prefabs) sumPrefabProbabilities += rp.probability;

        int randomIndex = Random.Range(0, sumPrefabProbabilities);
        int currentRoof = 0;

        foreach (RandomPrefab rp in prefabs)
        {
            currentRoof += rp.probability;
            if (randomIndex <= currentRoof) return rp.prefab;
        }

        return null;
    }
}

public class BlocSpawner : MonoBehaviour
{
    public PrefabPicker prefabPicker;
    public int spawnThreshold;

    private List<Trigger2DHelper> sensors;
    private ItemSpawner itemSpawner;

	void Start ()
    {
        sensors = new List<Trigger2DHelper>(GetComponentsInChildren<Trigger2DHelper>());
        itemSpawner = GetComponent<ItemSpawner>();
	}
	
	void Update ()
    {
        List<Trigger2DHelper> freeSensors = sensors.FindAll(s => s.IsColliding == false);
        int freeCount = freeSensors.Count;

        if (freeCount > spawnThreshold)
        {
            GameObject randomPrefab = prefabPicker.PickRandom();
            GameObject go = SpawnBlocOn(randomPrefab, freeSensors[Random.Range(0, freeCount)].transform.position);
            itemSpawner.SpawnAroundBloc(go.GetComponent<PolygonCollider2D>());
        }
	}

    private GameObject SpawnBlocOn (GameObject prefab, Vector2 position)
    {
        Object newBloc = Instantiate<Object>(prefab, position, Quaternion.identity);      
        GameObject go = newBloc as GameObject;

        RandomPoly rp = go.GetComponent<RandomPoly>();
        if (rp != null) rp.Randomize(); 

        return go;
    }
}
