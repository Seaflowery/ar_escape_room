using UnityEngine;
using UnityEngine.Events;

public class BatterySocket : MonoBehaviour
{
    [Header("Snap target (position + rotation)")]
    [SerializeField] private Transform snapPoint;   // create an empty child in holder and assign it

    [Header("Detection")]
    [SerializeField] private string batteryTag = "Battery"; // tag your battery as Battery
    [SerializeField] private float snapDistance = 0.08f;    // meters (8cm)

    [Header("Behavior")]
    [SerializeField] private bool snapOnlyWhenReleased = true; // recommended
    [SerializeField] private bool disableBatteryGrabOnInsert = true; // turn off grabbable component
    [SerializeField] private MonoBehaviour batteryGrabComponent; // optional: assign Grabbable / GrabInteractable here

    [Header("Output as input value")]
    [SerializeField] private BoolSignal batteryInsertedSignal;

    [Header("Events")]
    public UnityEvent OnBatteryInserted;

    private Battery _inserted;

    private void Reset()
    {
        // Try to auto-find snapPoint if you named it "SnapPoint"
        Transform t = transform.Find("SnapPoint");
        if (t != null) snapPoint = t;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_inserted != null) return;
        if (!other.CompareTag(batteryTag)) return;

        Battery battery = other.GetComponentInParent<Battery>();
        if (battery == null) return;
        if (battery.IsInserted) return;

        if (snapOnlyWhenReleased && battery.IsHeld) return;

        // Close enough to snap?
        if (snapPoint == null) return;
        float d = Vector3.Distance(battery.transform.position, snapPoint.position);
        if (d > snapDistance) return;

        // Snap now
        _inserted = battery;
        battery.SnapTo(snapPoint, this.transform);

        // Optional: disable grab component so it can't be pulled / won't fight the snap
        if (disableBatteryGrabOnInsert)
        {
            // Option A: if you dragged a specific grab component into batteryGrabComponent
            if (batteryGrabComponent != null) batteryGrabComponent.enabled = false;

            // Option B: try to disable ANY grab-like component on the battery (safe fallback)
            // Uncomment one of these if you know the exact component name in your project:
            // var grabbable = battery.GetComponentInChildren<Oculus.Interaction.Grabbable>();
            // if (grabbable != null) grabbable.enabled = false;
        }

        // Output signal
        if (batteryInsertedSignal != null) batteryInsertedSignal.Set(true);

        OnBatteryInserted?.Invoke();
    }
}
