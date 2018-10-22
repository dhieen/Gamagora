using UnityEngine;
using FYFY;

using System.Collections.Generic;

public class MoveControlSystem : FSystem {

    private Family _controlSystemGO = FamilyManager.getFamily(new AllOfComponents(typeof(MoveControls)));
    private Family _selectorPointerGO = FamilyManager.getFamily(new AllOfComponents(typeof(SelectorPointer)));

	protected override void onProcess(int familiesUpdateCount)
    {
        MoveControls controls = _controlSystemGO.First().GetComponent<MoveControls>();
        SelectorPointer sp = _selectorPointerGO.First().GetComponent<SelectorPointer>();

        if (sp.selectionChanged)
        {
            List<GameObject> controlled = sp.selectedGO.FindAll(go => go.CompareTag(controls.controlledTag));

            if (controlled.Count > 0)
            {
                controls.controlled = new List<MoveToDestination>();
                foreach (GameObject go in controlled)
                    controls.controlled.Add(go.GetComponent<MoveToDestination>());
            }
            else
            {
                controls.destination = sp.pointerPosition;
                foreach (MoveToDestination mtd in controls.controlled)
                    mtd.currentDestination = controls.destination;
            }
        }
	}
}