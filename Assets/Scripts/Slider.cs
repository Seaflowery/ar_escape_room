using UnityEngine;

public class Slider : MonoBehaviour
{
    [Header("Battery insert (from BatterySocket)")]
    [SerializeField] private BoolSignal batteryInsertedSignal;

    [Header("Pointer (visual)")]
    [SerializeField] private Transform pointer;        // the pointer object

    [Header("Radio (2 AudioSources: [0]=noise, [1]=morse)")]
    [SerializeField] private GameObject radio;

    [Header("Slider X range (world)")]
    [SerializeField] private float sliderMinX = -2.932f;
    [SerializeField] private float sliderMaxX = -1.727f;

    [Header("Pointer X range (world)")]
    [SerializeField] private float pointerMinX = -2.74f;
    [SerializeField] private float pointerMaxX = -2.08f;

    private AudioSource[] audioSources;
    private Rigidbody rb;

    private float lockedY;
    private float lockedZ;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (radio != null) audioSources = radio.GetComponents<AudioSource>();

        // Lock initial Y/Z so it behaves like a 1D slider even if physics nudges it
        lockedY = transform.position.y;
        lockedZ = transform.position.z;

        // Initialize pointer based on current slider X
        UpdatePointerAndAudio(transform.position.x, forceMuteIfNoBattery: true);
    }

    // For physics-grabbed objects, clamping in FixedUpdate is usually smoother
    void FixedUpdate()
    {
        // 1) Clamp slider position to X-only track
        Vector3 p = (rb != null) ? rb.position : transform.position;

        float clampedX = Mathf.Clamp(p.x, sliderMinX, sliderMaxX);
        Vector3 clampedPos = new Vector3(clampedX, lockedY, lockedZ);

        if (rb != null && !rb.isKinematic)
        {
            rb.MovePosition(clampedPos);
        }
        else
        {
            transform.position = clampedPos;
        }

        // 2) Update pointer + audio based on clamped X
        UpdatePointerAndAudio(clampedX, forceMuteIfNoBattery: true);
    }

    private void UpdatePointerAndAudio(float sliderX, bool forceMuteIfNoBattery)
    {
        // pointer follow
        if (pointer != null)
        {
            Vector3 newPointerPos = pointer.position;
            newPointerPos.x = Map(sliderMinX, sliderMaxX, pointerMinX, pointerMaxX, sliderX);
            pointer.position = newPointerPos;
        }

        if (audioSources == null || audioSources.Length < 2) return;

        bool batteryInserted = (batteryInsertedSignal != null && batteryInsertedSignal.Value);
        if (!batteryInserted)
        {
            audioSources[0].volume = 0f;
            audioSources[1].volume = 0f;
            return;
        }

        // same logic as your old code: slider position -> t in [0,1]
        float t = Map(sliderMaxX, sliderMinX, 0f, 1f, sliderX); // keep your original direction
        audioSources[0].volume = GetSoundVolume(t, "noise");
        audioSources[1].volume = GetSoundVolume(t, "morse");
    }

    private float Map(float min1, float max1, float min2, float max2, float value)
    {
        float retValue = (min2 + (value - min1) * (max2 - min2) / (max1 - min1));
        if (retValue < Mathf.Min(min2, max2)) retValue = Mathf.Min(min2, max2);
        else if (retValue > Mathf.Max(min2, max2)) retValue = Mathf.Max(min2, max2);
        return retValue;
    }

    private float GetSoundVolume(float sliderPosition, string soundType)
    {
        if (sliderPosition <= 0.6f)
            return (soundType == "noise") ? 1f : 0f;

        if (soundType == "noise")
            return (13f + (-32f) * sliderPosition + 20f * sliderPosition * sliderPosition);

        return (-10.4f + 28f * sliderPosition + (-17.5f) * sliderPosition * sliderPosition);
    }
}
