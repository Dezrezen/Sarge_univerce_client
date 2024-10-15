using System.Collections.Generic;
using CityBuildingKit.Scripts.UIControllersAndData.StructureType;

namespace CityBuildingKit.Scripts.Utils
{
    public static class ArmyUtil
    {
        private static List<StructureType> ListOfBuildingsToTrainUnits = new List<StructureType> { StructureType.TrainingCamp, StructureType.Barrack};

        public static bool IsBuildingToTrainUnit(StructureType type)
        {
            return ListOfBuildingsToTrainUnits.Contains(type);
        }
        
    }
}