using UnityEngine;

namespace TestTools
{
    public class BuildGrid : MonoBehaviour
    {
        [SerializeField] private GridLayout _grid = null;
        [SerializeField] private int _size = 50;
        
        [SerializeField] private bool _debug = true;
        [SerializeField] private float _right = 10;
        [SerializeField] private float _left = 10;
        [SerializeField] private float _up = 10;
        [SerializeField] private float _down = 10;

        public float Right => _right;
        public float Left => _left;
        public float Up => _up;
        public float Down => _down;
        
        public static BuildGrid Instanse { get; private set; }

        private void Awake()
        {
            Instanse ??= this;
        }
        
        public Vector3 GetGridCenter()
        {
            return new Vector3(transform.position.x, _down + (_up - _down) / 2f, transform.position.z);
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