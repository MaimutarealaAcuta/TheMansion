using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("Name of the scene to load when triggered.")]
    [SerializeField] private string sceneName = "MainMenu";

    [Header("Player Settings")]
    [Tooltip("Tag assigned to the player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Delay Before Loading")]
    [Tooltip("Delay in seconds before loading the scene.")]
    [SerializeField] private float delay = 1f;

    private bool hasTriggered = false; // To prevent multiple triggers

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggering object has the specified player tag and ensure the trigger hasn't been activated before
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true; // Prevent re-triggering
            Debug.Log($"Trigger activated by {other.gameObject.name}. Loading scene: {sceneName} in {delay} seconds.");

            // Start the coroutine to load the scene after the specified delay
            StartCoroutine(LoadSceneAfterDelay());
        }
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // Check if the scene exists in the build settings
        if (IsSceneInBuildSettings(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' not found in Build Settings. Please add it to proceed.");
        }
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            // Get the scene path (e.g., "Assets/Scenes/MainMenu.unity")
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            // Extract the scene name
            string currentSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (currentSceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
