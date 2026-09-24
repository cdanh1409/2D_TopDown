using UnityEngine;

namespace TopDownSurvival.World
{
    public class PlacedBuilding : MonoBehaviour
    {
        [Header("Building Properties")]
        [SerializeField] private string buildingName = "Structure";
        
        public void Setup(string name, Color color)
        {
            buildingName = name;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = color;
            }
        }

        public string GetBuildingName() => buildingName;
    }
}
