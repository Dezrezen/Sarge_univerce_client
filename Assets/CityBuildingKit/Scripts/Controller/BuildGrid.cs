using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using SargeUniverse.Scripts.Map;
using UnityEngine;
using Utils;

namespace Controller
{
    public class BuildGrid : MapGrid
    {
        private List<Building> _buildings = new();

        public Building GetBuilding(long databaseId)
        {
            return _buildings.Find(building => building.DatabaseId == databaseId);
        }

        public Building GetHqBuilding()
        {
            return _buildings.Find(building => building.buildingId == BuildingID.hq);
        }

        public int GetTotalBuilding(BuildingID buildingId)
        {
            return _buildings.FindAll(building => building.buildingId == buildingId).Count;
        }
        
        public void AddBuilding(Building building)
        {
            _buildings.Add(building);
        }
        
        public Vector3 GetEndPosition(Building building)
        {
            return GetEndPosition(building.currentX, building.currentY, building.Width, building.Height);
        }

        public Building SelectBuilding(Vector2Int position)
        {
            return _buildings.Find(t => 
                IsGridPositionIsOnZone(position, t.currentX, t.currentY, t.Width, t.Height));
        }
        
        public bool IsGridPositionOverBuilding(Vector2Int position)
        {
            return _buildings.Any(t => 
                IsGridPositionIsOnZone(position, t.currentX, t.currentY, t.Width, t.Height));
        }

        public Vector2Int GetBestBuildingPlace(int width, int height)
        {
            var cameraCenterPosition = CameraUtils.CameraScreenPositionToWorldPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
            var cameraCenterCellPosition = Grid.WorldToCell(cameraCenterPosition);
            var center = new Vector2(cameraCenterCellPosition.x, cameraCenterCellPosition.y);

            return GetBestBuildingPlace(width, height, center);
        }

        private Vector2Int GetBestBuildingPlace(int width, int height, Vector2 center)
        {
            var point = new Vector2Int(Mathf.FloorToInt(Constants.GridSize / 2f), Mathf.FloorToInt(Constants.GridSize / 2f));
            var distance = Mathf.Infinity;
            for (var x = 0; x < Constants.GridSize; x++)
            {
                for (var y = 0; y < Constants.GridSize; y++)
                {
                    if (x > Constants.GridSize - height || y > Constants.GridSize - width ||
                        (Mathf.Approximately(x, center.x) && Mathf.Approximately(y, center.y)))
                    {
                        continue;
                    }
                    var d = Vector2.Distance(center, new Vector2(x, y));
                    if (!(d < distance) || !CanPlaceBuilding(x, y, width, height))
                    {
                        continue;
                    }
                        
                    distance = d;
                    point = new Vector2Int(x, y);
                }
            }
            return point;
        }

        public Vector2Int GetWallNearPosition(Vector2Int position)
        {
            Vector2Int checkPosition;
            Building building;
            
            // First try to attach wall to other wall
            if (position.y is > 0 and < Constants.GridSize - 1)
            {
                checkPosition = new Vector2Int(position.x, position.y + 1);
                building = SelectBuilding(checkPosition);
                if (building && building.buildingId == BuildingID.wall)
                {
                    checkPosition = new Vector2Int(position.x, position.y - 1);
                    if (!IsGridPositionOverBuilding(checkPosition))
                    {
                        return checkPosition;
                    }
                }
                
                checkPosition = new Vector2Int(position.x, position.y - 1);
                building = SelectBuilding(checkPosition);
                if (building && building.buildingId == BuildingID.wall)
                {
                    checkPosition = new Vector2Int(position.x, position.y + 1);
                    if (!IsGridPositionOverBuilding(checkPosition))
                    {
                        return checkPosition;
                    }
                }
            }
            
            if (position.x is > 0 and < Constants.GridSize - 1)
            {
                checkPosition = new Vector2Int(position.x + 1, position.y);
                building = SelectBuilding(checkPosition);
                if (building && building.buildingId == BuildingID.wall)
                {
                    checkPosition = new Vector2Int(position.x - 1, position.y);
                    if (!IsGridPositionOverBuilding(checkPosition))
                    {
                        return checkPosition;
                    }
                }
                
                checkPosition = new Vector2Int(position.x - 1, position.y);
                building = SelectBuilding(checkPosition);
                if (building && building.buildingId == BuildingID.wall)
                {
                    checkPosition = new Vector2Int(position.x + 1, position.y);
                    if (!IsGridPositionOverBuilding(checkPosition))
                    {
                        return checkPosition;
                    }
                }
            }
            
            if (position.y is > 0 and < Constants.GridSize - 1)
            {
                checkPosition = new Vector2Int(position.x, position.y - 1);
                if (!IsGridPositionOverBuilding(checkPosition))
                {
                    return checkPosition;
                }
                
                checkPosition = new Vector2Int(position.x, position.y + 1);
                if (!IsGridPositionOverBuilding(checkPosition))
                {
                    return checkPosition;
                }
            }
            
            if (position.x is > 0 and < Constants.GridSize - 1)
            {
                checkPosition = new Vector2Int(position.x - 1, position.y);
                if (!IsGridPositionOverBuilding(checkPosition))
                {
                    return checkPosition;
                }
                
                checkPosition = new Vector2Int(position.x + 1, position.y);
                if (!IsGridPositionOverBuilding(checkPosition))
                {
                    return checkPosition;
                }
            }
            
            return GetBestBuildingPlace(1, 1, position);
        }
        
        private bool CanPlaceBuilding(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || x + height > Constants.GridSize || y + width > Constants.GridSize)
            {
                return false;
            }
            
            foreach (var t in _buildings)
            {
                var rect1 = new Rect(t.currentX, t.currentY, t.Width, t.Height);
                var rect2 = new Rect(x, y, height, width);
                if (rect2.Overlaps(rect1))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanPlaceBuilding(Building building, int x, int y)
        {
            if (building.currentX < 0 || building.currentY < 0 || 
                building.currentX + building.Width > Constants.GridSize || 
                building.currentY + building.Height > Constants.GridSize)
            {
                return false;
            }

            foreach (var b in _buildings)
            {
                if (b != building)
                {
                    var rect1 = new Rect(b.currentX, b.currentY, b.Width, b.Height);
                    var rect2 = new Rect(building.currentX, building.currentY, building.Width, building.Height);
                    
                    if (rect2.Overlaps(rect1))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}