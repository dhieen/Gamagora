using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroControls : MonoBehaviour
{
    public HeroMovements hero;
    public string controlAxisName = "Horizontal";

    private void FixedUpdate()
    {
        hero.controlDirection = Input.GetAxisRaw(controlAxisName);

        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width / 2)
            {
                hero.controlDirection = -1f;
            }
            else
            {
                hero.controlDirection = 1f;
            }
        }
    }
}
