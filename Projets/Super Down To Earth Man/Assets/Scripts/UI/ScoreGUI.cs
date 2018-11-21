using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScoreGUI : MonoBehaviour
{
    public ItemReaction player;

    private Text text;
    private UnityAction<int,int> actionOnPoints;

	void Start ()
    {
        StartCoroutine(Init());        
	}

    IEnumerator Init ()
    {
        yield return new WaitUntil(() => player.Initialized);

        text = GetComponent<Text>();
        actionOnPoints = new UnityAction<int, int>(SetPoints);
        player.getsPoints.AddListener(actionOnPoints);
        SetPoints(0, 0);
    }

    void SetPoints (int gainedPoints, int score)
    {
        text.text = score.ToString();
    }
}
