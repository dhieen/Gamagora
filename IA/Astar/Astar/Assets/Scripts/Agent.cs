using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    public Transform heroCell;
    public float speed;
    public Vector2Int Coordinates {get; private set; }
    public Vector2Int prevCoordinates { get; private set; }
    public Color cellColor;

    private GameBoard board;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        board = FindObjectOfType<GameBoard>();
    }

    private void FixedUpdate()
    {
        Controls();
    }

    public void Move (Vector2Int direction)
    {
        Vector2 normalizedDir = new Vector2(direction.x, direction.y).normalized;

        rb.AddForce (normalizedDir * speed);
    }

    public void UseWallPower()
    {
        board.GetCell(prevCoordinates).SetWall(true) ;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hexacell cell = collision.GetComponent<Hexacell>();
        if (cell != null && Coordinates != cell.coordinates)
        {
            Coordinates = cell.coordinates;
            cell.SetTemporaryColor(cellColor);
            heroCell.position = board.CoordinatesToPosition(Coordinates);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Hexacell cell = collision.GetComponent<Hexacell>();
        if (cell != null && prevCoordinates != cell.coordinates)
        {
            prevCoordinates = cell.coordinates;
            cell.StartCoroutine (cell.GetBackToDefaultColorCoroutine());

            if (Input.GetButton("Fire1")) UseWallPower();
        }
    }

    private void Controls ()
    {
        int xInput = (int)Input.GetAxisRaw("Horizontal");
        int yInput = (int)Input.GetAxisRaw("Vertical");
        Vector2Int directionInput = new Vector2Int(xInput, yInput);
        Move(directionInput);
    }
}
