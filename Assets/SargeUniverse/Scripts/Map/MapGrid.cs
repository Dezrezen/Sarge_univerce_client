using UnityEngine;
using UnityEngine.Tilemaps;

namespace SargeUniverse.Scripts.Map
{
    public abstract class MapGrid : MonoBehaviour
    {
        [SerializeField] protected GridLayout _grid;
        [SerializeField] protected Tilemap _tilemap;

        [SerializeField] private bool _debug = true;

        [SerializeField]
        protected float _right = 27f;
        [SerializeField]
        protected float _left = 27f;
        [SerializeField]
        protected float _up = 39f;
        [SerializeField]
        protected float _down  = 1.5f;

        public float Right => _right;
        public float Left => _left;
        public float Up => _up;
        public float Down => _down;
        
        public Vector2 xDirection = Vector2.right;
        public Vector2 yDirection = Vector2.up;
        public float cellSize = 1f;

        public GridLayout Grid => _grid;
        
        protected virtual void Awake()
        {
            cellSize = Mathf.Sqrt(Mathf.Pow(_grid.cellSize.x / 2f, 2) + Mathf.Pow((_grid.cellSize.y / 2f), 2));
            xDirection = new Vector2(_grid.cellSize.x, _grid.cellSize.y).normalized;
            yDirection = new Vector2(-_grid.cellSize.x, _grid.cellSize.y).normalized;
        }
        
        public Vector3 CellToWorld(int x, int y)
        {
            var position = _grid.CellToWorld(new Vector3Int(x, y, 0));
            position.z = 0;
            return position;
        }
        
        public Vector2Int WorldToCell(Vector3 position)
        {
            var cell = _grid.WorldToCell(position);
            return new Vector2Int(cell.x, cell.y);
        }

        public Vector3 GetGridCenter()
        {
            var start = _grid.CellToWorld(new Vector3Int(0, 0, 0));
            var end = _grid.CellToWorld(new Vector3Int(Constants.GridSize, Constants.GridSize));
            // return new Vector3(transform.position.x, Down + (Up - Down) / 2f, transform.position.z);
            return new Vector3(transform.position.x, start.y + (end.y - start.y) / 2f, transform.position.z);
        }
        
        public Vector3 GetStartPosition(int x, int y)
        {
            return _grid.CellToWorld(new Vector3Int(x, y, 0));
        }
        
        public Vector3 GetCenterPosition(int x, int y, int width, int height)
        {
            var start = _grid.CellToWorld(new Vector3Int(x, y, 0));
            var end = _grid.CellToWorld(new Vector3Int(x + height, y + width, 0));
            return Vector3.Lerp(start, end, 0.5f);
        }
        
        public Vector3 GetEndPosition(int x, int y, int width, int height)
        {
            var position = _grid.CellToWorld(new Vector3Int(x + height, y + width, 0));
            position.z = 0;
            return position;
        }

        public Vector3 GetRandomPositionInZone(int x, int y, int width, int height)
        {
            var up = GetStartPosition(x + width, y + height);
            var left = GetStartPosition(x, y + height);
            var right = GetStartPosition(x + width, y);
            var down = GetStartPosition(x, y);
            
            var yLen = up.y - down.y;
            
            var yPos = Random.Range(down.y, up.y) - down.y;
            var yRatio = yPos / yLen;
            var xPos = (Random.Range(left.x, right.x) - down.x) * (1 - yRatio);

            return new Vector3(xPos, yPos, 0);
        }
        
        public bool IsGridPositionIsOnZone(Vector2Int position, int x, int y, int width, int height)
        {
            var rect = new Rect(x, y, width, height);
            return rect.Contains(new Vector2(position.x, position.y));
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!_debug)
            {
                return;
            }

            if (_grid == null)
            {
                return;
            }
            
            var bl = _grid.transform.position + Vector3.down * Down + Vector3.left * Left;
            var br = _grid.transform.position + Vector3.down * Down + Vector3.right * Right;
            var tr = _grid.transform.position + Vector3.up * Up + Vector3.right * Right;
            var tl = _grid.transform.position + Vector3.up * Up + Vector3.left * Left;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tr, tl);
            Gizmos.DrawLine(tl, bl);
            var start = _grid.CellToWorld(new Vector3Int(0, 0, 0));
            start.z = 0;
            var end = _grid.CellToWorld(new Vector3Int(Constants.GridSize, Constants.GridSize));
            end.z = 0;
            var side1 = _grid.CellToWorld(new Vector3Int(Constants.GridSize, 0));
            side1.z = 0;
            var side2 = _grid.CellToWorld(new Vector3Int(0, Constants.GridSize));
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