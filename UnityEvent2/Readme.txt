UnityEvent2
=============

Instructions
------------
1. Add namespace
using BitByte;

2. Add Variable
public UnityEvent2[] unityEvent2;

3. Call Event using
unityEvent2.Invoke();

Example.cs
------------
using UnityEngine;
using BitByte;

public class Example : MonoBehaviour {
    public UnityEvent2[] unityEvent2;

    void Awake() {
        CallEvent();
    }
    
    public void CallEvent() {
        unityEvent2.Invoke();
    }
}