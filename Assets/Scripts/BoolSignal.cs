using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoolChangedEvent : UnityEvent<bool> { }

public class BoolSignal : MonoBehaviour
{
    [SerializeField] private bool value;
    public bool Value => value;

    public BoolChangedEvent OnChanged;

    public void Set(bool v)
    {
        if (value == v) return;
        value = v;
        OnChanged?.Invoke(value);
    }
}
