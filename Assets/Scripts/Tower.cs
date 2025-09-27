using UnityEngine; // Подключаем основные функции Unity

public class Tower : MonoBehaviour // Наследуем от MonoBehaviour чтобы скрипт работал в Unity
{
    // === НАСТРОЙКИ БАШНИ ===
    [Header("Настройки башни")] // Заголовок в инспекторе
    public float attackRange = 5f;    // Радиус, в котором башня видит врагов
    public float attackRate = 2f;     // Сколько раз в секунду башня стреляет (1/2 = каждые 0.5 сек)

    // === ССЫЛКИ НА ОБЪЕКТЫ ===
    [Header("Ссылки на объекты")]
    public Transform firePoint;       // Место, откуда вылетают снаряды

    // === ПЕРЕМЕННЫЕ ДЛЯ РАБОТЫ ===
    private float lastAttackTime;     // Время последней атаки (запоминаем)
    private Transform currentTarget;  // Текущий враг, в которого стреляем

    // === ОСНОВНОЙ ЦИКЛ ===
    // Update вызывается КАЖДЫЙ КАДР (60 раз в секунду)
    void Update()
    {
        // 1. Ищем врага в радиусе атаки
        FindTarget();

        // 2. Если есть цель и прошло достаточно времени с последней атаки
        if (currentTarget != null && CanAttack())
        {
            // 3. Атакуем врага
            Attack();
            // 4. Запоминаем время атаки
            lastAttackTime = Time.time;
        }
    }

    // === ПОИСК ВРАГА ===
    void FindTarget()
    {
        // Сбрасываем цель, чтобы искать заново
        currentTarget = null;

        // Ищем ВСЕ коллайдеры в радиусе атаки
        Collider[] allColliders = Physics.OverlapSphere(transform.position, attackRange);

        // Перебираем все найденные коллайдеры по очереди
        foreach (Collider collider in allColliders)
        {
            // Проверяем тег объекта - если это "Enemy", то это враг
            if (collider.CompareTag("Enemy"))
            {
                // Нашли врага! Запоминаем его и выходим из цикла
                currentTarget = collider.transform;
                break; // Прерываем цикл - нам нужен только один враг
            }
        }
    }

    // === ПРОВЕРКА МОЖНО ЛИ АТАКОВАТЬ ===
    bool CanAttack()
    {
        // Time.time - текущее время игры в секундах
        // lastAttackTime - время последней атаки
        // attackRate - скорость атаки (например, 2 раза в секунду)

        // Вычисляем: текущее время >= время последней атаки + интервал между атаками
        return Time.time >= lastAttackTime + (1f / attackRate);
    }

    // === АТАКА ===
    void Attack()
    {
        // Выводим сообщение в консоль Unity
        Debug.Log("🎯 Башня стреляет во врага: " + currentTarget.name);

        // Здесь потом будет:
        // 1. Создание снаряда
        // 2. Вращение башни к цели
        // 3. Анимация выстрела
    }

    // === ВИЗУАЛИЗАЦИЯ В РЕДАКТОРЕ ===
    // Этот метод рисует вспомогательные линии ТОЛЬКО в редакторе Unity
    void OnDrawGizmosSelected()
    {
        // Устанавливаем красный цвет
        Gizmos.color = Color.red;
        // Рисуем прозрачную сферу радиуса attackRange
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Если есть цель, рисуем линию к ней
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, currentTarget.position);
        }
    }
}