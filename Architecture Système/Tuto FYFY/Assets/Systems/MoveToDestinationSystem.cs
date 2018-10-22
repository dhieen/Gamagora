using UnityEngine;
using FYFY;

public class MoveToDestinationSystem : FSystem {

    private Family _randomMovingGO = FamilyManager.getFamily(new AllOfComponents(typeof(MoveToDestination)));

    public MoveToDestinationSystem()
    {
        foreach (GameObject go in _randomMovingGO)
        {
            MoveToDestination move = go.GetComponent<MoveToDestination>();
            Transform tr = go.GetComponent<Transform>();
            move.currentDestination = tr.position;
        }
    }

    protected override void onProcess(int familiesUpdateCount)
    {
        foreach (GameObject go in _randomMovingGO)
        {
            MoveToDestination move = go.GetComponent<MoveToDestination>();
            Transform tr = go.GetComponent<Transform>();

            if (Vector3.Distance (tr.position, move.currentDestination) > .1f)
            {
                Vector3 movement = (move.currentDestination - tr.position).normalized;
                movement.Scale(new Vector3(1f, 1f, 0f));
                tr.position += movement * move.speed * Time.fixedDeltaTime;
            }            
        }
	}
}