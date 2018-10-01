using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Stalker : MonoBehaviour {

    public class DieEvent : UnityEvent<Stalker> { }

    public class AstarCell
    {
        public Vector2Int pos;
        public float heuristic;
        public int cost;
        public AstarCell prev;
        public float TotalCost () { return (float)cost + heuristic; }
    }

    public static Agent target;
    public static List<Stalker> secondaryTargets;
    public float speed;
    public float panicSpeed;
    public int reactionDelay = 10;
    public int maxLife;
    public int life = 10;
    public int panicLife = 3;
    public Color healthyColor;
    public Color deadColor;
    public ShockWave dieShockWavePrefab;
    public ShockWave reviveShockWavePrefab;

    public DieEvent dieEvent;

    public Vector2Int Coordinates { get; private set; }
    public bool isDead { get; private set; }
    public bool isFrozen { get; private set; }

    private GameBoard board;
    private Rigidbody2D rb;
    private SpriteRenderer ren;

    public void Init()
    {
        board = FindObjectOfType<GameBoard>();
        rb = GetComponent<Rigidbody2D>();
        ren = GetComponent<SpriteRenderer>();
        dieEvent = new DieEvent();

        isDead = false;
    }

    public IEnumerator PlayTurnCoroutine()
    {
        while (!isFrozen)
        {
            List<List<Vector2Int>> possiblePaths = new List<List<Vector2Int>>();
            List<Vector2Int> path = GetPathTo(target.Coordinates);
            if (path != null) possiblePaths.Add(path);
            foreach (Stalker s in Stalker.secondaryTargets)
            {
                path = GetPathTo(s.Coordinates);
                if (path != null) possiblePaths.Add(path);
            }                

            List<Vector2Int> shortestPath = possiblePaths.Find(p => p.Count == possiblePaths.Min(x => x.Count));

            if (shortestPath == null)
            {
                RandomStep();

                ren.color = Color.Lerp(deadColor, healthyColor, (float)(life) / (float)(panicLife));

                if (life > panicLife)
                {
                    yield return new WaitForSecondsRealtime(1f / speed);
                }
                else
                {
                    yield return new WaitForSecondsRealtime((1f / panicSpeed) * ((float)life / (float)panicLife));
                }

                if (--life <= 0)
                {
                    Freeze();
                }
            }
            else if (shortestPath.Count > 0)
            {
                if (life < maxLife)
                {
                    life = maxLife;
                    ren.color = healthyColor;
                }

                for (int i = 0; i < shortestPath.Count; i++)
                {
                    SetCoordinates(shortestPath[i]);
                    yield return new WaitForSecondsRealtime(1f / speed);
                    if (i > reactionDelay) break;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    public void SetCoordinates(Vector2Int coord)
    {
        Coordinates = coord;
        rb.position = board.GetCell(coord).transform.position;
    }

    public void RandomStep()
    {
        List<GameCell> possible = board.GetNeihbours(Coordinates).FindAll(n => !n.IsWall);
        if (possible.Count <= 0) return;
        Vector2Int randomDest = possible[Random.Range(0, possible.Count)].coordinates;

        SetCoordinates(randomDest);
    }

    public List<Vector2Int> GetPathTo(Vector2Int destination)
    {
        List<AstarCell> openList = new List<AstarCell>();
        List<AstarCell> closeList = new List<AstarCell>();

        Vector2Int startCoord = Coordinates;
        AstarCell current = new AstarCell();
        current.pos = startCoord;
        current.cost = 0;
        current.heuristic = 0f;
        current.prev = null;
        openList.Add(current);
        
        bool found = false;

        while (openList.Count > 0 && found == false)
        {
            current = openList.Find(min => min.TotalCost() == openList.Min(x => x.TotalCost()));
            closeList.Add(current);
            openList.Remove(current);

            if (current.pos == destination)
            {
                found = true;
                break;
            }

            foreach (GameCell n in board.GetNeihbours(current.pos))
            {
                if (openList.Find (x=> x.pos==n.coordinates) == null
                    && closeList.Find (x=> x.pos==n.coordinates) == null
                    && !n.IsWall)
                {
                    AstarCell neighbour = new AstarCell();
                    neighbour.pos = n.coordinates;
                    neighbour.cost = current.cost + 1;
                    neighbour.heuristic = Vector2.Distance(n.transform.position, board.GetCell (destination).transform.position);
                    neighbour.prev = current;
                    openList.Add(neighbour);
                }
            }
        }

        if (found)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            for (AstarCell cell = closeList.Last(); cell.pos != startCoord; cell = cell.prev)
            {
                path.Add(cell.pos);
            }
            path.Reverse();
            return path;
        }
        else
        {
            return null;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Agent agent = collision.GetComponent<Agent>();
        if (agent != null)
        {
            Debug.Log("GAME OVER");
        }

        Stalker stalker = collision.GetComponent<Stalker>();
        if (stalker != null && stalker.isDead)
        { 
            stalker.Ressucitate();
        }
    }

    public void Freeze()
    {
        isFrozen = true;
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(1f);
        isDead = true;
        dieEvent.Invoke(this);

        ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        particles.Play();

        ShockWave sw =Instantiate(dieShockWavePrefab);
        sw.transform.position = this.transform.position;
    }

    public void Ressucitate ()
    {
        isDead = false;
        isFrozen = false;
        dieEvent.Invoke(this);

        ShockWave sw = Instantiate(reviveShockWavePrefab);
        sw.transform.position = this.transform.position;

        StartCoroutine(PlayTurnCoroutine());
    }
}
