using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameCell : MonoBehaviour {

    public Sprite defaultSprite;
    public Sprite wallSprite;
    public UnityEvent CellChangeEvent;

    private bool isWall;
    private SpriteRenderer ren;
    private Color defaultColor;

    public bool IsWall
    {
        get { return isWall; }
        set { SetIsWall(value); }
    }

    public void Init()
    {
        ren = GetComponent<SpriteRenderer>();
        CellChangeEvent = new UnityEvent();
    }

    public void SetColor (Color c)
    {
        ren.color = c;
        defaultColor = c;
    }

    private void SetIsWall (bool val)
    {
        isWall = val;

        if (isWall)
        {
            ren.sprite = wallSprite;
            ren.color = Color.grey;
        }
        else
        {
            ren.sprite = defaultSprite;
            ren.color = defaultColor;
        }
    }

    private void OnMouseUpAsButton()
    {
        SetIsWall(!isWall);
        CellChangeEvent.Invoke();
    }
}
