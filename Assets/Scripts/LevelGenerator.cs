using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Бесконечная генерация уровня по мере движения игрока.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] platformPrefabs; // 0 — обычная, 1 — препятствие

    [Header("Generation Settings")]
    public float stepDistance = 2.5f;           // Расстояние между платформами
    public float generationThreshold = 5f;      // Дистанция, при которой создаётся новая платформа
    public float removeThreshold = 15f;         // Дистанция, после которой платформа удаляется
    public float obstacleChance = 0.3f;         // Шанс препятствия (30%)

    [Header("References")]
    public Transform player;                    // Ссылка на игрока

    private List<GameObject> spawnedPlatforms = new List<GameObject>();
    private float lastGeneratedZ;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set!");
            return;
        }

        // Генерируем первые 5 платформ при старте, чтобы игроку было куда идти
        for (int i = 0; i < 5; i++)
        {
            GenerateNextPlatform();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float playerZ = player.position.z;

        // Получаем Z-координату последней платформы
        float lastPlatformZ = spawnedPlatforms[spawnedPlatforms.Count - 1].transform.position.z;

        // 1. Генерация новой платформы, если игрок приблизился к краю
        if (lastPlatformZ - playerZ < generationThreshold)
        {
            GenerateNextPlatform();
        }

        // 2. Удаление платформ, оставшихся далеко позади
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
        // Вычисляем позицию Z для новой платформы
        float zPos = (spawnedPlatforms.Count == 0) ? 0f :
                     spawnedPlatforms[spawnedPlatforms.Count - 1].transform.position.z + stepDistance;

        Vector3 position = new Vector3(0f, 0f, zPos);

        // Выбор типа платформы (30% шанс на препятствие)
        int prefabIndex = Random.value < obstacleChance ? 1 : 0;

        // Создаём платформу
        GameObject newPlatform = Instantiate(platformPrefabs[prefabIndex], position, Quaternion.identity);
        spawnedPlatforms.Add(newPlatform);
    }
}