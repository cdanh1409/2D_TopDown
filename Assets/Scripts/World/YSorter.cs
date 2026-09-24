using UnityEngine;

namespace TopDownSurvival.World
{
    /// <summary>
    /// Dynamic Y-Sorting helper for 2D Top-Down perspective.
    /// Ensures Player gets covered when walking behind Trees/Rocks/Buildings.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class YSorter : MonoBehaviour
    {
        [Header("Sorting Settings")]
        [Tooltip("Base sorting order to offset calculations")]
        [SerializeField] private int sortingOrderBase = 5000;

        [Tooltip("Vertical offset for sorting point (e.g. at the feet/gost of object)")]
        [SerializeField] private float sortingOffset = 0f;

        [Tooltip("If true, calculates sorting order once on Start (for static objects like Trees/Rocks)")]
        [SerializeField] private bool isStatic = false;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            UpdateSortingOrder();
        }

        private void Start()
        {
            if (isStatic)
            {
                UpdateSortingOrder();
                enabled = false; // Disable update loop for static environmental objects
            }
        }

        private void LateUpdate()
        {
            if (!isStatic)
            {
                UpdateSortingOrder();
            }
        }

        public void UpdateSortingOrder()
        {
            if (spriteRenderer != null)
            {
                // Lower Y position (lower on screen) gets higher sorting order (drawn on top)
                float sortingY = transform.position.y + sortingOffset;
                spriteRenderer.sortingOrder = (int)(sortingOrderBase - (sortingY * 100));
            }
        }
    }
}
