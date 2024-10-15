using System.Collections.Generic;
using Data;
using Model;
using Save;

namespace CityBuildingKit.Scripts.Controller
{
    public class SaveDataStorage
    {
        public List<UnitData> units;
        public List<UnitUpgradesData> upgrades;
        
        public void SaveData()
        {
            var data = new SaveDataModel();
        }

        public void LoadData()
        {
            var data = SaveLoad.LoadPlayerData();
        }
    }
}