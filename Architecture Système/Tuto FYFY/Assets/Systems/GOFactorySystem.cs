using System.Collections;
using UnityEngine;
using FYFY;

public class GOFactorySystem : FSystem {

    private Family _factoryGO = FamilyManager.getFamily(new AllOfComponents(typeof(GOFactory)));
    private Family _mainLoop = FamilyManager.getFamily(new AllOfComponents(typeof(MainLoop)));

    public GOFactorySystem()
    {
        foreach (GameObject go in _factoryGO)
            _mainLoop.First().GetComponent<MainLoop>().StartCoroutine(ReloadCoroutine(go.GetComponent<GOFactory>()));
    }
	protected override void onProcess(int familiesUpdateCount)
    {
        
	}

    private IEnumerator ReloadCoroutine(GOFactory factory)
    {
        while (true)
        {
            yield return new WaitForSeconds(factory.runEverySeconds);
            GameObject go = GameObject.Instantiate<GameObject>(factory.productPrefab);
            go.transform.position = factory.transform.position;
            GameObjectManager.bind(go);
        }
    }
}