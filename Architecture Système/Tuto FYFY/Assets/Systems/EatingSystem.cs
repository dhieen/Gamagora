using UnityEngine;
using FYFY;
using FYFY_plugins.TriggerManager;

public class EatingSystem : FSystem {

    private Family _triggeredGO = FamilyManager.getFamily(new AllOfComponents(typeof(Triggered2D)), new AllOfComponents(typeof(Eater)));

	protected override void onProcess(int familiesUpdateCount)
    {
        foreach (GameObject go in _triggeredGO)
        {
            Triggered2D t2D = go.GetComponent<Triggered2D>();
            Eater eat = go.GetComponent<Eater>();

            foreach (GameObject target in t2D.Targets)
            {
                if (target.CompareTag(eat.eatableTag))
                {
                    GameObjectManager.unbind(target);
                    Object.Destroy(target);
                }
            }
        }
	}
}