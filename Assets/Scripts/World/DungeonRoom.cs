using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownSurvival.World
{
    /// <summary>
    /// Manages an individual Dungeon Room.
    /// Locks doors when player enters, spawns enemies, and unlocks doors when all enemies are defeated.
    /// </summary>
    public class DungeonRoom : MonoBehaviour
    {
        [Header("Room Trigger Settings")]
        [Tooltip("Trigger Collider để phát hiện khi Player đi vào phòng")]
        [SerializeField] private Collider2D roomTrigger;

        [Header("Doors")]
        [Tooltip("Các GameObject Cửa (Tilemap/Prefab Tường chặn) của phòng này")]
        [SerializeField] private GameObject[] doors;

        [Header("Enemy Spawning")]
        [Tooltip("Các Prefabs Quái muốn spawn trong phòng này")]
        [SerializeField] private GameObject[] enemyPrefabs;

        [Tooltip("Các điểm Spawn vị trí quái trong phòng (Transforms)")]
        [SerializeField] private Transform[] spawnPoints;

        [Tooltip("Nếu không kéo spawnPoints, có thể đặt số lượng quái để spawn ngẫu nhiên xung quanh tâm phòng")]
        [SerializeField] private int enemyCountToSpawn = 3;

        [Tooltip("Hệ số phóng to Kích thước (Scale) của quái khi spawn trong phòng này (VD: 1.5 hoặc 2)")]
        [SerializeField] private float enemyScaleMultiplier = 4f;

        private List<GameObject> spawnedEnemies = new List<GameObject>();
        private bool isCleared = false;
        private bool isRoomActive = false;

        private void Awake()
        {
            if (roomTrigger == null)
            {
                // Chỉ lấy Collider2D gắn trên chính GameObject này (Room_1)
                Collider2D[] colliders = GetComponents<Collider2D>();
                foreach (var col in colliders)
                {
                    if (col.isTrigger)
                    {
                        roomTrigger = col;
                        break;
                    }
                }
                if (roomTrigger == null && colliders.Length > 0) roomTrigger = colliders[0];
            }

            // Đảm bảo ban đầu cửa mở
            SetDoorsActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isCleared || isRoomActive) return;

            if (other.CompareTag("Player"))
            {
                StartDungeonChallenge();
            }
        }

        private void Update()
        {
            if (!isRoomActive || isCleared) return;

            // Remove null entries (enemies that were destroyed)
            spawnedEnemies.RemoveAll(enemy => enemy == null);

            if (spawnedEnemies.Count == 0)
            {
                OnRoomCleared();
            }
        }

        public void StartDungeonChallenge()
        {
            isRoomActive = true;

            // Tắt EnemySpawner ngoài map khi vào phòng Dungeon
            EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
            if (spawner != null) spawner.SetSpawnerEnabled(false);

            // 1. Đóng cửa
            SetDoorsActive(true);

            // 2. Spawn quái
            SpawnEnemies();

            Debug.Log($"[DungeonRoom] Phòng '{gameObject.name}' đã KHÓA CỬA! Đã spawn {spawnedEnemies.Count} quái.");
        }

        private void SpawnEnemies()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                foreach (Transform sp in spawnPoints)
                {
                    if (sp == null) continue;
                    GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                    GameObject enemy = Instantiate(prefab, sp.position, Quaternion.identity);
                    ApplyScaleMultiplier(enemy);
                    spawnedEnemies.Add(enemy);
                }
            }
            else
            {
                // Ngẫu nhiên bên trong vùng lòng phòng (Bounds của Trigger)
                Bounds bounds = roomTrigger != null ? roomTrigger.bounds : new Bounds(transform.position, Vector3.one * 4f);
                for (int i = 0; i < enemyCountToSpawn; i++)
                {
                    float rx = Random.Range(bounds.min.x + 1f, bounds.max.x - 1f);
                    float ry = Random.Range(bounds.min.y + 1f, bounds.max.y - 1f);
                    Vector3 spawnPos = new Vector3(rx, ry, 0f);

                    GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                    GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
                    ApplyScaleMultiplier(enemy);
                    spawnedEnemies.Add(enemy);
                }
            }
        }

        private void ApplyScaleMultiplier(GameObject enemy)
        {
            if (enemy != null && enemyScaleMultiplier > 0f)
            {
                enemy.transform.localScale = enemy.transform.localScale * enemyScaleMultiplier;
            }
        }

        private void OnRoomCleared()
        {
            isCleared = true;
            isRoomActive = false;

            // Bật lại EnemySpawner ngoài map khi diệt hết quái trong phòng
            EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
            if (spawner != null) spawner.SetSpawnerEnabled(true);

            // Mở lại cửa
            SetDoorsActive(false);

            Debug.Log($"[DungeonRoom] Phòng '{gameObject.name}' đã DIỆT HẾT QUÁI! ĐÃ MỞ CỬA.");
        }

        private void SetDoorsActive(bool active)
        {
            if (doors == null) return;
            foreach (GameObject door in doors)
            {
                if (door != null)
                {
                    door.SetActive(active);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isCleared ? Color.green : (isRoomActive ? Color.red : Color.yellow);
            if (spawnPoints != null)
            {
                foreach (var sp in spawnPoints)
                {
                    if (sp != null) Gizmos.DrawSphere(sp.position, 0.3f);
                }
            }
        }
    }
}
