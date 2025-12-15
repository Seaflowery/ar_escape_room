using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Keypad : MonoBehaviour
{
    [Header("Correct code (order does NOT matter)")]
    private HashSet<int> values;

    private HashSet<int> inputValues;
    private int pressCount;

    [Header("Scene to go back to")]
    [SerializeField] private string mainSceneName = "EscapeRoom"; // <-- set this to your main scene name

    [Header("LED feedback")]
    [SerializeField] private Material materialOn;
    [SerializeField] private Renderer greenLedRenderer; // drag LED_Green's Renderer here (recommended)

    private bool unlocked;

    void Start()
    {
        values = new HashSet<int>(new int[] { 5, 1, 8, 4 });
        inputValues = new HashSet<int>();
        pressCount = 0;

        if (greenLedRenderer == null)
        {
            var led = GameObject.Find("LED_Green");
            if (led != null) greenLedRenderer = led.GetComponent<Renderer>();
        }
    }

    public void AddToSet(int elem)
    {
        if (unlocked) return;

        // Reset after 4 presses (even if duplicates)
        if (pressCount >= 4)
        {
            inputValues.Clear();
            pressCount = 0;
        }

        inputValues.Add(elem);
        pressCount++;

        // Only check when user has pressed 4 times
        if (pressCount == 4 && values.SetEquals(inputValues))
        {
            unlocked = true;

            if (greenLedRenderer != null && materialOn != null)
                greenLedRenderer.material = materialOn;

            SceneManager.LoadScene(mainSceneName);
            // If you prefer index: SceneManager.LoadScene(0); (requires main scene is first in Scene List)
        }
    }
}
