using UnityEngine;

public class XRPlayerHealth : MonoBehaviour
{
    [Header("Player HP")]
    public int maxHp = 5;
    public int currentHp;

    [Header("Death UI")]
    [Tooltip("Assign your DeathCanvas object here (text only).")]
    public GameObject deathCanvas;

    private bool isDead = false;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHp = maxHp;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHp -= amount;
        if (XRHitFeedback.Instance != null)
        {
            XRHitFeedback.Instance.OnHit(amount);
        }
        
        Debug.Log($"Player damaged. HP = {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            currentHp = 0;
            OnPlayerDead();
        }
    }

    private void OnPlayerDead()
    {
        Debug.Log("Player Dead!");
        isDead = true;

        if (deathCanvas != null)
            deathCanvas.SetActive(true);

        BalloonGhostController.globalGhostSpawningEnabled = false;
    }

    private void Update()
    {
        if (!isDead) return;

        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.Active))
        {
            Debug.Log("Primary pressed (OVRInput)");
            RestartGame();
        }

    }
       
    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        isDead = false;
        currentHp = maxHp;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        BalloonGhostController.globalGhostSpawningEnabled = true;

        BalloonGhostController.ResetGlobalSpawnClock();
    }
}
