using System.Collections;
using UnityEngine;

public class ElectricPlate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float totalDegrees = 120f;   // your old script: 120 frames * 1 deg = 120 deg
    public float degPerSecond = 360f;   // speed (360 = 1 circle per second)

    private bool _isRotating;
    private Coroutine _co;

    // Call this from ScrewCross
    public void StartRotate()
    {
        if (_isRotating) return;
        _co = StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        _isRotating = true;

        float rotated = 0f;
        while (rotated < totalDegrees)
        {
            float step = degPerSecond * Time.deltaTime;
            if (rotated + step > totalDegrees) step = totalDegrees - rotated;

            transform.Rotate(0f, 0f, -step);
            rotated += step;

            yield return null;
        }

        _isRotating = false;
        _co = null;
    }
}
