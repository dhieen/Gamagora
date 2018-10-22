using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PressControlSwitch
{
    public class PressControlEvent : UnityEvent { }

    public string inputButtonName;

    private PressControlEvent controlEvent;

    public void AddListener(UnityAction call)
    {
        if (controlEvent == null) controlEvent = new PressControlEvent();
        controlEvent.AddListener(call);
    }

    public void RemoveListener(UnityAction call)
    {
        if (controlEvent == null) return;
        controlEvent.RemoveListener(call);
    }

    public void Run()
    {
        if (controlEvent != null && Input.GetButtonDown (inputButtonName))
            controlEvent.Invoke();
    }

    public void Destroy()
    {
        controlEvent.RemoveAllListeners();
        controlEvent = null;
    }
}

[System.Serializable]
public class HoldControlSwitch
{
    public class HoldControlEvent : UnityEvent<bool> { }

    public string inputButtonName;

    private HoldControlEvent controlEvent;
    public bool currentState { get; private set; }

    public void AddListener (UnityAction<bool> call)
    {
        if (controlEvent == null) controlEvent = new HoldControlEvent();
        controlEvent.AddListener(call);
    }

    public void RemoveListener(UnityAction<bool> call)
    {
        if (controlEvent == null) return;
        controlEvent.RemoveListener(call);
    }

    public void Run()
    {
        if (currentState != Input.GetButton(inputButtonName))
        {
            currentState = !currentState;
            if (controlEvent != null) controlEvent.Invoke(currentState);
        }
    }

    public void Destroy()
    {
        controlEvent.RemoveAllListeners();
        controlEvent = null;
    }
}

[System.Serializable]
public class DirectionControlSwitch
{
    public class DirectionControlEvent : UnityEvent<int> { }

    public string inputAxisName;
    public float neutralThresh = 0.2f;
    public bool smooth;

    private DirectionControlEvent controlEvent;
    public int currentDirection { get; private set; }

    public void AddListener(UnityAction<int> call)
    {
        if (controlEvent == null) controlEvent = new DirectionControlEvent();
        controlEvent.AddListener(call);
    }

    public void RemoveListener(UnityAction<int> call)
    {
        if (controlEvent == null) return;
        controlEvent.RemoveListener(call);
    }

    public void Run()
    {
        float input = smooth ? Input.GetAxis(inputAxisName) : Input.GetAxisRaw(inputAxisName);

        int inputDirection = 0;
        if (input > neutralThresh) inputDirection = 1;
        else if (input < -neutralThresh) inputDirection = -1;

        if (inputDirection != currentDirection)
        {
            currentDirection = inputDirection;
            if (controlEvent != null) controlEvent.Invoke(currentDirection);
        }
    }

    public void Destroy()
    {
        controlEvent.RemoveAllListeners();
        controlEvent = null;
    }
}