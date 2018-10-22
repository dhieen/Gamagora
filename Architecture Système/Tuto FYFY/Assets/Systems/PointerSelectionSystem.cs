using UnityEngine;
using FYFY;
using FYFY_plugins.PointerManager;

using System.Collections.Generic;

public class PointerSelectionSystem : FSystem
{
    private Family _selectorPointerGO = FamilyManager.getFamily(new AllOfComponents(typeof(SelectorPointer)));
    private Family _pointedGO = FamilyManager.getFamily(new AllOfComponents(typeof(PointerOver)));

    protected override void onProcess(int familiesUpdateCount)
    {
        SelectorPointer sp = _selectorPointerGO.First().GetComponent<SelectorPointer>();
        sp.selectionChanged = false;

        if (Input.GetMouseButtonDown(0))
        {
            Camera camera = Camera.main;
            sp.selectedGO = new List<GameObject>();
            sp.selectedGO.Add(_pointedGO.First());
            sp.pointerPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            sp.selectionChanged = true;
        }
    }
}