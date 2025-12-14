using System.Collections;
using UnityEngine;

public class ScrewCross : MonoBehaviour
{
    [Header("What happens when poked by screwdriver")]
    public ElectricPlate plateToRotate;
    public float screwTotalDegrees = 720f;   // 2 circles = 720 degrees
    public float screwDegPerSecond = 720f;   // speed (720 = 2 circles per second)
    public bool destroyAfter = true;         // or disable instead

    private bool _done;

    // Called by ScrewdriverTip
    public void OnScrewdriverPoked()
    {
        if (_done) return;
        _done = true;

        if (plateToRotate != null)
            plateToRotate.StartRotate();

        StartCoroutine(ScrewRoutine());
    }

    private IEnumerator ScrewRoutine()
    {
        float rotated = 0f;
        while (rotated < screwTotalDegrees)
        {
            float step = screwDegPerSecond * Time.deltaTime;
            if (rotated + step > screwTotalDegrees) step = screwTotalDegrees - rotated;

            transform.Rotate(0f, 0f, -step);
            rotated += step;

            yield return null;
        }

        // “Disappear”
        if (destroyAfter)
        {
            Destroy(gameObject);
        }
        else
        {
            // disable visuals + collider
            var r = GetComponentInChildren<Renderer>();
            if (r != null) r.enabled = false;

            var c = GetComponent<Collider>();
            if (c != null) c.enabled = false;

            gameObject.SetActive(false);
        }
    }
}
