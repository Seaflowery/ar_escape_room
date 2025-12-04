using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;           // Keyboard
using UnityEngine.InputSystem.Controls;  // ButtonControl
using UnityEngine.InputSystem.XR;        

public class XRPlayerHealth : MonoBehaviour
{
    public InputActionReference primaryButtonAction;
    public InputActionReference secondaryButtonAction;

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

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R pressed (keyboard)");
            RestartGame();
            return;
        }
        
        if (primaryButtonAction.action.WasPressedThisFrame())
        {
            Debug.Log("Primary pressed (simulator)");
            RestartGame();
            return;
        }

        if (secondaryButtonAction.action.WasPressedThisFrame())
        {
            Debug.Log("Secondary pressed (simulator)");
            RestartGame();
            return;
        }
    }
       
    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        isDead = false;
        currentHp = maxHp;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        // 重新允许刷 ghost
        BalloonGhostController.globalGhostSpawningEnabled = true;

        // 重置所有 ghost 的内部计时器和状态，让它们重新开始计时再刷出来
        BalloonGhostController.ResetGlobalSpawnClock();
    }
}
