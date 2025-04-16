using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5; // Player's maximum health
    public int currentHealth;
    public bool isDead = false; // Flag to check if the player is dead
    private GameObject gameOverUI; // Private reference to the Game Over UI

    void Start()
    {
        currentHealth = maxHealth; // Initialize health at the start
        // Find the GameOverUI GameObject in the scene
        gameOverUI = GameObject.FindGameObjectWithTag("GameOverUI");
        if (gameOverUI == null)
        {
            Debug.LogError("GameOverUI GameObject not found in the scene!");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0 && !isDead) // Added !isDead check
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
        Debug.Log("PlayerHealth: Player Died!");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Destroy(gameObject);
        GameManager.Instance.PlayerDied(); // Notify GameManager
    }
}
