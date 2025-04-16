

using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float roamingSpeed = 2f; // Speed when roaming
    public float chaseSpeed = 4f; // Speed when chasing the player
    public float attackRange = 1f; // Range within which the enemy can attack
    public int maxHits = 3; // Number of hits to kill the player
    public MazeGenerator mazeGenerator; // Reference to MazeGenerator
    public GameObject enemyPrefab; // Assign the prefab of the enemy
    public float cellSize = 1f; // Match your cell size
    private Transform player; // Reference to the player transform
    private Animator animator;
    private int currentHits = 0; // Counter for hits on the player
    private Vector3 targetPosition; // Target position for roaming
    private bool isChasing = false; // Flag for chasing state
    private float spawnTimer = 30f; // Time between enemy spawns
    private float timeSinceLastSpawn = 0f; // Time since the last enemy spawned
    private float spawnSpeedIncrease = 1f; // Speed increase for each new enemy
    private float currentRoamingSpeed; // Current roaming speed
    private float currentChaseSpeed; // Current chase speed

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentRoamingSpeed = roamingSpeed;
        currentChaseSpeed = chaseSpeed;
        SetRandomRoamingPosition();
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnTimer)
        {
            SpawnNewEnemy();
            timeSinceLastSpawn = 0f;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Roam();
        }
    }

    void SetRandomRoamingPosition()
    {
        Vector2Int randomCell = GetRandomValidCell();
        targetPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);
    }

    void Roam()
    {
        animator.SetBool("isRoaming", true);
        animator.SetBool("isPlayerSpotted", false);
        animator.SetBool("isAttacking", false);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentRoamingSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetRandomRoamingPosition();
        }

        // Check for player proximity
        if (Vector3.Distance(transform.position, player.position) < 5f) // Adjust detection range
        {
            isChasing = true;
        }
    }

    void ChasePlayer()
    {
        animator.SetBool("isRoaming", false);
        animator.SetBool("isPlayerSpotted", true);
        animator.SetBool("isAttacking", false);

        transform.position = Vector3.MoveTowards(transform.position, player.position, currentChaseSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            AttackPlayer();
        }

        if (Vector3.Distance(transform.position, player.position) > 10f) // Adjust distance to stop chasing
        {
            isChasing = false;
            SetRandomRoamingPosition();
        }
    }

    void AttackPlayer()
    {
        animator.SetBool("isAttacking", true);

        // Simulate attack
        currentHits++;
        if (currentHits >= maxHits)
        {
            // Logic for player death
            Debug.Log("Player has been killed!");
            // You can implement player death logic here
        }

        // Reset after attack
        StartCoroutine(WaitBeforeNextAttack());
    }

    IEnumerator WaitBeforeNextAttack()
    {
        yield return new WaitForSeconds(1f); // Wait for a second before allowing another attack
        currentHits = 0; // Reset hit counter after the attack
        animator.SetBool("isAttacking", false);
    }

    Vector2Int GetRandomValidCell()
    {
        if (mazeGenerator == null || mazeGenerator.maze == null)
        {
            Debug.LogError("MazeGenerator or Maze array is not initialized!");
            return Vector2Int.zero; // Or handle the error appropriately
        }

        int width = mazeGenerator.mazeWidth;
        int height = mazeGenerator.mazeHeight;

        Vector2Int randomCell;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            if (attempts >= maxAttempts)
            {
                Debug.LogError("Could not find a valid cell after " + maxAttempts + " attempts.");
                return Vector2Int.zero;
            }

            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            randomCell = new Vector2Int(x, y);
            attempts++;
        } while (mazeGenerator.maze[randomCell.x, randomCell.y] == null); // Ensure the cell isn't null

        return randomCell;
    }

    void SpawnNewEnemy()
    {
        // Here you would instantiate a new enemy prefab and set its speed
        Vector2Int randomCell = GetRandomValidCell();
        Vector3 spawnPosition = new Vector3(randomCell.x * cellSize + cellSize / 2f, 0.5f, randomCell.y * cellSize + cellSize / 2f);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        EnemyAI enemyAI = newEnemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.mazeGenerator = mazeGenerator;
            enemyAI.cellSize = cellSize;
            enemyAI.roamingSpeed = roamingSpeed + spawnSpeedIncrease;
            enemyAI.chaseSpeed = chaseSpeed + spawnSpeedIncrease;
            spawnSpeedIncrease += 1f;
        }
        else
        {
            Debug.LogError("EnemyAI script not found on prefab!");
            Destroy(newEnemy); // Destroy the instance if the script is missing
        }
    }
}

    

