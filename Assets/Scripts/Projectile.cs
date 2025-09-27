using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Настройки снаряда")]
    public float speed = 10f;         // Скорость полета
    public int damage = 20;           // Урон врагу

    private Transform target;         // Цель для попадания

    void Update()
    {
        // Если цель уничтожена - уничтожаем снаряд
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Вычисляем направление к цели
        Vector3 direction = (target.position - transform.position).normalized;

        // Двигаем снаряд
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Поворачиваем снаряд в направлении движения
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Проверяем достигли ли цели (дистанция меньше 0.3f)
        if (Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            HitTarget();
        }
    }

    // Метод для установки цели
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Попадание в цель
    void HitTarget()
    {
        // Наносим урон врагу
        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Уничтожаем снаряд
        Destroy(gameObject);
    }
}