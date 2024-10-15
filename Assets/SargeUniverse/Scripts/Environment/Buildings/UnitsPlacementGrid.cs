using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class UnitsPlacementGrid : MonoBehaviour
    {
        private const int Columns = 8;
        private const int Rows = 8;
        
        [Header("Anchors")]
        [SerializeField] private Transform _top = null;
        [SerializeField] private Transform _left = null;
        [SerializeField] private Transform _right = null;
        [SerializeField] private Transform _bottom = null;

        [Header("Content")]
        [SerializeField] private Transform _unitsTransform = null;
        
        private List<Vector3> _coordinates = new();
        private List<int> _takenCoords = new();

        private void Awake()
        {
            CalculateCoordinates();
            _takenCoords.Clear();
        }

        public int GetFreeCoordinatesIndex()
        {
            var index = Random.Range(0, Columns * Rows);
            if (_takenCoords.Contains(index))
            {
                return GetFreeCoordinatesIndex();
            }
            
            _takenCoords.Add(index);
            return index;
        }

        public Vector3 GetCoordinates(int index)
        {
            return _coordinates[index];
        }

        public void FreeCoordinates(int index)
        {
            _takenCoords.Remove(index);
        }

        public Transform GetParent()
        {
            return _unitsTransform;
        }
        
        
        private void CalculateCoordinates()
        {
            var width = _right.localPosition.x - _left.localPosition.x;
            var height = _top.localPosition.y - _bottom.localPosition.y;
            
            for (var i = 1; i < Columns + 2; i++)
            {
                for (var j = 1; j < Rows + 2; j++)
                {
                    var xPart = 1f * i / (Columns + 2);
                    var yPart = 1f * j / (Rows + 2);
                    var x = _bottom.localPosition.x + (yPart - xPart) * width / 2;
                    var y = _bottom.localPosition.y + (yPart + xPart) * height / 2;
                    var z = -(1 - xPart * yPart) / 1000f;

                    _coordinates.Add(new Vector3(x, y, z));
                }
            }
        }
    }
}