using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private int _boardSize = 32;
        [SerializeField] private int _offset = 10;
        [SerializeField] private float _tileHeight = 0.375f;

        [SerializeField] private List<GameObject> _grassTiles = new();
        [SerializeField] private List<GameObject> _sandTiles = new();
        [SerializeField] private List<GameObject> _waterTiles = new();

        private void Awake()
        {
            GenerateBoard();
        }

        private void GenerateBoard()
        {
            for (var i = -_offset; i < _boardSize + _offset; i++)
            {
                for (var j = -_offset; j < _boardSize + _offset; j++)
                {
                    var tile = GetTile(i, j);
                    var position = new Vector3(-(i-j) * 0.5f, (j+i) * _tileHeight + _tileHeight, 0);
                    var instance = Instantiate(tile, position, Quaternion.identity);
                    instance.name = "Tile_" + i + "_" + j;
                    instance.transform.SetParent(this.transform);
                }
            }
        }

        private GameObject GetTile(int x, int y)
        {
            if (x <= -4 || x > _boardSize + 2 || y <= -4 || y > _boardSize + 2)
            {
                return _waterTiles[0];
            }

            if (CheckRange(x, y, 3))
            {
                return _sandTiles[0];
            }
            
            if (CheckRange(x, y, 2))
            {
                return _sandTiles[0];
            }
            
            if (CheckRange(x, y, 1))
            {
                return _sandTiles[0];
            }
            
            var index = (x + y) % 2;
            return _grassTiles[index];
        }

        private bool CheckRange(int x, int y, int range)
        {
            if (x == -range || x == _boardSize + range - 1)
            {
                return y >= -range || y <= _boardSize + range - 1;
            }

            if (y == -range || y == _boardSize + range - 1)
            {
                return x >= -range || x <= _boardSize + range - 1;
            }
            
            return false;
        }
    }
}