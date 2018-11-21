using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{    
    public int points;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemReaction picker = collision.GetComponent<ItemReaction>();

        if (picker != null)
        {
            picker.GetPoints(points);
            GetPicked();
        }
    }

    private void GetPicked ()
    {
        GameObject.Destroy(this.gameObject);
    }
}
