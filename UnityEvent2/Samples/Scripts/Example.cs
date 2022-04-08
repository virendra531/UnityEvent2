using UnityEngine;

namespace BitByte{
public class Example : MonoBehaviour {
    public UnityEvent2[] unityEvent2;

    void Awake() {
        CallEvent();
    }

    public void CallEvent() {
        unityEvent2.Invoke();
    }
}
}