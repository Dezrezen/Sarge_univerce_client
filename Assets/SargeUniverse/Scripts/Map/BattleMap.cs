using System.Collections.Generic;
using System.Linq;
using Config;
using Controller;
using SargeUniverse.Common.View;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using SargeUniverse.Scripts.Environment.Units;
using UI.Elements;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.Map
{
    public class BattleMap : MapGrid
    {
        [SerializeField] private Transform _gameCanvasTransform = null;
        [SerializeField] private UIBar _hpBarPrefab = null;
        
        private readonly List<BattleBuilding> _buildings = new();
        private readonly List<Building> _trainingCamps = new();
        private WallGrid _wallGrid;
        
        private List<Unit> _defenceUnits = new();
        private List<Unit> _attackUnits = new();

        private UnitView.Factory _factory;
        private BuildingsConfig _buildingsConfig;
        private UnitsConfig _unitsConfig;

        [Inject]
        private void Construct(
            UnitView.Factory factory, 
            BuildingsConfig buildingsConfig, 
            UnitsConfig unitsConfig)
        {
            _factory = factory;
            _buildingsConfig = buildingsConfig;
            _unitsConfig = unitsConfig;
        }

        protected override void Awake()
        {
            base.Awake();
            _wallGrid = new WallGrid();
        }
        
        private void OnDestroy()
        {
            foreach (var building in _buildings)
            {
                Destroy(building);
            }
            _buildings.Clear();
            _trainingCamps.Clear();

            foreach (var unit in _defenceUnits)
            {
                Destroy(unit);
            }
            _defenceUnits.Clear();

            foreach (var unit in _attackUnits)
            {
                Destroy(unit);
            }

            _attackUnits.Clear();
            _wallGrid = null;
        }

        public void AddBuilding(Data_Building buildingData)
        {
            var building = InstantiateBuilding(buildingData.id);
            if (building != null)
            {
                //var building = Instantiate(prefab.GetGenericView<BattleBuilding>(), Vector3.zero, quaternion.identity);
                building.SetMapGrid(this);
                building.SetBuildingData(buildingData);
                building.InitBuilding();
                building.transform.SetParent(transform);

                building.HealthBar = Instantiate(_hpBarPrefab, _gameCanvasTransform);
            
                _buildings.Add(building);

                if (buildingData.id == BuildingID.trainingcamp)
                {
                    _trainingCamps.Add(building);
                }

                if (buildingData.id == BuildingID.wall)
                {
                    _wallGrid.AddWall(
                        new Vector2Int(building.currentX, building.currentY), 
                        building.GetGenericView<Wall>()
                    );
                }
            }
            else
            {
                Debug.LogWarning($"Building prefab with ID {buildingData.id} not found");
            }
        }
        
        public BattleBuilding GetBuilding(long databaseId)
        {
            return _buildings.Find(building => building.DatabaseId == databaseId);
        }
        
        public void AddDefenceUnit(Data_Unit unitData)
        {
            var unit = InstantiateUnit(unitData);
            unit.PlaceOnGrid(_trainingCamps[0].currentX, _trainingCamps[0].currentY);
            _defenceUnits.Add(unit);
        }
        
        public void AddDefenceUnit(Data_Unit unitData, int x, int y)
        {
            var unit = InstantiateUnit(unitData);
            unit.PlaceOnGrid(x, y);
            _defenceUnits.Add(unit);
        }

        public Unit GetDefenceUnit(long databaseId)
        {
            return _defenceUnits.Find(unit => unit.DatabaseId == databaseId);
        }
        
        public void RemoveDefenceUnit(Unit unit)
        {
            _defenceUnits.Remove(unit);
        }

        public bool CanPlaceUnit(int x, int y)
        {
            return _buildings.All(b => !IsGridPositionIsOnZone(new Vector2Int(x, y), b.currentX, b.currentY, b.Height, b.Width));
        }

        public void PlaceUnit(Data_Unit unitData, float x, float y)
        {
            var unit = InstantiateUnit(unitData);
            unit.PlaceOnGrid(x, y);
            _attackUnits.Add(unit);
        }

        public Unit GetAttackUnit(long databaseId)
        {
            return _attackUnits.Find(unit => unit.DatabaseId == databaseId);
        }

        public void RemoveAttackUnit(Unit unit)
        {
            _attackUnits.Remove(unit);
        }
        
        private Unit InstantiateUnit(Data_Unit unitData)
        {
            var prefab = GameConfig.instance.UnitsConfig.GetUnitData(unitData.id).unitPrefab;
            if (prefab != null)
            {
                var unit = Instantiate(prefab, transform);
                unit.SetMapGrid(this);
                unit.SetUnitData(unitData);
                unit.HealthBar = Instantiate(_hpBarPrefab, _gameCanvasTransform);

                var configData = _unitsConfig.GetUnitData(unitData.id);
                unit.SetAudioData(configData.attackAudioTrack, configData.moveAudioTrack);
                
                return unit;
            }

            Debug.LogWarning($"Unit prefab with ID {unitData.id} not found");
            return null;
        }

        public List<Unit> GetAllUnits()
        {
            var units = new List<Unit>();
            units.AddRange(_defenceUnits);
            units.AddRange(_attackUnits);

            return units;
        }

        public Vector2Int GetCampPosition()
        {
            return _trainingCamps?.Count > 0 ? 
                new Vector2Int(_trainingCamps[0].currentX, _trainingCamps[0].currentY) : 
                new Vector2Int(Mathf.CeilToInt(Constants.GridSize), Mathf.CeilToInt(Constants.GridSize));
        }

        private BattleBuilding InstantiateBuilding(BuildingID buildingId)
        {
            var prefab = _buildingsConfig.GetBuildingData(buildingId).opponentPrefab;
            if (prefab == null)
            {
                return null;
            }
            
            var instance = _factory.Create(prefab) as BattleBuilding;
            if (instance == null)
            {
                return null;
            }
            
            instance.SetPosition(Vector3.zero);
            instance.SetRotation(Quaternion.identity);
            instance.SetParent(transform);
            return instance;
        }
    }
}