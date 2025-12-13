using UnityEngine;

public class PaperSequenceManager : MonoBehaviour
{
    public static PaperSequenceManager Instance;

    public int CurrentIndex { get; private set; } = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetIndex()
    {
        CurrentIndex = 0;
    }

    public void AdvanceIndex()
    {
        CurrentIndex++;
    }
}