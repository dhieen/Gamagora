using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirZoom : MonoBehaviour
{
    public float minSize;
    public float maxSize;
    public float zoomInSpeed;
    public float zoomOutSpeed;

    private Camera cam;
    private HeroMovements hero;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = maxSize;

        hero = FindObjectOfType<HeroMovements>();
    }
       
    private void FixedUpdate()
    {
        if (hero.IsOnGround == false)
        {
            if (cam.orthographicSize < maxSize)
                cam.orthographicSize += zoomOutSpeed * Time.fixedDeltaTime;
        }
        else
        {
            if (cam.orthographicSize > minSize)
                cam.orthographicSize -= zoomInSpeed * Time.fixedDeltaTime;
        }
    }
}
