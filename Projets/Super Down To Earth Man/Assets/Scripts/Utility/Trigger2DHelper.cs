using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2DHelper : MonoBehaviour
{
    public List<string> onlyWithTags;
    public List<Collider2D> collisions { get; private set; }
    public bool IsColliding
    {
        get { return collisions.Count > 0; }
    }


    private void Start()
    {
        collisions = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onlyWithTags.Count == 0 || onlyWithTags.Contains(collision.tag))
            collisions.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (onlyWithTags.Count == 0 || onlyWithTags.Contains(collision.tag))
            collisions.Remove(collision);
    }
}
