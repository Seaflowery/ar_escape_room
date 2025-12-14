using UnityEngine;

public class Actions : MonoBehaviour
{
    [Header("Quest controller trigger (index trigger)")]
    [SerializeField] private OVRInput.Controller controller = OVRInput.Controller.RTouch;

    [Header("Optional: if you need hand transform elsewhere")]
    [SerializeField] private Transform rightHandTransform;

    [Header("Runtime state")]
    public bool triggerDown { get; private set; }

    private GameObject selectedObj;
    private GameObject lastSelected;

    // These are called by your ISDK event wiring (via SelectionRelay)
    public void NotifySelected(GameObject go)
    {
        selectedObj = go;
        lastSelected = go;
    }

    public void NotifyUnselected(GameObject go)
    {
        if (selectedObj == go) selectedObj = null;
    }

    private void Update()
    {
        // Index trigger down/up
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            TriggerDown();

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
            TriggerUp();
    }

    private GameObject CurrentTarget()
    {
        return selectedObj != null ? selectedObj : lastSelected;
    }

    private void TriggerDown()
    {
        triggerDown = true;

        var go = CurrentTarget();
        if (!go) return;

        string n = go.name;

        if (n.StartsWith("Input"))
        {
            if (go.TryGetComponent<Input_Data>(out var input))
                input.ToggleOutput();
        }
        else if (n == "Switch")
        {
            if (go.TryGetComponent<Switch>(out var sw))
                sw.switchSide = !sw.switchSide;
        }
        else if (IsCarryItem(n))
        {
            // keep your original logic
            if (n == "screw_driver" && go.TryGetComponent<Snap>(out var snap) && snap.rotate)
                return;

            if (go.TryGetComponent<Snap>(out var s))
                s.inPlace = false;
        }
        else if (n.StartsWith("tile"))
        {
            if (go.TryGetComponent<Tile>(out var tile))
                tile.triggerDown = true;
        }
    }

    private void TriggerUp()
    {
        triggerDown = false;

        var go = CurrentTarget();
        if (!go) return;

        string n = go.name;

        if (IsCarryItem(n))
        {
            if (go.TryGetComponent<Snap>(out var snap))
                snap.inPlace = true;
        }
        else if (n.StartsWith("tile"))
        {
            if (go.TryGetComponent<Tile>(out var tile))
                tile.triggerDown = false;
        }
    }

    private bool IsCarryItem(string n)
    {
        return n.StartsWith("Gate") || n == "battery" || n == "screw_driver" || n == "morse_code";
    }
}
