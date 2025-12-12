using Meta.XR.MRUtilityKit;
using UnityEngine;

public class trackableAdded : MonoBehaviour
{
    public string qrstring = null;

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode &&
            trackable.MarkerPayloadString != null)
        {
            //Debug.Log($"Detected QR code: {trackable.MarkerPayloadString}");
            if (trackable.MarkerPayloadString == qrstring)
            { 
                transform.SetParent(trackable.gameObject.transform, false);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (trackable.TrackableType == OVRAnchor.TrackableType.QRCode &&
            trackable.MarkerPayloadString != null)
        {
            //Debug.Log($"Detected QR code: {trackable.MarkerPayloadString}");
            if (trackable.MarkerPayloadString == qrstring)
            {
                transform.SetParent(null, true);   // keeps current world pose
            }
        }
    }
}
