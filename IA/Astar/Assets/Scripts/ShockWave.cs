using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour {

    public Color color = Color.white;
    public float radius;
    public float speed;

    private void Start()
    {
        StartCoroutine(ExpandCoroutine());
    }

    private IEnumerator ExpandCoroutine ()
    {
        float r = 0f;

        while (transform.localScale.magnitude < radius)
        {
            r += Time.fixedDeltaTime * speed;
            transform.localScale = new Vector3(r, r, 1f);
            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hexacell cell = collision.GetComponent<Hexacell>();
        if (cell != null)
        {
            cell.SetTemporaryColor(color);
            cell.StartCoroutine(cell.GetBackToDefaultColorCoroutine());
            cell.SetWall(false);
            return;
        }
    }
}
