using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.PathFinding
{
    public class PathBuilder
    {
        private readonly IBattleGrid _battleGrid;
        private readonly PriorityQueue _priorityQueue;

        public PathBuilder(IBattleGrid battleGrid) 
        {
            _battleGrid = battleGrid;
            _priorityQueue = new PriorityQueue(_battleGrid.Size.x * _battleGrid.Size.x);
        }

        private double Heuristic(Cell cell, Cell goal) 
        {
            var dX = Mathf.Abs(cell.location.x - goal.location.x);
            var dY = Mathf.Abs(cell.location.y - goal.location.y);
            
            return 1 * (dX + dY) + (Mathf.Sqrt(2) - 2 * 1) * Mathf.Min(dX, dY);
        }

        public void Reset() 
        {
            _battleGrid.Reset();
            _priorityQueue.Clear();
        }

        public List<Cell> BuildPath(Vector2Int start, Vector2Int goal) 
        {
            Reset();
            if (start.x < -1000 || start.y < -1000)
            {
                Debug.Log("Error");
            }
            Debug.Log(start);
            var startCell = _battleGrid[start];
            Debug.Log(goal);
            var goalCell = _battleGrid[goal];
            _priorityQueue.Enqueue(startCell, 0);
            var bounds = _battleGrid.Size;
            Cell node = null;
            while (_priorityQueue.Count > 0) 
            {
                node = _priorityQueue.Dequeue();
                node.closed = true;
                var cBlock = false;
                var g = node.G + 1;
                if (goalCell.location == node.location)
                {
                    break;
                }
                var proposed = new Vector2Int(0, 0);
                for (var i = 0; i < Constants.DirectionsCount; i++) 
                {
                    var direction = Constants.GetDirection(i);
                    proposed.x = node.location.x + direction.x;
                    proposed.y = node.location.y + direction.y;

                    // Check map bounds
                    if (proposed.x < 0 || proposed.x >= bounds.x || proposed.y < 0 || proposed.y >= bounds.y)
                    {
                        continue;
                    }

                    var neighbour = _battleGrid[proposed];
                    if (neighbour.blocked) 
                    {
                        if (i < 4)
                        {
                            cBlock = true;
                        }
                        continue;
                    }

                    // Prevent slipping between blocked cardinals by an open diagonal
                    if (i >= 4 && cBlock) continue;

                    if (_battleGrid[neighbour.location].closed)
                    {
                        continue;
                    }

                    if (!_priorityQueue.Contains(neighbour)) 
                    {
                        neighbour.G = g;
                        neighbour.H = Heuristic(neighbour, node);
                        neighbour.parent = node;

                        // F will be set by the queue
                        _priorityQueue.Enqueue(neighbour, neighbour.G + neighbour.H);
                    }
                    else if (g + neighbour.H < neighbour.priority) 
                    {
                        neighbour.G = g;
                        neighbour.priority = neighbour.G + neighbour.H;
                        neighbour.parent = node;
                    }
                }
            }

            var path = new Stack<Cell>();
            while (node != null) 
            {
                path.Push(node);
                node = node.parent;
            }
            return path.ToList();
        }
    }
}