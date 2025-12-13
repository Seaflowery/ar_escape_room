using UnityEngine;
using System.Collections.Generic;

public class QRButtonPuzzleManager : MonoBehaviour
{
    public static QRButtonPuzzleManager Instance { get; private set; }

    [Header("Puzzle Settings")]
    public int[] solution = { 1, 0, 2 };

    [Header("Buttons")]
    public List<QRButton> buttons = new List<QRButton>();

    int currentStep = 0;
    bool isSolved = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        if (buttons.Count == 0)
        {
            QRButton[] found = FindObjectsOfType<QRButton>();
            buttons.AddRange(found);
        }
    }

    public void RegisterPress(QRButton button)
    {
        if (isSolved) return;

        if (button.buttonId == solution[currentStep])
        {
            button.Flatten();
            currentStep++;

            if (currentStep >= solution.Length)
            {
                PuzzleSolved();
            }
        }
        else
        {
            ResetPuzzle();
        }
    }

    void PuzzleSolved()
    {
        isSolved = true;

        VRSceneManager.Instance.LoadScene("EscapeRoom");
    }

    void ResetPuzzle()
    {
        Debug.Log("Wrong order, reset puzzle.");

        currentStep = 0;

        foreach (var btn in buttons)
        {
            btn.Restore();
        }
    }
}