using UnityEngine;

public class SceneSwitch: MonoBehaviour
{
    public void SwitchScene(string sceneName)
    {
        VRSceneManager.Instance.LoadScene(sceneName);
    }
}