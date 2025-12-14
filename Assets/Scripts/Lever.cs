using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("Condition: this wire turns BLUE when true")]
    [SerializeField] private Wire resultWire;   // assign the RESULT wire here

    [Header("Target action")]
    [SerializeField] private Drawer drawer;     // assign the drawer here

    [Header("Lever visual (optional)")]
    [SerializeField] private Transform leverHandle;  // leave empty to rotate this transform
    [SerializeField] private Vector3 offEuler = new Vector3(0f, 0f, 45f);
    [SerializeField] private Vector3 onEuler = new Vector3(0f, 0f, -45);

    [Header("Rules")]
    [SerializeField] private bool onlyWorkWhenWireIsTrue = true;
    [SerializeField] private bool triggerDrawerOnlyWhenTurningOn = true;
    [SerializeField] private float debounceSeconds = 0.5f;

    private bool _isOn;
    private float _lastPokeTime = -999f;
    private float _revertAt = -1f;

    private void Awake()
    {
        _isOn = false;
        if (leverHandle == null) leverHandle = transform;

        // Always apply initial visual (your old code could skip this)
        ApplyVisual();
    }

    private void Update()
    {
        if (_revertAt > 0f && Time.time >= _revertAt)
        {
            _revertAt = -1f;
            ApplyVisual();
        }
    }

    // Wire this from Meta ISDK: PokeInteractable -> When Select
    public void OnPoked()
    {
        // prevent double fire from a single poke
        if (Time.time - _lastPokeTime < debounceSeconds) return;
        _lastPokeTime = Time.time;

        bool prev = _isOn;

        // flip first for instant feedback
        _isOn = !_isOn;
        ApplyVisual();

        // check wire
        bool wireIsTrue = (resultWire != null && resultWire.value);
        Debug.Log(wireIsTrue);

        if (!wireIsTrue)
        {
            // revert back if wire not correct
            _isOn = prev;
            _revertAt = Time.time + 0.5f;
            return;
        }

        if (drawer != null)
        {
            // optional: only trigger when turning ON
            if (!triggerDrawerOnlyWhenTurningOn || _isOn)
            {
                drawer.Activate(); // or drawer.Open() if you want strictly "open"
            }
        }
    }

    private void ApplyVisual()
    {
        if (leverHandle == null) return;
        Debug.Log(_isOn);
        leverHandle.localRotation = Quaternion.Euler(_isOn ? onEuler : offEuler);
    }
}
