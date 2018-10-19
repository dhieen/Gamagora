using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControls : MonoBehaviour {

    public string controlledTag = "MouseControl";
    public List<MoveToDestination> controlled;
    public Vector3 destination;
}
