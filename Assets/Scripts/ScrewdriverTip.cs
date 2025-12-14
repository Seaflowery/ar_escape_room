using UnityEngine;

public class ScrewdriverTip : MonoBehaviour
{
    [Header("Must be true to trigger screw (set by grab events)")]
    public bool onlyWorkWhenHeld = true;

    private bool _isHeld;

    // Wire these from Meta ISDK grab events
    public void OnGrabbed() => _isHeld = true;
    public void OnReleased() => _isHeld = false;

    private void OnTriggerEnter(Collider other)
    {
        if (onlyWorkWhenHeld && !_isHeld) return;

        // Find ScrewCross on the thing we touched (or its parent)
        ScrewCross screw = other.GetComponent<ScrewCross>();
        if (screw == null) screw = other.GetComponentInParent<ScrewCross>();
        if (screw == null) return;

        screw.OnScrewdriverPoked();
    }
}
