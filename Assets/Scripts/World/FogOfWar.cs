using UnityEngine;
using UnityEngine.Tilemaps;

namespace TopDownSurvival.World
{
    public class FogOfWar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Tilemap fogTilemap;
        [SerializeField] private TileBase fogTile; // Black or semi-transparent tile placeholder
        [SerializeField] private Transform playerTransform;

        [Header("Settings")]
        [SerializeField] private int revealRadius = 5;
        [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);
        [SerializeField] private Vector2Int mapOffset = new Vector2Int(-50, -50);

        private Vector3Int lastPlayerCell;

        private void Start()
        {
            if (fogTilemap == null)
            {
                fogTilemap = GetComponent<Tilemap>();
            }

            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            // Tự động tạo Tile đen mặc định nếu người dùng chưa gán Tile trong Inspector
            if (fogTile == null)
            {
                CreateFallbackFogTile();
            }

            InitializeFog();
        }

        private void CreateFallbackFogTile()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.95f)); // Màu đen mờ 95%
            tex.Apply();

            Sprite fogSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
            Tile defaultTile = ScriptableObject.CreateInstance<Tile>();
            defaultTile.sprite = fogSprite;
            defaultTile.color = Color.black;
            fogTile = defaultTile;
        }

        private void InitializeFog()
        {
            if (fogTilemap == null || fogTile == null)
            {
                Debug.LogWarning("FogOfWar: Tilemap or FogTile is not assigned.");
                return;
            }

            fogTilemap.ClearAllTiles();

            // Fill map with fog
            for (int x = mapOffset.x; x < mapOffset.x + mapSize.x; x++)
            {
                for (int y = mapOffset.y; y < mapOffset.y + mapSize.y; y++)
                {
                    fogTilemap.SetTile(new Vector3Int(x, y, 0), fogTile);
                }
            }

            RevealAroundPlayer();
        }

        private void Update()
        {
            if (playerTransform == null || fogTilemap == null) return;

            Vector3Int currentPlayerCell = fogTilemap.WorldToCell(playerTransform.position);
            if (currentPlayerCell != lastPlayerCell)
            {
                lastPlayerCell = currentPlayerCell;
                RevealAroundPlayer();
            }
        }

        private void RevealAroundPlayer()
        {
            if (fogTilemap == null || playerTransform == null) return;

            Vector3Int centerCell = fogTilemap.WorldToCell(playerTransform.position);

            for (int x = -revealRadius; x <= revealRadius; x++)
            {
                for (int y = -revealRadius; y <= revealRadius; y++)
                {
                    // Check if inside circle radius
                    if (x * x + y * y <= revealRadius * revealRadius)
                    {
                        Vector3Int cellPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);
                        if (fogTilemap.HasTile(cellPos))
                        {
                            // Clear tile to reveal the world map underneath
                            fogTilemap.SetTile(cellPos, null);
                        }
                    }
                }
            }
        }

        public void SetRevealRadius(int newRadius)
        {
            revealRadius = newRadius;
            // Force an immediate reveal at the new radius
            RevealAroundPlayer();
        }
    }
}
