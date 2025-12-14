using UnityEngine;
using UnityEngine.Events;

public class Battery : MonoBehaviour
{
    [Header("Runtime")]
    public bool IsHeld { get; private set; }
    public bool IsInserted { get; private set; }

    [Header("Optional callbacks")]
    public UnityEvent OnInserted;
    public UnityEvent OnRemoved;

    private Rigidbody _rb;
    private Transform _originalParent;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _originalParent = transform.parent;
    }

    // Wire from Meta ISDK grab event: When Select
    public void NotifyGrabbed()
    {
        IsHeld = true;
    }

    // Wire from Meta ISDK grab event: When Unselect
    public void NotifyReleased()
    {
        IsHeld = false;
    }

    // Called by BatterySocket when it decides to snap
    public void SnapTo(Transform snapPoint, Transform newParent = null)
    {
        IsInserted = true;

        // Stop physics motion
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true; // lock it in place
        }

        // Move to exact snap pose
        transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);

        // Parent to holder so it stays aligned
        if (newParent != null) transform.SetParent(newParent, true);

        OnInserted?.Invoke();
    }

    // Optional: allow removal later (if you want)
    public void Unsnap()
    {
        IsInserted = false;

        if (_rb != null) _rb.isKinematic = false;
        transform.SetParent(_originalParent, true);

        OnRemoved?.Invoke();
    }
}
