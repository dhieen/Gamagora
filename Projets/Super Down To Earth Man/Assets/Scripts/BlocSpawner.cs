using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocSpawner : MonoBehaviour
{
    public GameObject blocPrefab;
    public float radiusParam;
    public float angleParam;
    public float freqParam;
    public int spawnThreshold;

    private List<Trigger2DHelper> sensors;

	void Start ()
    {
        sensors = new List<Trigger2DHelper>(GetComponentsInChildren<Trigger2DHelper>());
	}
	
	void Update ()
    {
        List<Trigger2DHelper> freeSensors = sensors.FindAll(s => s.IsColliding == false);
        int freeCount = freeSensors.Count;

        if (freeCount > spawnThreshold)
        {
            SpawnBlocOn(freeSensors[Random.Range(0, freeCount)].transform.position);
        }
	}

    private void SpawnBlocOn (Vector2 position)
    {
        Object newBloc = Instantiate<Object>(blocPrefab, position, Quaternion.identity);
        RandomPoly poly = new RandomPoly(newBloc as GameObject, radiusParam, freqParam, angleParam);
        poly.Randomize();
    }
}
