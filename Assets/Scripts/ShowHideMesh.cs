using Meta.XR.MRUtilityKit;
using UnityEngine;

public class ShowHideMesh : MonoBehaviour
{
    public OVRInput.RawButton showHideButton;

    private EffectMesh effectMesh;

    void Start()
    {
        effectMesh = GetComponent<EffectMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(showHideButton))
        {
            effectMesh.HideMesh = !effectMesh.HideMesh;
        }
    }
}