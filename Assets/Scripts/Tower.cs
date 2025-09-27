using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Настройки башни")]
    public float attackRange = 10f;    // Радиус атаки
    public float attackRate = 1f;     // Скорость атаки (выстрелов в секунду)

    [Header("Ссылки")]
    public Transform firePoint;       // Точка выстрела
    public GameObject projectilePrefab; // Префаб снаряда

    private float lastAttackTime;     // Время последней атаки
    private Transform currentTarget;  // Текущая цель

    void Update()
    {
        // Ищем цель
        FindTarget();

        // Если есть цель и можно атаковать - атакуем
        if (currentTarget != null && CanAttack())
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void FindTarget()
    {
        // Сбрасываем текущую цель
        currentTarget = null;

        // Ищем все коллайдеры в радиусе атаки
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, attackRange);

        // Перебираем все найденные коллайдеры
        foreach (Collider collider in collidersInRange)
        {
            // Проверяем тег "Enemy"
            if (collider.CompareTag("Enemy"))
            {
                // Нашли врага - запоминаем и выходим из цикла
                currentTarget = collider.transform;
                break;
            }
        }
    }

    bool CanAttack()
    {
        // Проверяем, прошло ли достаточно времени с последней атаки
        return Time.time >= lastAttackTime + (1f / attackRate);
    }

    void Attack()
    {
        Debug.Log("🏹 Башня атакует: " + currentTarget.name);

        // Проверяем все необходимые компоненты
        if (projectilePrefab == null)
        {
            Debug.LogError("❌ Не назначен projectilePrefab!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("❌ Не назначен firePoint!");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogError("❌ Нет цели для атаки!");
            return;
        }

        // Создаем снаряд
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Получаем компонент Projectile у снаряда
        Projectile projectileComponent = newProjectile.GetComponent<Projectile>();

        if (projectileComponent != null)
        {
            // Передаем цель снаряду
            projectileComponent.SetTarget(currentTarget);
            Debug.Log("➡️ Снаряд летит к: " + currentTarget.name);
        }
        else
        {
            Debug.LogError("❌ У снаряда нет компонента Projectile!");
        }
    }

    // Визуализация радиуса атаки в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Дополнительная визуализация - полупрозрачная сфера
        Gizmos.color = new Color(1, 0, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}