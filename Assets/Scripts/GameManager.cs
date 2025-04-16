using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    private bool playerIsDead = false;

    void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure GameManager persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayerDied()
    {
        Debug.Log("GameManager: Player Died!");
        playerIsDead = true;
        StartCoroutine(RestartGameAfterDelay());
    }

    private System.Collections.IEnumerator RestartGameAfterDelay()
    {
        Debug.Log("GameManager: Waiting 5 seconds before restarting...");
        yield return new WaitForSeconds(5f);
        Debug.Log("GameManager: Restarting the game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }
}
