using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCell : MonoBehaviour {

    public Vector2Int coordinates;
    public Sprite defaultSprite;
    public Sprite wallSprite;
    public float colorChangeSpeed;
    public bool isStatic;

    public bool IsWall { get; private set; }

    private SpriteRenderer ren;
    private Collider2D col;
    private Color defaultColor;

    public void Init()
    {
        ren = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        IsWall = false;
    }

    public void SetColor (Color c)
    {
        ren.color = c;
        defaultColor = c;
    }

    public void SetWall (bool wall)
    {
        if (isStatic) return;
        ren.sprite = wall ? wallSprite : defaultSprite;
        col.isTrigger = !wall;
        IsWall = wall;
    }

    public  void SetRenderOrder(int order)
    {
        ren.sortingOrder = order;
    }

    public void SetTemporaryColor (Color c)
    {
        ren.color = c;
        StopCoroutine(GetBackToDefaultColorCoroutine());
    }

    public IEnumerator GetBackToDefaultColorCoroutine ()
    {
        while (ren.color != defaultColor)
        {
            ren.color = Color.Lerp(ren.color, defaultColor, Time.fixedDeltaTime * colorChangeSpeed);
            yield return new WaitForFixedUpdate();

        }
    }
}
