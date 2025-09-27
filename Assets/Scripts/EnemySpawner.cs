using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        [Header("Настройки волны")]
        public string waveName = "Wave 1";        // Название волны (для удобства)
        public GameObject enemyPrefab;            // Префаб врага для спауна
        public int enemyCount = 5;                // Количество врагов в волне
        public float spawnInterval = 2f;          // Интервал между спауном врагов (в секундах)
        public float waveDelay = 5f;              // Задержка перед началом волны (в секундах)
    }

    [Header("Настройки спаунера")]
    public Wave[] waves;                          // Массив волн
    public Transform spawnPoint;                  // Точка спауна врагов
    public bool autoStart = true;                 // Автоматически начинать при старте игры

    [Header("Текущая информация")]
    public int currentWaveIndex = 0;              // Индекс текущей волны
    public int enemiesAlive = 0;                  // Количество живых врагов
    public bool isSpawning = false;               // Идет ли процесс спауна

    void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    // Начать спаун волн
    public void StartSpawning()
    {
        if (!isSpawning && waves.Length > 0)
        {
            StartCoroutine(SpawnWaves());
        }
    }

    // Остановить спаун
    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    // Процесс спауна волн
    IEnumerator SpawnWaves()
    {
        isSpawning = true;

        // Проходим по всем волнам
        for (currentWaveIndex = 0; currentWaveIndex < waves.Length; currentWaveIndex++)
        {
            Wave currentWave = waves[currentWaveIndex];

            Debug.Log($"?? Начинается волна: {currentWave.waveName}");

            // Ждем перед началом волны
            yield return new WaitForSeconds(currentWave.waveDelay);

            // Спауним врагов текущей волны
            yield return StartCoroutine(SpawnWave(currentWave));

            // Ждем пока все враги не умрут или не дойдут до конца
            yield return new WaitWhile(() => enemiesAlive > 0);

            Debug.Log($"? Волна {currentWave.waveName} завершена!");
        }

        Debug.Log("?? Все волны завершены!");
        isSpawning = false;
    }

    // Спаун одной волны
    IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    // Создание одного врага
    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("? Не назначена точка спауна!");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("? Не назначен префаб врага!");
            return;
        }

        // Создаем врага
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemiesAlive++;

        // Подписываемся на событие смерти врага
        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            // Создаем временный скрипт для отслеживания смерти
            EnemyTracker tracker = newEnemy.AddComponent<EnemyTracker>();
            tracker.Initialize(this);
        }

        Debug.Log($"?? Заспаунен враг! Всего живых: {enemiesAlive}");
    }

    // Метод для уменьшения счетчика живых врагов
    public void OnEnemyDied()
    {
        enemiesAlive--;
        enemiesAlive = Mathf.Max(0, enemiesAlive); // Не даем уйти ниже 0
        Debug.Log($"?? Враг умер! Осталось живых: {enemiesAlive}");
    }

    // Метод для ручного запуска волны по индексу
    public void StartWave(int waveIndex)
    {
        if (waveIndex >= 0 && waveIndex < waves.Length)
        {
            StopSpawning();
            currentWaveIndex = waveIndex;
            StartCoroutine(SpawnSingleWave(waveIndex));
        }
    }

    // Запуск одной конкретной волны
    IEnumerator SpawnSingleWave(int waveIndex)
    {
        isSpawning = true;
        Wave wave = waves[waveIndex];

        yield return StartCoroutine(SpawnWave(wave));
        yield return new WaitWhile(() => enemiesAlive > 0);

        isSpawning = false;
    }

    // Визуализация точки спауна в редакторе - ИСПРАВЛЕННЫЙ МЕТОД
    void OnDrawGizmos()
    {
        // Проверяем что spawnPoint не null
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnPoint.position, Vector3.one);

            // Рисуем стрелку направления
            Gizmos.DrawRay(spawnPoint.position, spawnPoint.forward * 2f);
        }
    }
}

// Вспомогательный класс для отслеживания смерти врага
public class EnemyTracker : MonoBehaviour
{
    private EnemySpawner spawner;
    private Enemy enemy;

    public void Initialize(EnemySpawner spawnerRef)
    {
        spawner = spawnerRef;
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        // Если враг уничтожен (дошел до конца или убит)
        if (enemy == null || !enemy.gameObject.activeInHierarchy)
        {
            if (spawner != null)
            {
                spawner.OnEnemyDied();
            }
            Destroy(this); // Удаляем трекер
        }
    }
}