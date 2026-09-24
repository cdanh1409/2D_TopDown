using System.Collections;
using UnityEngine;
using TopDownSurvival.Core;
using TopDownSurvival.Player;

namespace TopDownSurvival.World
{
    public class ResourceObject : MonoBehaviour, IInteractable
    {
        [Header("Resource Settings")]
        [SerializeField] private string resourceName = "Tree";
        [SerializeField] private int maxHitPoints = 3;
        [SerializeField] private ItemData itemDrop;
        [SerializeField] private int minDropAmount = 1;
        [SerializeField] private int maxDropAmount = 3;

        private int currentHitPoints;
        private bool isDestroyed = false;

        private void Start()
        {
            currentHitPoints = maxHitPoints;
        }

        public string GetInteractionPrompt()
        {
            return $"Harvest {resourceName}";
        }

        public void Interact(GameObject actor)
        {
            if (isDestroyed) return;

            // Warrior perk: deal 2 damage per hit
            int damage = 1;
            PlayerRoleManager roleManager = actor.GetComponent<PlayerRoleManager>();
            if (roleManager != null && roleManager.IsWarrior())
            {
                damage = 2;
            }

            currentHitPoints -= damage;
            Debug.Log($"[Harvest] {gameObject.name} hit for {damage}! HP remaining: {currentHitPoints}/{maxHitPoints}");

            // Visual feedback: Shake
            StartCoroutine(ShakeEffect());

            if (currentHitPoints <= 0)
            {
                HarvestComplete();
            }
        }

        private void HarvestComplete()
        {
            isDestroyed = true;
            int dropAmount = Random.Range(minDropAmount, maxDropAmount + 1);

            Debug.Log($"[Harvest] {gameObject.name} harvested! Spawning {dropAmount}x {itemDrop.displayName}");

            // Spawn drops
            for (int i = 0; i < dropAmount; i++)
            {
                SpawnDrop();
            }

            // Destroy resource object
            Destroy(gameObject);
        }

        private void SpawnDrop()
        {
            if (itemDrop == null) return;

            // Create drop object dynamically
            GameObject drop = new GameObject($"Drop_{itemDrop.displayName}");
            
            // Random offset and spawn position
            Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            drop.transform.position = transform.position + randomOffset;

            // Add sprite
            SpriteRenderer sr = drop.AddComponent<SpriteRenderer>();
            sr.sprite = itemDrop.icon;
            // Draw order
            sr.sortingOrder = 5;

            // Add physics collider
            BoxCollider2D col = drop.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            // Add WorldItem logic
            WorldItem worldItem = drop.AddComponent<WorldItem>();
            worldItem.Setup(itemDrop, 1);
        }

        private IEnumerator ShakeEffect()
        {
            Vector3 originalPosition = transform.position;
            float elapsed = 0f;
            float duration = 0.15f;
            float magnitude = 0.1f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPosition;
        }
    }
}
