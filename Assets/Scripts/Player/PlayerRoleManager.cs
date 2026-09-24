using UnityEngine;
using UnityEngine.InputSystem;
using TopDownSurvival.Core;
using TopDownSurvival.World;

namespace TopDownSurvival.Player
{
    public class PlayerRoleManager : MonoBehaviour
    {
        [Header("Current Role")]
        [SerializeField] private RoleType currentRole = RoleType.Warrior;

        // Baseline stats (saved at start to allow role switching)
        private float baseSpeed;
        private float baseDamage;
        private int baseFogRadius;

        // Component references
        private PlayerController playerController;
        private PlayerAttacker playerAttacker;
        private SurvivalStats survivalStats;
        private FogOfWar fogOfWar;

        [Header("UX Reference")]
        [SerializeField] private GameObject floatingTextPrefab;


        // Role stat config
        private const float WarriorSpeed = 4f;
        private const float WarriorDamage = 30f;

        private const float RangerSpeed = 7f;
        private const float RangerDamage = 12f;
        private const int RangerFogRadius = 8;

        private const float EngineerSpeed = 5f;
        private const float EngineerDamage = 10f;

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerAttacker = GetComponent<PlayerAttacker>();
            survivalStats = GetComponent<SurvivalStats>();
            fogOfWar = FindAnyObjectByType<FogOfWar>();

            // Store defaults
            if (playerController != null) baseSpeed = playerController.MoveSpeed;
            if (playerAttacker != null) baseDamage = playerAttacker.AttackDamage;
        }

        private void Start()
        {
            // Apply the initial role
            ApplyRole(currentRole);
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                ApplyRole(RoleType.Warrior);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                ApplyRole(RoleType.Ranger);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                ApplyRole(RoleType.Engineer);
            }
        }

        private void ApplyRole(RoleType role)
        {
            currentRole = role;

            Color popColor = Color.white;
            string roleName = "Warrior";

            switch (role)
            {
                case RoleType.Warrior:
                    if (playerController != null) playerController.MoveSpeed = WarriorSpeed;
                    if (playerAttacker != null) playerAttacker.AttackDamage = WarriorDamage;
                    SetFogRadius(5);
                    SetPlayerColor(new Color(0.9f, 0.3f, 0.3f)); // Red tinted
                    popColor = new Color(0.9f, 0.3f, 0.3f);
                    roleName = "Warrior Role";
                    Debug.Log("[Role] Switched to WARRIOR: High damage (30 DMG), double harvest speed.");
                    break;

                case RoleType.Ranger:
                    if (playerController != null) playerController.MoveSpeed = RangerSpeed;
                    if (playerAttacker != null) playerAttacker.AttackDamage = RangerDamage;
                    SetFogRadius(RangerFogRadius);
                    SetPlayerColor(new Color(0.3f, 0.8f, 0.3f)); // Green tinted
                    popColor = new Color(0.3f, 0.8f, 0.3f);
                    roleName = "Ranger Role";
                    Debug.Log("[Role] Switched to RANGER: High speed (7), expanded fog vision (Radius 8).");
                    break;

                case RoleType.Engineer:
                    if (playerController != null) playerController.MoveSpeed = EngineerSpeed;
                    if (playerAttacker != null) playerAttacker.AttackDamage = EngineerDamage;
                    SetFogRadius(5);
                    SetPlayerColor(new Color(0.4f, 0.6f, 1f)); // Blue tinted
                    popColor = new Color(0.4f, 0.6f, 1f);
                    roleName = "Engineer Role";
                    Debug.Log("[Role] Switched to ENGINEER: Half crafting cost. Press C to craft Axe with only 1 Wood!");
                    break;
            }

            // Spawn floating text on switch
            if (floatingTextPrefab != null && Time.timeSinceLevelLoad > 0.5f)
            {
                GameObject ftObj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
                TopDownSurvival.UI.FloatingText ft = ftObj.GetComponent<TopDownSurvival.UI.FloatingText>();
                if (ft != null)
                {
                    ft.SetText(roleName, popColor);
                }
            }
        }


        private void SetFogRadius(int radius)
        {
            if (fogOfWar != null)
            {
                fogOfWar.SetRevealRadius(radius);
            }
        }

        private void SetPlayerColor(Color color)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Giữ màu tự nhiên nguyên bản của Sprite Asset
                sr.color = Color.white;
            }
        }

        public RoleType GetCurrentRole() => currentRole;

        public bool IsWarrior() => currentRole == RoleType.Warrior;
        public bool IsRanger() => currentRole == RoleType.Ranger;
        public bool IsEngineer() => currentRole == RoleType.Engineer;
    }
}
