using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float roamingSpeed = 2f;
    public float chaseSpeed = 4f;
    public float attackRange = 1f;
    public float damageInterval = 15f;
    public int damageAmount = 1;
    public MazeGenerator mazeGenerator;
    public GameObject enemyPrefab;
    public float cellSize = 1f;
    private Transform player;
    private Animator animator;
    private Vector3 targetPosition;
    private bool isChasing = false;
    private float timeSinceLastDamage = 0f;
    private float spawnTimer = 30f;
    private float timeSinceLastSpawn = 0f;
    private float spawnSpeedIncrease = 1f;
    private float currentRoamingSpeed;
    private float currentChaseSpeed;

    private bool isAttacking = false; // Flag to prevent continuous damage
    void OnCollisionEnter(Collision collision) // Or OnTriggerEnter(Collider other) if it's a trigger
    {
        if (collision.gameObject.CompareTag("Player")) // Or if (other.CompareTag("Player")) if using OnTriggerEnter
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null && !playerHealth.isDead)
        {
            playerHealth.TakeDamage(damageAmount);
        }
        yield return new WaitForSeconds(damageInterval);
        isAttacking = false;
    }
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

        // Check for player proximity and deal damage
        if (isChasing && Vector3.Distance(transform.position, player.position) < attackRange)
        {
            DealDamageToPlayer();
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

        if (Vector3.Distance(transform.position, player.position) < 5f)
        {
            isChasing = true;
        }
    }

    void ChasePlayer()
    {
        animator.SetBool("isRoaming", false);
        animator.SetBool("isPlayerSpotted", true);
        animator.SetBool("isAttacking", false); // No longer needed for this damage system

        transform.position = Vector3.MoveTowards(transform.position, player.position, currentChaseSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, player.position) > 10f)
        {
            isChasing = false;
            SetRandomRoamingPosition();
        }
    }

    void DealDamageToPlayer()
    {
        timeSinceLastDamage += Time.deltaTime;
        if (timeSinceLastDamage >= damageInterval)
        {
            timeSinceLastDamage = 0f;
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.isDead)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
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

    

