using System.Collections.Generic;
using CityBuildingKit.Scripts.Data.Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace CityBuildingKit.ScriptableObject.Units
{
    [CreateAssetMenu(fileName = "ArmyUnitData", menuName = "ScriptableObjects/ArmyUnitData", order = 1)]
    public class ArmyUnitData : UnityEngine.ScriptableObject
    {
        public List<TroopUpgradeInfo> upgradeInfo;
        [FormerlySerializedAs("statsInfo")] public UnitStats stats;

        public int[] trainingTime = new int[4];
    }
}