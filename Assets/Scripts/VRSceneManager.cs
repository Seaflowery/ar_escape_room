using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VRSceneManager : MonoBehaviour
{
    public static VRSceneManager Instance;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (fadeCanvas != null)
            StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (fadeCanvas != null)
            yield return FadeOut();

        yield return SceneManager.LoadSceneAsync(sceneName);

        if (fadeCanvas != null)
            yield return FadeIn();
    }

    private IEnumerator FadeIn()
    {
        fadeCanvas.blocksRaycasts = true;
        for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
        {
            fadeCanvas.alpha = t / fadeDuration;
            yield return null;
        }
        fadeCanvas.alpha = 0;
        fadeCanvas.blocksRaycasts = false;
    }

    private IEnumerator FadeOut()
    {
        fadeCanvas.blocksRaycasts = true;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            fadeCanvas.alpha = t / fadeDuration;
            yield return null;
        }
        fadeCanvas.alpha = 1;
    }
}