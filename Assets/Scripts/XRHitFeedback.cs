using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class XRHitFeedback : MonoBehaviour
{
    public static XRHitFeedback Instance { get; private set; }

    public float maxAlpha = 0.6f;

    public float fadeDuration = 0.25f;

    private CanvasGroup hitFlashCanvas;
    private Coroutine flashRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        hitFlashCanvas = GetComponent<CanvasGroup>();
        hitFlashCanvas.alpha = 0f;    
    }

    /// <summary>
    /// 玩家受击时调用
    /// </summary>
    public void OnHit(int damage)
    {
        if (hitFlashCanvas == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        hitFlashCanvas.alpha = maxAlpha;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = 1f - (t / fadeDuration);
            hitFlashCanvas.alpha = maxAlpha * k;
            yield return null;
        }

        hitFlashCanvas.alpha = 0f;
    }
}