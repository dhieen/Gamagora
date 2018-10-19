using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : MonoBehaviour {

    public string eatableTag = "Virus";
	void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag(eatableTag)) Destroy(other.gameObject);
    }
}
