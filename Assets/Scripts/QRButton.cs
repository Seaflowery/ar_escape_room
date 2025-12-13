using UnityEngine;
using TMPro;

public class QRButton : MonoBehaviour
{
    [Header("Button Settings")]
    public int buttonId;            
    public float flattenScaleY = 0.2f; 
    public float flattenSpeed = 8f;    
    public float restoreSpeed = 8f;     

    [Header("Number Flash")]
    public TextMeshPro numberText;      
    public float flashInterval = 3f;    
    public float flashDuration = 0.6f;  

    Vector3 originalScale;
    bool isFlattened = false;
    bool isRestoring = false;

    void Start()
    {
        originalScale = transform.localScale;

        if (numberText != null)
        {
            numberText.enabled = false;
            StartCoroutine(FlashNumberLoop());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        QRButtonPuzzleManager.Instance.RegisterPress(this);
    }

    public void Flatten()
    {
        isFlattened = true;
        isRestoring = false;
    }

    public void Restore()
    {
        isFlattened = false;
        isRestoring = true;
    }

    void Update()
    {
        Vector3 scale = transform.localScale;

        if (isFlattened)
        {
            scale.y = Mathf.Lerp(scale.y, flattenScaleY, Time.deltaTime * flattenSpeed);
            transform.localScale = scale;
        }
        else if (isRestoring)
        {
            scale.y = Mathf.Lerp(scale.y, originalScale.y, Time.deltaTime * restoreSpeed);
            transform.localScale = scale;

            if (Mathf.Abs(scale.y - originalScale.y) < 0.001f)
            {
                scale.y = originalScale.y;
                transform.localScale = scale;
                isRestoring = false;
            }
        }
    }

    System.Collections.IEnumerator FlashNumberLoop()
    {
        while (true)
        {
            numberText.enabled = true;
            yield return new WaitForSeconds(flashDuration);

            numberText.enabled = false;
            yield return new WaitForSeconds(flashInterval - flashDuration);
        }
    }
}
