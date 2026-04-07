using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Бесконечная генерация уровня по мере движения игрока.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] platformPrefabs; // 0 — обычная платформа, 1 — препятствие

    [Header("Materials")]
    public Material[] basicPlatformMaterials; // Материалы для обычных платформ
    public Material[] obstacleMaterials;      // Материалы для препятствий

    [Header("Generation Settings")]
    public float stepDistance = 2.5f;           // Расстояние между центрами платформ по оси Z
    public float xOffsetRange = 1.5f;           // Максимальное случайное смещение по оси X
    public float generationThreshold = 5f;      // Дистанция до игрока, при которой создаётся новая платформа
    public float removeThreshold = 15f;         // Дистанция позади игрока, после которой платформа удаляется
    public float obstacleChance = 0.3f;         // Вероятность появления препятствия (0.0–1.0)

    [Header("References")]
    public Transform player;                    // Ссылка на трансформ игрока для отслеживания позиции

    private List<GameObject> spawnedPlatforms = new List<GameObject>();

    private void Start()
    {
        // Проверка наличия ссылки на игрока
        if (player == null)
        {
            Debug.LogError("Player reference not set! Assign player in Inspector.");
            return;
        }

        // Проверка наличия префабов
        if (platformPrefabs == null || platformPrefabs.Length < 2)
        {
            Debug.LogError("Need at least 2 platform prefabs (basic and obstacle)!");
            return;
        }

        // Генерация начального сегмента уровня (5 платформ) для старта игрока
        for (int i = 0; i < 5; i++)
        {
            GenerateNextPlatform();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float playerZ = player.position.z;

        // Получение Z-координаты последней сгенерированной платформы
        float lastPlatformZ = spawnedPlatforms[spawnedPlatforms.Count - 1].transform.position.z;

        // Генерация новой платформы, если игрок приблизился к краю текущего уровня
        if (lastPlatformZ - playerZ < generationThreshold)
        {
            GenerateNextPlatform();
        }

        // Удаление платформ, которые остались далеко позади игрока, для оптимизации памяти
        if (spawnedPlatforms.Count > 0 &&
            spawnedPlatforms[0].transform.position.z < playerZ - removeThreshold)
        {
            Destroy(spawnedPlatforms[0]);
            spawnedPlatforms.RemoveAt(0);
        }
    }

    /// <summary>
    /// Создаёт одну платформу и добавляет её в конец списка.
    /// </summary>
    private void GenerateNextPlatform()
    {
        // Вычисление позиции Z для новой платформы
        float zPos = (spawnedPlatforms.Count == 0) ? 0f :
                     spawnedPlatforms[spawnedPlatforms.Count - 1].transform.position.z + stepDistance;

        // Случайное смещение по оси X в заданном диапазоне для вариативности трассы
        float xOffset = Random.Range(-xOffsetRange, xOffsetRange);

        Vector3 position = new Vector3(xOffset, 0f, zPos);

        // Выбор типа платформы: обычная или препятствие, на основе вероятности
        int prefabIndex = Random.value < obstacleChance ? 1 : 0;

        // Создание экземпляра платформы в рассчитанной позиции
        GameObject newPlatform = Instantiate(platformPrefabs[prefabIndex], position, Quaternion.identity);

        // Назначение случайного материала в зависимости от типа платформы
        ApplyRandomMaterial(newPlatform, prefabIndex);

        spawnedPlatforms.Add(newPlatform);
    }

    /// <summary>
    /// Назначает случайный материал платформе в зависимости от её типа.
    /// </summary>
    /// <param name="platform">Созданная платформа</param>
    /// <param name="typeIndex">Тип платформы (0 — обычная, 1 — препятствие)</param>
    private void ApplyRandomMaterial(GameObject platform, int typeIndex)
    {
        Renderer renderer = platform.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Platform has no Renderer component!");
            return;
        }

        Material selectedMaterial = null;

        // Выбор массива материалов в зависимости от типа платформы
        if (typeIndex == 0)
        {
            // Обычная платформа — выбираем из basicPlatformMaterials
            if (basicPlatformMaterials != null && basicPlatformMaterials.Length > 0)
            {
                int randomIndex = Random.Range(0, basicPlatformMaterials.Length);
                selectedMaterial = basicPlatformMaterials[randomIndex];
            }
        }
        else if (typeIndex == 1)
        {
            // Препятствие — выбираем из obstacleMaterials
            if (obstacleMaterials != null && obstacleMaterials.Length > 0)
            {
                int randomIndex = Random.Range(0, obstacleMaterials.Length);
                selectedMaterial = obstacleMaterials[randomIndex];
            }
        }

        // Применение материала
        if (selectedMaterial != null)
        {
            renderer.material = selectedMaterial;
        }
    }
}