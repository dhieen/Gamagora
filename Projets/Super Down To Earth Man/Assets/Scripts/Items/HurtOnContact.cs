using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtOnContact : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemReaction hurt = collision.GetComponent<ItemReaction>();

        if (hurt != null)
        {
            hurt.GetHurt(transform.position, damage);
        }
    }
}
