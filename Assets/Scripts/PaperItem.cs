using UnityEngine;
using UnityEngine.Events;

public class PaperItem : MonoBehaviour
{
    public UnityEvent onTriggered;

    public void InvokeTriggered()
    {
        onTriggered?.Invoke();
    }
}