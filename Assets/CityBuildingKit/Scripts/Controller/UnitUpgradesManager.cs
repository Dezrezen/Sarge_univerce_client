using System.Collections.Generic;
using Enums;
using UnityEngine.Events;

namespace Controller
{
    public class UnitUpgradesManager
    {
        private Dictionary<UnitID, int> _upgrades;

        public UnityEvent<UnitID> OnUpgradeComplete = new UnityEvent<UnitID>();
        
        public void StartUpgrade(UnitID id)
        {
            // TODO:
        }

        public bool CheckUpgradeRequirements(UnitID id)
        {
            // TODO: Need check if HQ and armory level meet the minimum requirements
            return true;
        }

        public int GetUpgradeCost(UnitID id)
        {
            // TODO:
            return 0;
        }

        public int GetUpgradeDuration(UnitID id)
        {
            // TODO:
            return 0;
        }
    }
}