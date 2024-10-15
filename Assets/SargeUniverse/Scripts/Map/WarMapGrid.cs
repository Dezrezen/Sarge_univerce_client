using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Map.Buildings;
using UnityEngine;

namespace SargeUniverse.Scripts.Map
{
    public class WarMapGrid : MonoBehaviour
    {
        private const int BuildingSize = 1;
        
        [SerializeField] private GridLayout _grid = null;
        [SerializeField] private int _size = 21;
        
        [SerializeField] private bool _debug = true;
        [SerializeField] private float _right = 6f;
        [SerializeField] private float _left = 6f;
        [SerializeField] private float _up = 15.5f;
        [SerializeField] private float _down = -0.5f;

        public GridLayout Grid => _grid;
        public float Right => _right;
        public float Left => _left;
        public float Up => _up;
        public float Down => _down;
        
        private Vector2 _xDirection = Vector2.right;
        private Vector2 _yDirection = Vector2.up;
        private float _cellSize = 1f;

        private List<WarBase> _warBases = new();
        
        private void Awake()
        {
            _cellSize = Mathf.Sqrt(Mathf.Pow(_grid.cellSize.x / 2f, 2) + Mathf.Pow((_grid.cellSize.y / 2f), 2));
            _xDirection = new Vector2(_grid.cellSize.x, _grid.cellSize.y).normalized;
            _yDirection = new Vector2(-_grid.cellSize.x, _grid.cellSize.y).normalized;
        }
        
        public Vector3 CellToWorld(int x, int y)
        {
            var position = _grid.CellToWorld(new Vector3Int(x, y, 0));
            position.z = 0;
            return position;
        }
        
        public Vector3 GetGridCenter()
        {
            return new Vector3(transform.position.x, _down + (_up - _down) / 2f, transform.position.z);
        }
        
        private Vector3 GetEndPosition(int x, int y)
        {
            var position = _grid.CellToWorld(new Vector3Int(x + BuildingSize, y + BuildingSize, 0));
            position.z = 0;
            return position;
        }
        
        public Vector3 GetEndPosition(WarBase warBase)
        {
            return GetEndPosition(warBase.X, warBase.Y);
        }
        
        public void SelectWarBase(Vector2Int position)
        {
            var warBase = _warBases.Find(t => 
                IsGridPositionIsOnWarBase(position, t.X, t.Y));
            warBase.Selected();
        }
        
        public bool IsGridPositionIsOnWarBase(Vector2Int position, int x, int y)
        {
            var rect = new Rect(x, y, BuildingSize, BuildingSize);
            return rect.Contains(new Vector2(position.x, position.y));
        }
        
        public bool IsGridPositionOverWarBase(Vector2Int position)
        {
            return _warBases.Any(t => IsGridPositionIsOnWarBase(position, t.X, t.Y));
        }
        
        public void AddWarBase(WarBase warBase)
        {
            _warBases.Add(warBase);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_debug) { return; }
            if (_grid == null) { return; }
            
            var bl = _grid.transform.position + Vector3.down * _down + Vector3.left * _left;
            var br = _grid.transform.position + Vector3.down * _down + Vector3.right * _right;
            var tr = _grid.transform.position + Vector3.up * _up + Vector3.right * _right;
            var tl = _grid.transform.position + Vector3.up * _up + Vector3.left * _left;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tr, tl);
            Gizmos.DrawLine(tl, bl);
            var start = _grid.CellToWorld(new Vector3Int(0, 0, 0));
            start.z = 0;
            var end = _grid.CellToWorld(new Vector3Int(_size, _size));
            end.z = 0;
            var side1 = _grid.CellToWorld(new Vector3Int(_size, 0));
            side1.z = 0;
            var side2 = _grid.CellToWorld(new Vector3Int(0, _size));
            side2.z = 0;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(start, 0.1f);
            Gizmos.DrawLine(start, side2);
            Gizmos.DrawLine(side2, end);
            Gizmos.DrawLine(end, side1);
            Gizmos.DrawLine(side1, start);
        }
#endif
    }
}