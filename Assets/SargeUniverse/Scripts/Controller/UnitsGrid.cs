using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SargeUniverse.Scripts.Environment.Units;

namespace SargeUniverse.Scripts.Controller
{
    public class UnitsGrid : MonoBehaviour
    {
        [SerializeField] private GridLayout _grid = null;
        [SerializeField] private Tilemap _tilemap = null;
        [SerializeField] private int _size = 50;

        [SerializeField] private bool _debug = true;
        [SerializeField] private float _right = 10;
        [SerializeField] private float _left = 10;
        [SerializeField] private float _up = 10;
        [SerializeField] private float _down = 10;
        public float right => _right;
        public float left => _left;
        public float up => _up;
        public float down => _down;

        private Vector2 _xDirection = Vector2.right;
        private Vector2 _yDirection = Vector2.up;
        
        private int _rows = 0;
        private int _columns = 0;
        private float _cellSize = 1f;

        public GridLayout grid => _grid;
        public float cellSize => _cellSize;

        private List<Unit> _units = new();
        
        private void Awake()
        {
            _rows = _size;
            _columns = _size;
            
            _cellSize = Mathf.Sqrt(Mathf.Pow(_grid.cellSize.x / 2f, 2) + Mathf.Pow((_grid.cellSize.y / 2f), 2));
            _xDirection = new Vector2(_grid.cellSize.x, _grid.cellSize.y).normalized;
            _yDirection = new Vector2(-_grid.cellSize.x, _grid.cellSize.y).normalized;
        }
        
        public Unit GetUnit(long databaseId)
        {
            return _units.Find(unit => unit.DatabaseId == databaseId);
        }
        
        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public void RemoveUnit(long databaseId)
        {
            var unit = GetUnit(databaseId);
            if (unit != null)
            {
                _units.Remove(unit);
                Destroy(unit.gameObject);
            }
        }
        
        public Vector3 CellToWorld(int x, int y)
        {
            var position = grid.CellToWorld(new Vector3Int(x, y, 0));
            position.z = 0;
            return position;
        }
    }
}