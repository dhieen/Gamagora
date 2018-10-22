using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusGenerator : MonoBehaviour {

    public GameObject virusPrefab;

    public float reloadTime = 2f;

	void Start ()
    {
        StartCoroutine(ReloadCoroutine());
	}

    private IEnumerator ReloadCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(reloadTime);
            GameObject go = Instantiate<GameObject>(virusPrefab);
        }
    }
}
