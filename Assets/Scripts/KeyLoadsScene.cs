using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyLoadsScene : MonoBehaviour
{
    [SerializeField] private string puzzleSceneName = "PuzzleBoxScene";
    [SerializeField] private string spawnTag = "PlayerSpawn";   // tag on the spawn point in PuzzleBoxScene
    [SerializeField] private string rigTag = "PlayerRig";       // tag on your rig root (optional)

    private bool _loading;

    public void OnGrabbed()
    {
        if (_loading) return;
        _loading = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(puzzleSceneName, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != puzzleSceneName) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        var spawn = GameObject.FindGameObjectWithTag(spawnTag);
        if (spawn == null)
        {
            Debug.LogWarning($"No spawn point found with tag {spawnTag}");
            return;
        }

        // 1) Prefer rig by tag (best: tag your rig root as PlayerRig)
        GameObject rig = GameObject.FindGameObjectWithTag(rigTag);

        // 2) Fallback: try find common rig types
        if (rig == null)
        {
            var ovr = Object.FindFirstObjectByType<OVRCameraRig>();
            if (ovr != null) rig = ovr.gameObject;
        }
#if UNITY_XR_INTERACTION_TOOLKIT
        if (rig == null)
        {
            var xr = Object.FindFirstObjectByType<UnityEngine.XR.Interaction.Toolkit.XROrigin>();
            if (xr != null) rig = xr.gameObject;
        }
#endif

        if (rig == null)
        {
            Debug.LogWarning("No rig found to move. Tag your rig root as PlayerRig.");
            return;
        }

        // Move the rig root to spawn
        rig.transform.SetPositionAndRotation(spawn.transform.position, spawn.transform.rotation);
    }
}
