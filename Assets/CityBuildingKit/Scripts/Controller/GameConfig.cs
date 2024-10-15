using Config;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller
{
    public class GameConfig : MonoBehaviour
    {
        [SerializeField] private UnitsConfig unitsConfig;
        [SerializeField] private BuildingsConfig buildingConfig;
        
        public BuildingsConfig BuildingConfig => buildingConfig;
        public UnitsConfig UnitsConfig => unitsConfig;
        
        public static GameConfig instance;

        private void Awake()
        {
            instance ??= this;
        }
    }
}