using System.Collections.Generic;
using Config.Building;
using Enums;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using Zenject;

namespace Config
{
    [CreateAssetMenu(fileName = "BuildingsConfig", menuName = "Data/BuildingsConfig")]
    public class BuildingsConfig : ScriptableObjectInstaller
    {
        public List<BuildingData> buildings = new();

        public BuildingData GetBuildingData(BuildingID id)
        {
            return buildings.Find(data => data.buildingId == id);
        }

        public List<BuildingData> GetBuildingDataOfType(BuildingType type)
        {
            return buildings.FindAll(data => data.buildingType == type);
        }
        
        public override void InstallBindings()
        {
            Container.Bind<BuildingsConfig>()
                .FromInstance(this)
                .AsSingle();
        }
    }
}