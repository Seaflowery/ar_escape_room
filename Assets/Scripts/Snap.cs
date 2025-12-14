using UnityEngine;

public class Snap : MonoBehaviour
{
    private Vector3 position;
    private GameObject placeholder1;
    private GameObject placeholder2;

    public bool soundOn;
    public float th = 0.08f;
    public bool inPlace;
    public bool rotate;

    private Actions actions;
    public Transform rightHand; // assign RightHandAnchor in inspector

    private GameObject screw;
    private Transform screw_parent;

    void Start()
    {
        placeholder1 = GameObject.Find("gate_placeholder1");
        placeholder2 = GameObject.Find("gate_placeholder2");

        inPlace = false;

        actions = GameObject.Find("ActionsObj")?.GetComponent<Actions>();

        screw = GameObject.Find("Screw_Cross_1");
        if (screw) screw_parent = screw.transform.parent;


    }

    void Update()
    {
        position = transform.position;

        // ---- Gate snap ----
        if (name.StartsWith("Gate"))
        {
            if (!placeholder1 || !placeholder2) return;

            if (inPlace && Vector3.Distance(position, placeholder1.transform.position) < th)
                SnapGateTo(placeholder1.transform);
            else if (inPlace && Vector3.Distance(position, placeholder2.transform.position) < th)
                SnapGateTo(placeholder2.transform);
        }
        // ---- Battery snap ----
        else if (name == "battery")
        {
            Vector3 newPos = new Vector3(-1.455f, 3.818f, -1.075f);

            if (inPlace && Vector3.Distance(position, newPos) < th)
            {
                transform.position = newPos;
                transform.rotation = Quaternion.Euler(90f, 0f, -90f);
                inPlace = false;

                var rb = GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }

                tag = "Untagged";

                if (!soundOn)
                {
                    var radio = GameObject.Find("radio");
                    if (radio)
                    {
                        AudioSource[] audioSources = radio.GetComponents<AudioSource>();
                        if (audioSources.Length >= 2)
                        {
                            audioSources[0].volume = 1;
                            audioSources[1].volume = 0;
                        }
                    }
                    soundOn = true;
                }
            }
        }
        // ---- Screwdriver snap + rotate ----
        else if (name == "screw_driver")
        {
            Vector3 newPos = new Vector3(-1.3198f, 3.8926f, -1.1651f);

            if (inPlace && Vector3.Distance(position, newPos) < th)
            {
                transform.position = newPos;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                inPlace = false;
                rotate = true;

                var rb = GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }

            if (rotate && actions != null && actions.triggerDown && rightHand != null && screw != null)
            {
                float zRot = rightHand.eulerAngles.z;
                transform.rotation = Quaternion.Euler(0f, 0f, zRot);

                screw.transform.parent = transform;

                Vector3 newPos2 = transform.position;
                newPos2.z = Map(60f, 210f, -1.1651f, -1.2343f, zRot);
                transform.position = newPos2;

                if (transform.position.z <= -1.2343f)
                {
                    screw.transform.parent = screw_parent;

                    var srb = screw.GetComponent<Rigidbody>();
                    if (srb)
                    {
                        srb.useGravity = true;
                        srb.isKinematic = false;
                    }

                    //var ep = GameObject.Find("electric_plate")?.GetComponent<ElectricPlate>();
                    //if (ep) ep.rotate = true;

                    rotate = false;

                    var rb = GetComponent<Rigidbody>();
                    if (rb)
                    {
                        rb.useGravity = true;
                        rb.isKinematic = false;
                    }
                }
            }
        }
    }

    private void SnapGateTo(Transform ph)
    {
        Vector3 newPos = ph.position;
        newPos.x = -1.13f;
        transform.position = newPos;

        Quaternion rot = ph.rotation;
        Vector3 e = rot.eulerAngles;
        e.z = 90f;
        transform.rotation = Quaternion.Euler(e);

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        inPlace = false;
    }

    private float Map(float min1, float max1, float min2, float max2, float value)
    {
        return (min2 + (value - min1) * (max2 - min2) / (max1 - min1));
    }
}
