using UnityEngine;
using System.Collections;

public class KeyButton : MonoBehaviour
{
    private Keypad keypad;

    private Vector3 originalScale;
    private float y_scale_factor = 0.5f;

    private float hideSeconds = 0.3f;

    void Start()
    {
        //var battery = GameObject.Find("battery");
        //batterySnap = battery ? battery.GetComponent<Snap>() : null;

        var keypadObj = GameObject.Find("keypad");
        keypad = keypadObj ? keypadObj.GetComponent<Keypad>() : null;

        originalScale = transform.localScale;
    }

    // Call this from InteractableUnityEventWrapper -> When Select()
    public void Press()
    {
        Debug.Log(gameObject.name);

        StartCoroutine(ScaleRoutine());
        //if (batterySnap == null || keypad == null) return;
        //if (!batterySnap.soundOn) return;

        // your naming: "Button_(0)" / "Button_(7)" etc
        int digit = int.Parse(name.Substring(8, 1));
        keypad.AddToSet(digit);
    }

    IEnumerator ScaleRoutine()
    {
        Vector3 buttonPressScale = new Vector3(originalScale.x, originalScale.y * y_scale_factor, originalScale.z);
        transform.localScale = buttonPressScale;
        yield return new WaitForSeconds(hideSeconds);
        transform.localScale = originalScale;
    }
}