using System.Collections.Generic;
using SargeUniverse.Scripts.Data;
using UnityEngine;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    [RequireComponent(typeof(UnitsPlacementGrid))]
    public class TrainingCamp : Building
    {
        private UnitsPlacementGrid _placementGrid = null;
        private int _maxHousing = 0;
        private int _housing = 0;

        private readonly List<Data_Unit> _garrisonUnits = new();

        protected override void Awake()
        {
            base.Awake();
            
            _placementGrid = GetComponent<UnitsPlacementGrid>();
            _housing = 0;
        }

        public override void SetBuildingData(Data_Building buildingData, bool updateSprite = false)
        {
            base.SetBuildingData(buildingData, updateSprite);
            _maxHousing = buildingData.capacity;
        }

        public UnitsPlacementGrid GetPlacementGrid()
        {
            return _placementGrid;
        }

        public bool CanAddUnit(Data_Unit unitData)
        {
            return _maxHousing - _housing > unitData.hosing;
        }

        public void AddUnitToCamp(Data_Unit unitData)
        {
            _garrisonUnits.Add(unitData);
            _housing += unitData.hosing;
        }

        public void RemoveUnitFromCamp(long databaseId)
        {
            var unitData = _garrisonUnits.Find(unit => unit.databaseID == databaseId);
            if (unitData != null)
            {
                _housing -= unitData.hosing;
                _garrisonUnits.Remove(unitData);
            }
        }
    }
}