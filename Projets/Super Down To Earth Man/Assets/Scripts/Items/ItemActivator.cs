using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivator : MonoBehaviour
{
    public void SetInteractionActive(bool active)
    {
        GetComponent<Collider2D>().enabled = active;
    }
}
