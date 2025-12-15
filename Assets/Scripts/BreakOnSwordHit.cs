using UnityEngine;

public class BreakOnSwordHit : MonoBehaviour
{
    public GameObject brokenPrefab;
    public float destroyBrokenAfter = 5f;

    private bool _broken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_broken) return;

        Debug.Log("Hit trigger with: " + other.name);

        // If the collider is on a child, this still works:
        if (!other.CompareTag("Sword") && other.GetComponentInParent<Transform>()?.CompareTag("Sword") != true)
            return;

        Break();
    }

    private void Break()
    {
        _broken = true;
        Debug.Log("break");

        if (brokenPrefab != null)
        {
            var broken = Instantiate(brokenPrefab, transform.position, transform.rotation);
            Destroy(broken, destroyBrokenAfter);
        }

        Destroy(gameObject);
    }
}
