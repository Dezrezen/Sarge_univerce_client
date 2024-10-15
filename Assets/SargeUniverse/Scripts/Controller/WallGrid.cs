using System.Collections.Generic;
using SargeUniverse.Scripts.Environment.Buildings;
using UnityEngine;

namespace SargeUniverse.Scripts.Controller
{
    public class WallGrid
    {
        private readonly Dictionary<Vector2Int, Wall> _walls = new();
        
        private Wall _editedWall;

        public void AddWall(Vector2Int position, Wall wall)
        {
            _walls.Add(position, wall);
            UpdateWalls(position);
        }

        public void RemoveWall(Vector2Int position)
        {
            if (_walls.ContainsKey(position))
            {
                _walls.Remove(position);
                UpdateWalls(position);
            }
        }

        public void MoveWall(Vector2Int position)
        {
            if (_editedWall)
            {
                _walls.Add(position, _editedWall);
                UpdateWalls(position);
                _editedWall = null;
            }
        }

        public void SetEditMode(Vector2Int position)
        {
            _editedWall = _walls[position];
            RemoveWall(position);
        }

        private void UpdateWalls(Vector2Int position, bool checkPatents = true)
        {
            if (_walls.TryGetValue(position, out var wall))
            {
                if (position.x < Constants.GridSize - 1)
                {
                    wall.SetRightConnection(_walls.ContainsKey(new Vector2Int(position.x + 1, position.y)));
                }

                if (position.y < Constants.GridSize - 1)
                {
                    wall.SetLeftConnection(_walls.ContainsKey(new Vector2Int(position.x, position.y + 1)));
                }
            }

            if (!checkPatents)
            {
                return;
            }
            
            if (position.x > 0)
            {
                UpdateWalls(new Vector2Int(position.x - 1, position.y), false);
            }

            if (position.y > 0)
            {
                UpdateWalls(new Vector2Int(position.x, position.y - 1), false);
            }
        }
    }
}