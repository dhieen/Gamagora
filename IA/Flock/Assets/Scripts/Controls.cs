using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Controls : MonoBehaviour {

    public DirectionControlSwitch hControl;
    public DirectionControlSwitch vControl;
    public HoldControlSwitch sControl;
    public HoldControlSwitch fControl;

    private UnityAction<int> hAction;
    private UnityAction<int> vAction;
    private UnityAction<bool> sAction;
    private UnityAction<bool> fAction;
    private Boid controlledBoid;

    private void Awake()
    {
        controlledBoid = GetComponent<Boid>();
        hAction = new UnityAction<int>(HorizontalHandler);
        vAction = new UnityAction<int>(VerticalHandler);
        sAction = new UnityAction<bool>(SpinHandler);
        fAction = new UnityAction<bool>(ForwardHandler);
    }

    private void Update()
    {
        hControl.Run();
        vControl.Run();
        sControl.Run();
        fControl.Run();
    }

    private void OnEnable()
    {
        hControl.AddListener(hAction);
        vControl.AddListener(vAction);
        sControl.AddListener(sAction);
        fControl.AddListener(fAction);
    }

    private void OnDisable()
    {
        hControl.RemoveListener(hAction);
        vControl.RemoveListener(vAction);
        sControl.RemoveListener(sAction);
        fControl.RemoveListener(fAction);
    }

    private void HorizontalHandler (int direction)
    {
        if (sControl.currentState == false)
        {
            switch (direction)
            {
                case 1: controlledBoid.HSteer(DIRECTION.RIGHT); break;
                case -1: controlledBoid.HSteer(DIRECTION.LEFT); break;
                case 0: controlledBoid.HSteer(DIRECTION.ZERO); break;
            }
        }
        else
        {
            switch (direction)
            {
                case 1: controlledBoid.ZSteer(DIRECTION.BACK); break;
                case -1: controlledBoid.ZSteer(DIRECTION.FRONT); break;
                case 0: controlledBoid.ZSteer(DIRECTION.ZERO); break;
            }
        }
    }

    private void SpinHandler (bool start)
    {
        if (start)
            controlledBoid.HSteer(DIRECTION.ZERO);
        else
            controlledBoid.ZSteer(DIRECTION.ZERO);

        HorizontalHandler(hControl.currentDirection);
    }

    private void VerticalHandler (int direction)
    {
        switch (direction)
        {
            case 1: controlledBoid.VSteer(DIRECTION.UP); break;
            case -1: controlledBoid.VSteer(DIRECTION.DOWN); break;
            case 0: controlledBoid.VSteer(DIRECTION.ZERO); break;
        }
    }

    private void ForwardHandler (bool start)
    {
        controlledBoid.AccelerateForward(start);
    }
}
