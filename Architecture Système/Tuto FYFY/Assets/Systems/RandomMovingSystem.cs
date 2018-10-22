using UnityEngine;
using FYFY;

public class RandomMovingSystem : FSystem
{
    private Family _randomMovingGO = FamilyManager.getFamily(new AllOfComponents(typeof(MoveToDestination), typeof(RandomTarget)));
    private Family _boundariesGO = FamilyManager.getFamily(new AllOfComponents(typeof(Boundaries)));
    public RandomMovingSystem ()
    {
        foreach (GameObject go in _randomMovingGO)
            onGOEnter(go);

        _randomMovingGO.addEntryCallback(onGOEnter);
    }

    private void onGOEnter (GameObject go)
    {
        MoveToDestination move = go.GetComponent<MoveToDestination>();
        Transform tr = go.GetComponent<Transform>();

        Boundaries bound = _boundariesGO.First().GetComponent<Boundaries> ();
        move.topLeftLimit = bound.topLeft.position;
        move.bottomRightLimit = bound.bottomRight.position;
        move.currentDestination = GetRandomPositionRange(bound.topLeft.position, bound.bottomRight.position);
        tr.rotation = Quaternion.LookRotation(Vector3.forward, move.currentDestination - tr.position);
        tr.rotation *= Quaternion.AngleAxis(90f, Vector3.forward);

    }

	protected override void onProcess(int familiesUpdateCount)
    {
        foreach (GameObject go in _randomMovingGO)
        {
            MoveToDestination move = go.GetComponent<MoveToDestination>();
            Transform tr = go.GetComponent<Transform>();

            if (Vector3.Distance(move.currentDestination, tr.position) < .1f)
            {
                move.currentDestination = GetRandomPositionRange(move.topLeftLimit, move.bottomRightLimit);
                tr.rotation = Quaternion.LookRotation(Vector3.forward, move.currentDestination - tr.position);
                tr.rotation *= Quaternion.AngleAxis(90f, Vector3.forward);
            }
        }
	}

    private void RandomizeDestination ()
    {


    }

    private Vector3 GetRandomPositionRange(Vector3 topLeft, Vector3 bottomRight)
    {
        return new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(topLeft.y, bottomRight.y), Random.Range(topLeft.z, bottomRight.z));
    }
}