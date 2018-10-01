using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellLabel : MonoBehaviour {

    public string Content
    {
        get { return GetComponentInChildren<Text>().text; }
        set { SetContent(value);  }
    }

	public void SetColor(Color c)
    {
        GetComponent<SpriteRenderer>().color = c;
        GetComponentInChildren<Text>().color = c;
    }

    public void SetContent(string s)
    {
        GetComponentInChildren<Text>().text = s;
    }

    public void AddContent(string s)
    {
        if (s != " ") GetComponentInChildren<Text>().text = Content + " " + s;
    }
}
