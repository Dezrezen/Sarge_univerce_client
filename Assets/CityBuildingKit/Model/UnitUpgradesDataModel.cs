using Enums;
using UnityEngine;

namespace Model
{
    [System.Serializable]
    public class UnitUpgradesDataModel
    {
        public UnitID id;
        public int level;
        public bool isUpgrading;
        public Time upgradeStartTime;
    }
}