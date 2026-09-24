using UnityEngine;
using UnityEngine.Tilemaps;

namespace TopDownSurvival.World
{
    /// <summary>
    /// Bulletproof Map Lock System (LockMap).
    /// Prevents player from leaving the map boundaries using both physics walls and position clamping.
    /// </summary>
    public class MapBoundsLock : MonoBehaviour
    {
        [Header("Tilemap Reference")]
        [Tooltip("Kéo Tilemap mặt đất vào đây để tự động lấy viền bản đồ")]
        [SerializeField] private Tilemap groundTilemap;

        [Header("Manual Bounds (Nếu không dùng Tilemap)")]
        [SerializeField] private bool useManualBounds = false;
        [SerializeField] private Vector2 manualMin = new Vector2(-20f, -15f);
        [SerializeField] private Vector2 manualMax = new Vector2(20f, 15f);

        [Header("Lock Options")]
        [Tooltip("Độ dày bức tường cản ngoài viền bản đồ")]
        [SerializeField] private float wallThickness = 4f;

        [Tooltip("Tick chọn để tự động giữ chặt vị trí Player trong viền map")]
        [SerializeField] private bool clampPlayerPosition = true;

        private float minX, maxX, minY, maxY;
        private Transform playerTransform;
        private bool isInitialized = false;

        private void Start()
        {
            InitializeMapBounds();
            CreatePhysicsWalls();
            FindPlayer();
        }

        private void LateUpdate()
        {
            if (clampPlayerPosition && isInitialized)
            {
                if (playerTransform == null)
                {
                    FindPlayer();
                }

                if (playerTransform != null)
                {
                    Vector3 pos = playerTransform.position;
                    float clampedX = Mathf.Clamp(pos.x, minX, maxX);
                    float clampedY = Mathf.Clamp(pos.y, minY, maxY);

                    if (pos.x != clampedX || pos.y != clampedY)
                    {
                        playerTransform.position = new Vector3(clampedX, clampedY, pos.z);
                    }
                }
            }
        }

        public void InitializeMapBounds()
        {
            if (groundTilemap != null && !useManualBounds)
            {
                groundTilemap.CompressBounds();
                BoundsInt cellBounds = groundTilemap.cellBounds;

                Vector3 minWorld = groundTilemap.CellToWorld(cellBounds.min);
                Vector3 maxWorld = groundTilemap.CellToWorld(cellBounds.max);

                minX = minWorld.x;
                maxX = maxWorld.x;
                minY = minWorld.y;
                maxY = maxWorld.y;
            }
            else
            {
                minX = manualMin.x;
                maxX = manualMax.x;
                minY = manualMin.y;
                maxY = manualMax.y;
            }

            isInitialized = true;
            Debug.Log($"[MapBoundsLock] Map Bounds initialized -> Min: ({minX}, {minY}) | Max: ({maxX}, {maxY})");
        }

        private void CreatePhysicsWalls()
        {
            // Clean up existing wall children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            float width = maxX - minX;
            float height = maxY - minY;
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0);

            // 1. Top Wall
            CreateWall("Wall_Top", new Vector3(center.x, maxY + wallThickness / 2f, 0), new Vector2(width + wallThickness * 2, wallThickness));

            // 2. Bottom Wall
            CreateWall("Wall_Bottom", new Vector3(center.x, minY - wallThickness / 2f, 0), new Vector2(width + wallThickness * 2, wallThickness));

            // 3. Left Wall
            CreateWall("Wall_Left", new Vector3(minX - wallThickness / 2f, center.y, 0), new Vector2(wallThickness, height + wallThickness * 2));

            // 4. Right Wall
            CreateWall("Wall_Right", new Vector3(maxX + wallThickness / 2f, center.y, 0), new Vector2(wallThickness, height + wallThickness * 2));
        }

        private void CreateWall(string wallName, Vector3 pos, Vector2 size)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.parent = transform;
            wall.transform.position = pos;

            BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
            col.size = size;
            col.isTrigger = false;
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Player.PlayerController pc = FindAnyObjectByType<Player.PlayerController>();
                if (pc != null) playerTransform = pc.transform;
            }
        }

        private void OnDrawGizmosSelected()
        {
            InitializeMapBounds();
            Gizmos.color = Color.green;
            Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 1);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
