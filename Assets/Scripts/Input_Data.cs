using UnityEngine;

public class Input_Data : MonoBehaviour
{
    [HideInInspector] public bool output;
    public bool forceOn;

    void Awake()
    {
        output = forceOn;
        ApplyRotation();
    }

    void Update()
    {
        ApplyRotation();
    }

    // Call these from Meta ISDK events (Select/Unselect/Activate/etc.)
    public void SetOutput(bool value) => output = value;
    public void ToggleOutput() => output = !output;

    private void ApplyRotation()
    {
        // Use localEulerAngles so you’re not rebuilding quaternions every frame
        var e = transform.localEulerAngles;
        e.z = output ? 180f : 0f;
        transform.localEulerAngles = e;
    }
}
