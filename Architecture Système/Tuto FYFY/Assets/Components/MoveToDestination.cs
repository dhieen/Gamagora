using UnityEngine;

public class MoveToDestination : MonoBehaviour
{
    public float speed;
    [HideInInspector] public Vector3 currentDestination;
    [HideInInspector] public Vector2 topLeftLimit;
    [HideInInspector] public Vector2 bottomRightLimit;
}