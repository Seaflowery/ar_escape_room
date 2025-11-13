using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour
{
    private Animation anim;
    private string[] actions = { "surprise", "speechless", "walk" };
    private int currentIndex = 0;

    void Start()
    {
        StartCoroutine(FindAnimationAfterDelay());
    }

    IEnumerator FindAnimationAfterDelay()
    {
        yield return new WaitForSeconds(1f); // wait for GLB to fully load

        anim = GetComponentInChildren<Animation>();

        if (anim == null)
        {
            Debug.LogError("No Animation component found after loading!");
            yield break;
        }

        anim.Play(actions[currentIndex]);
        Debug.Log("Playing animation: " + actions[currentIndex]);
    }

    void Update()
    {
        if (anim == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        // can be changed to XR controller button on Quest
        // if (Input.GetButtonDown("XRI_Right_PrimaryButton"))
        {
            currentIndex = (currentIndex + 1) % actions.Length;
            anim.Play(actions[currentIndex]);
            Debug.Log("Switched to animation: " + actions[currentIndex]);
        }
    }
}