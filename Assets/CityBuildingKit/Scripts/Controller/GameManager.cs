using CityBuildingKit.Scripts.Controller;
using UnityEngine;

namespace Controller
{
    public class GameManager : MonoBehaviour
    {
        public ArmyManager armyManager { get; private set; }
        public UnitUpgradesManager unitUpgrades { get; private set; }

        
        
        public static GameManager instanse;

        private void Awake()
        {
            instanse ??= this;

            armyManager = new ArmyManager();
            unitUpgrades = new UnitUpgradesManager();
        }
    }
}