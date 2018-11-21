using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{    
    public int points;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemPicker picker = collision.GetComponent<ItemPicker>();

        if (picker != null)
        {
            GetPicked();
        }
    }

    private void GetPicked ()
    {
        Debug.Log("GetPicked");
        GameObject.Destroy(this.gameObject);
    }
}
