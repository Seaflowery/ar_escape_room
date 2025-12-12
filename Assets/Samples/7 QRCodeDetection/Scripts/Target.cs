using UnityEngine;
using System.Collections; // <-- needed


public class Target : MonoBehaviour
{
    public float hideSeconds = 3f;
    Renderer[] rends;
    Collider[] cols;

    void Awake()
    {
        rends = GetComponentsInChildren<Renderer>(true);
        cols = GetComponentsInChildren<Collider>(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("projectile"))
        {
            StartCoroutine(HideRoutine());
            Destroy(other.attachedRigidbody.gameObject); // destroy the projectile
        }
    }

    IEnumerator HideRoutine()
    {
        foreach (var r in rends) r.enabled = false;
        foreach (var c in cols) c.enabled = false;
        yield return new WaitForSeconds(hideSeconds);
        foreach (var r in rends) r.enabled = true;
        foreach (var c in cols) c.enabled = true;
    }
}
