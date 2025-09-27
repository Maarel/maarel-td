using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Настройки врага")]
    public float speed = 3f;
    public int maxHealth = 50;

    [Header("Эффекты смерти")]
    public Material deadMaterial;
    public float destroyDelay = 3f;

    private int currentHealth;
    private bool isDead = false;
    private Rigidbody rb;
    private Collider enemyCollider;
    private Renderer enemyRenderer;

    // Переменные для движения по пути
    private Vector3[] pathPoints;
    private int currentPathIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;

        // Получаем компоненты
        rb = GetComponent<Rigidbody>();
        enemyCollider = GetComponent<Collider>();
        enemyRenderer = GetComponent<Renderer>();

        // Находим путь
        FindPath();

        // Начинаем с первой точки пути
        if (pathPoints != null && pathPoints.Length > 0)
        {
            transform.position = pathPoints[0];
        }
    }

    void Update()
    {
        if (isDead) return;

        MoveAlongPath();
    }

    void FindPath()
    {
        PathManager pathManager = FindObjectOfType<PathManager>();
        if (pathManager != null)
        {
            pathPoints = pathManager.GetPathPoints();
        }
    }

    void MoveAlongPath()
    {
        if (pathPoints == null || pathPoints.Length == 0) return;

        if (currentPathIndex >= pathPoints.Length) return;

        Vector3 targetPoint = pathPoints[currentPathIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint,
            speed * Time.deltaTime
        );

        if (targetPoint != transform.position)
        {
            transform.LookAt(targetPoint);
        }

        float distanceToPoint = Vector3.Distance(transform.position, targetPoint);
        if (distanceToPoint < 0.1f)
        {
            currentPathIndex++;
            if (currentPathIndex >= pathPoints.Length)
            {
                ReachedEnd();
            }
        }
    }

    void ReachedEnd()
    {
        Debug.Log("?? Враг достиг конца пути!");

        // Уведомляем спаунер о том, что враг уничтожен
        EnemyTracker tracker = GetComponent<EnemyTracker>();
        if (tracker != null)
        {
            // Трекер сам вызовет OnEnemyDied()
        }

        Destroy(gameObject);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"?? Враг получил {damageAmount} урона. Осталось HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("?? Враг умер!");

        // ВЫКЛЮЧАЕМ СКРИПТ ДВИЖЕНИЯ ПЕРВЫМ ДЕЛОМ!
        enabled = false; // Это отключает Update() и движение по пути

        EnablePhysics();
        ChangeToDeadColor();
        gameObject.tag = "Untagged";

        Destroy(gameObject, destroyDelay);
    }

    void EnablePhysics()
    {
        // Если Rigidbody нет - создаем
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("? Добавлен Rigidbody");
        }

        // ВАЖНО: выключаем kinematic и включаем гравитацию
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 1f;

        Debug.Log("?? Физика включена: Kinematic=" + rb.isKinematic + ", Gravity=" + rb.useGravity);

        // Добавляем случайное вращение для эффекта
        rb.angularVelocity = new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f)
        );

        // Небольшый толчок вперед для эффекта падения
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
    }

    void ChangeToDeadColor()
    {
        if (deadMaterial != null)
        {
            enemyRenderer.material = deadMaterial;
        }
        else
        {
            enemyRenderer.material.color = Color.gray;
        }
    }

    // Добавим для отладки
    void OnDrawGizmos()
    {
        if (isDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}