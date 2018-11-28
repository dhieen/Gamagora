using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelCell : WorldCell
{
    public enum SIDE { TOP = 0, RIGHT = 1, BOTTOM = 2, LEFT = 3 }

    [System.Serializable]
    public struct Connection
    {
        [SerializeField] SIDE side;
        [SerializeField] Vector2 worldPosition;
        [SerializeField] float width;

        public Connection(SIDE s, Vector2 pos, float w)
        {
            side = s;
            worldPosition = pos;
            width = w;
        }
    }

    public float minWidth;
    public float maxWidth;
    public float maxAngle;

    public List<Connection> connections;

    public override void GenerateContent(Vector2 bound, List<WorldCell> neigbours)
    {
        float width = Random.Range(minWidth, maxWidth);

        SIDE startSide = (SIDE)Random.Range(0, 4);
        SIDE endSide = (SIDE)Random.Range(0, 3);
        if (endSide >= startSide) endSide += 1;

        Vector2 startPoint = new Vector2();
        Vector2 endPoint = new Vector2();

        if (neigbours == null || neigbours.Count == 0)
        {
            switch (startSide)
            {
                case SIDE.BOTTOM:
                    startPoint.x = Random.Range(width / 2f, bound.x - width / 2f);
                    startPoint.y = 0f;
                    break;
                case SIDE.LEFT:
                    startPoint.x = 0f;
                    startPoint.y = Random.Range(width / 2f, bound.y - width / 2f);
                    break;
                case SIDE.RIGHT:
                    startPoint.x = bound.x;
                    startPoint.y = Random.Range(width / 2f, bound.y - width / 2f);
                    break;
                case SIDE.TOP:
                    startPoint.x = Random.Range(width / 2f, bound.x - width / 2f);
                    startPoint.y = bound.y;
                    break;
            }
        }        

        switch (endSide)
        {
            case SIDE.BOTTOM:
                endPoint.x = Random.Range(width/2f, bound.x - width/2f);
                endPoint.y = 0f;
                break;
            case SIDE.LEFT:
                endPoint.x = 0f;
                endPoint.y = Random.Range(width/2f, bound.y - width/2f);
                break;
            case SIDE.RIGHT:
                endPoint.x = bound.x;
                endPoint.y = Random.Range(width/2f, bound.y - width/2f);
                break;
            case SIDE.TOP:
                endPoint.x = Random.Range(width/2f, bound.x - width/2f);
                endPoint.y = bound.y;
                break;
        }

        connections = new List<Connection>();
        connections.Add(new Connection (startSide, startPoint, width));
        connections.Add(new Connection(endSide, endPoint, width));

        SetLines(startPoint, endPoint, width);
        SetColliders(startPoint, endPoint, width);
    }

    private void SetLines(Vector2 startPt, Vector2 endPt, float width)
    {
        LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
        if (lr == null || lr.Length != 2) return;

        Vector2 segment = endPt - startPt;
        Vector2 normal = new Vector2(-segment.y, segment.x).normalized;

        lr[0].positionCount = 2;
        lr[0].SetPositions(new Vector3[]
        {
            startPt + normal * width/2f,
            endPt + normal * width/2f
        });

        lr[1].positionCount = 2;
        lr[1].SetPositions(new Vector3[]
        {
            startPt - normal * width/2f,
            endPt - normal * width/2f
        });
    }

    private void SetColliders(Vector2 startPt, Vector2 endPt, float width)
    {
        EdgeCollider2D[] col = GetComponentsInChildren<EdgeCollider2D>();
        if (col == null || col.Length != 2) return;

        Vector2 segment = endPt - startPt;
        Vector2 normal = new Vector2(-segment.y, segment.x).normalized;

        col[0].points = new Vector2[]
        {
            startPt + normal * width/2f,
            endPt + normal * width/2f
        };

        col[1].points = new Vector2[]
        {
            startPt - normal * width/2f,
            endPt - normal * width/2f
        };
    }
}
