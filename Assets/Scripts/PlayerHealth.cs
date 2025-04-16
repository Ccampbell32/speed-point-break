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
        Debug.Log("Player Died! Activating Game Over UI.");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true); // Activate the Game Over UI
        }
    }
}

