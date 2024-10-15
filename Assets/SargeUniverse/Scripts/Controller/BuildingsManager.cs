using System.Collections.Generic;
using Config;
using Controller;
using SargeUniverse.Common.View;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.Controller
{
    public class BuildingsManager : MonoBehaviour
    {
        [SerializeField] private BuildGrid _grid = null;
        private WallGrid _wallGrid;
        
        public BuildGrid grid => _grid;
        
        public bool buildMode { get; private set; }
        public bool editMode { get; private set; }

        private UnitView.Factory _factory;
        
        private NetworkPacket _networkPacket;
        private GameController _gameController;
        
        private PlayerSyncController _playerSyncController;
        private BuildingsConfig _buildingsConfig;
        
        public Building activeBuilding = null;
        public Building selectedBuilding = null;
        
        public static BuildingsManager Instanse { get; private set; }

        [Inject]
        private void Construct(
            UnitView.Factory factory,
            NetworkPacket networkPacket,
            GameController gameController,
            BuildingsConfig buildingsConfig)
        {
            _factory = factory;
            _networkPacket = networkPacket;
            _gameController = gameController;
            _buildingsConfig = buildingsConfig;
        }
        
        private void Awake()
        {
            Instanse ??= this;
            buildMode = false;
            editMode = false;
            _wallGrid = new WallGrid();
        }

        private void OnDestroy()
        {
            Instanse = null;
            buildMode = false;
            editMode = false;
            _wallGrid = null;
        }

        public void SetEditMode(bool flag)
        {
            editMode = flag;
            if (selectedBuilding)
            {
                selectedBuilding.EditMode(flag);
                if (selectedBuilding.buildingId == BuildingID.wall && flag)
                {
                    _wallGrid.SetEditMode(new Vector2Int(selectedBuilding.currentX, selectedBuilding.currentY));
                }
            }
        }

        public void CreateBuilding(BuildingID buildingId)
        {
            var prefab = GetBuildingPrefab(buildingId);
            if (prefab != null)
            {
                buildMode = true;
                var serverData = _gameController.GetServerBuilding(buildingId, 1);
                var point = _grid.GetBestBuildingPlace(serverData.rows, serverData.columns);
                activeBuilding = InstantiateBuilding(buildingId);
                //activeBuilding = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                activeBuilding.SetMapGrid(_grid);
                activeBuilding.SetBuildingData(serverData);
                activeBuilding.GetGenericView<ConstructionBuilding>().PlaceOnGrid(point.x, point.y, true);
                //activeBuilding.Selected();
            }
        }

        private Building GetBuildingPrefab(BuildingID buildingID)
        {
            return GameConfig.instance.BuildingConfig.GetBuildingData(buildingID).buildingPrefab;
        }

        private void RemoveFromGrid()
        {
            buildMode = false;
            Destroy(activeBuilding?.gameObject);
            activeBuilding = null;
        }

        public bool ConfirmBuild()
        {
            if (activeBuilding)
            {
                _networkPacket.SendBuildRequest(activeBuilding.buildingId, activeBuilding.currentX, activeBuilding.currentY);

                if (activeBuilding.buildingId == BuildingID.wall)
                {
                    var newPosition = _grid.GetWallNearPosition(new Vector2Int(activeBuilding.currentX, activeBuilding.currentY));
                    activeBuilding.PlaceOnGrid(newPosition.x, newPosition.y);
                    return false;
                }
                RemoveFromGrid();
            }
            
            return true;
        }

        public void CancelBuild()
        {
            if (activeBuilding)
            {
                RemoveFromGrid();
            }
        }

        public void SyncBuildings(List<Data_Building> buildings)
        {
            foreach (var buildingData in buildings)
            {
                buildingData.isPlayerBuilding = true;
                
                var building = _grid.GetBuilding(buildingData.databaseID);
                if (building == null)
                {
                    //building = InstantiateBuilding(buildingData.id);
                    building = InstantiateBuilding(buildingData.id);
                    building.SetMapGrid(_grid);
                    building.SetBuildingData(buildingData, true);
                    building.PlaceOnGrid(buildingData.x, buildingData.y);
                    building.onBuldingMoved
                        .AddListener(SendBuildingMovementRequest);
                    building.GetGenericView<ConstructionBuilding>()
                        .onSyncUpdataRequest
                        .AddListener(SendBuildingDataRequest);
                    
                    _grid.AddBuilding(building);
                    if (buildingData.id == BuildingID.trainingcamp)
                    {
                        UnitsManager.Instanse.AddTrainingCamp(building);
                    }

                    if (buildingData.id == BuildingID.wall)
                    {
                        _wallGrid.AddWall(
                            new Vector2Int(buildingData.x, buildingData.y), 
                            building.GetGenericView<Wall>()
                        );
                    }
                    
                    UIManager.Instanse.InitBuildingUI((ConstructionBuilding)building);
                }
                else
                {
                    if (building.skipSync)
                    {
                        building.skipSync = false;
                    }
                    else if (selectedBuilding == null || selectedBuilding.DatabaseId != building.DatabaseId)
                    {
                        building.PlaceOnGrid(buildingData.x, buildingData.y);
                    }
                    
                    building.SetBuildingData(buildingData);
                }
            }
        }

        public void MoveBuilding(long databaseId, int xPosition, int yPosition)
        {
            var building = _grid.GetBuilding(databaseId);
            if (building.buildingId == BuildingID.wall)
            {
                _wallGrid.MoveWall(new Vector2Int(xPosition, yPosition));
            }
            building.MoveBuilding(xPosition, yPosition);
        }

        /*private ConstructionBuilding InstantiateBuilding(BuildingID buildingID)
        {
            var prefab = GetBuildingPrefab(buildingID) as ConstructionBuilding;
            if (prefab == null)
            {
                return null;
            }
            
            var position = Vector3.zero;
            return Instantiate(prefab, position, Quaternion.identity, _grid.transform);
        }*/

        public void CollectResources(long databaseID, int amount)
        {
            var building = _grid.GetBuilding(databaseID) as ResourcesBuilding;
            if (building)
            {
                building.SetCollectState(amount);
            }
        }

        public int GetHqLevel()
        {
            return _grid.GetHqBuilding().BuildingData.level;
        }

        public int GetTotalBuilding(BuildingID buildingId)
        {
            return _grid.GetTotalBuilding(buildingId);
        }

        private void SendBuildingDataRequest()
        {
            _networkPacket.SyncRequest();
        }
        
        private void SendBuildingMovementRequest(long databaseId, int x, int y)
        {
            _networkPacket.SentMoveBuildingRequest(databaseId, x, y);
        }

        private ConstructionBuilding InstantiateBuilding(BuildingID buildingID)
        {
            var prefab = _buildingsConfig.GetBuildingData(buildingID).buildingPrefab;
            var instance = _factory.Create(prefab) as ConstructionBuilding;
            instance.SetPosition(Vector3.zero);
            instance.SetRotation(Quaternion.identity);
            instance.SetParent(_grid.transform);
            return instance;
        }
        
        // Selected Building methods

        public void SelectBuilding(Vector2Int position)
        {
            if (activeBuilding)
            {
                CancelBuild();
            }
            
            var building = _grid.SelectBuilding(position);
            if (selectedBuilding)
            {
                if (selectedBuilding == building)
                {
                    return;
                }
                DeselectBuilding();
            }

            selectedBuilding = building;
            selectedBuilding.Selected();
        }

        public void DeselectBuilding()
        {
            if (selectedBuilding)
            {
                selectedBuilding.Deselected();
                if (selectedBuilding.BuildingData.id == BuildingID.wall)
                {
                    _wallGrid.MoveWall(new Vector2Int(selectedBuilding.currentX, selectedBuilding.currentY));
                }
            }
            selectedBuilding = null;
        }
        
        // Network Request methods

        public void InstantBuildBuilding()
        {
            if (selectedBuilding)
            {
                _networkPacket.SendInstantBuildRequest(selectedBuilding.DatabaseId);
            }
        }
    }
}