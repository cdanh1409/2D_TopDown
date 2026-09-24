using UnityEngine;

namespace TopDownSurvival.World
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private bool enableSpawner = true;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float spawnInterval = 8f;
        [SerializeField] private int maxEnemies = 6;
        [SerializeField] private float minSpawnDistance = 5f;
        [SerializeField] private float maxSpawnDistance = 12f;

        private Transform playerTransform;
        private float nextSpawnTime = 0f;
        private Camera mainCam;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;

            mainCam = Camera.main;

            // First spawn after a short delay
            nextSpawnTime = Time.time + 3f;
        }

        public void SetSpawnerEnabled(bool enabled)
        {
            enableSpawner = enabled;
        }

        private void Update()
        {
            if (!enableSpawner) return;
            if (playerTransform == null || enemyPrefabs == null || enemyPrefabs.Length == 0) return;
            if (Time.time < nextSpawnTime) return;

            nextSpawnTime = Time.time + spawnInterval;

            // Count alive enemies
            int aliveCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (aliveCount >= maxEnemies) return;

            TrySpawnEnemy();
        }

        private void TrySpawnEnemy()
        {
            const int maxAttempts = 10;
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
                Vector3 spawnPos = playerTransform.position + new Vector3(randomDir.x * dist, randomDir.y * dist, 0f);

                // Don't spawn on-screen
                if (mainCam != null)
                {
                    Vector3 viewportPos = mainCam.WorldToViewportPoint(spawnPos);
                    bool onScreen = viewportPos.x >= 0f && viewportPos.x <= 1f &&
                                    viewportPos.y >= 0f && viewportPos.y <= 1f &&
                                    viewportPos.z > 0f;
                    if (onScreen) continue;
                }

                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Instantiate(prefab, spawnPos, Quaternion.identity);
                Debug.Log($"[EnemySpawner] Spawned {prefab.name} at {spawnPos}");
                return;
            }
        }
    }
}
