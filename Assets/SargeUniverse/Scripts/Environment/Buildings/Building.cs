using System;
using System.Collections.Generic;
using Config;
using Controller;
using SargeUniverse.Common.View;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings.Animators;
using SargeUniverse.Scripts.Map;
using SargeUniverse.Scripts.Sound;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class Building : UnitView
    {
        [SerializeField] protected BuildingID _buildingId = BuildingID.hq;
        [SerializeField] protected BuildingAnimator _animator = null;
        [SerializeField] private ConstructionProcess _constructionProcess = null;
        [SerializeField] private AttackRange _attackRange = null;
        
        [SerializeField] protected SpriteRenderer _baseArea = null;
        [SerializeField] protected SpriteRenderer _buildingSprite = null;
        
        protected bool _isConstructing = false;
        
        protected MapGrid _grid = null;
        protected Data_Building _buildingData;
        protected DateTime _nowTime;

        public Data_Building BuildingData => _buildingData;
        [HideInInspector] public UnityEvent<long, int, int> onBuldingMoved = new();
        
        protected int _originalX = 0;
        protected int _originalY = 0;
        protected int _currentY = 0;
        protected int _currentX = 0;
        protected int _x = 0;
        protected int _y = 0;

        public long DatabaseId => _buildingData.databaseID;

        [HideInInspector] public bool skipSync = false;
        public BuildingID buildingId => _buildingId;

        public int Width => _buildingData.rows;
        public int Height => _buildingData.columns;

        public int currentX => _currentX;
        public int currentY => _currentY;

        protected BuildingsConfig _buildingsConfig;

        [Inject]
        private void Construct(BuildingsConfig buildingsConfig)
        {
            _buildingsConfig = buildingsConfig;
        }
        
        protected virtual void Awake()
        {
            _baseArea.gameObject.SetActive(false);
        }
        
        protected virtual void Update()
        {
            onBuldingMoved.RemoveAllListeners();
        }

        protected virtual void OnDestroy()
        {
            _animator.StopAnimation();
        }

        public void SetMapGrid(MapGrid grid)
        {
            _grid = grid;
        }

        public virtual void SetBuildingData(Data_Building buildingData, bool updateSprite = false)
        {
            if (_buildingData?.level != buildingData.level)
            {
                updateSprite = true;
            }
            
            _buildingData = buildingData;

            if (_isConstructing != buildingData.isConstructing && buildingData.buildTime > 0)
            {
                _isConstructing = buildingData.isConstructing;
                if (_isConstructing)
                {
                    updateSprite = false;
                    StartUpgradeProcess();
                }
                else
                {
                    FinishUpgradeProcess();
                    updateSprite = true;
                }
            }
            
            if (updateSprite)
            {
                SetBuildingSprite();
                _attackRange?.SetRangeSize(_buildingData.radius);
            }
        }

        public void SetBuildingData(ServerBuilding serverData)
        {
            _buildingData = new Data_Building
            {
                id = Enum.Parse<BuildingID>(serverData.id),
                columns = serverData.columns,
                rows = serverData.rows,
                radius = serverData.radius
            };
            
            SetBuildingSprite();
            _attackRange?.SetRangeSize(_buildingData.radius);
        }

        public void InitBuilding()
        {
            PlaceOnGrid(_buildingData.x, _buildingData.y);
        }

        public void MoveBuilding(int x, int y)
        {
            skipSync = true;
            PlaceOnGrid(x, y);
        }
        
        public void PlaceOnGrid(int x, int y)
        {
            _currentX = x;
            _currentY = y;
            _x = x;
            _y = y;
            _originalX = x;
            _originalY = y;
            SetPosition(x, y);
        }
        
        private void SetPosition(int x, int y, bool editMode = false)
        {
            var position = _grid.CellToWorld(x, y);
            position.z = -1 + Math.Min(x, y) / 100f + Math.Max(x, y) / 10000f - (editMode ? 1 : 0);
            transform.position = position;
        }

        public void StartMovingOnGrid()
        {
            _x = _currentX;
            _y = _currentY;
        }

        public virtual void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
        {
            var dir = currentPosition - basePosition;
            var original = _grid.CellToWorld(_x, _y);
            var position = original + dir;
            var p = _grid.WorldToCell(position);
            _currentX = p.x;
            _currentY = p.y;
            SetPosition(_currentX, _currentY, true);
        }
        
        public void Selected()
        {
            _originalX = _currentX;
            _originalY = _currentY;
            _baseArea.gameObject.SetActive(true);
            BuildingsManager.Instanse.selectedBuilding = this;

            PlaySound();
            
            UIManager.Instanse.ShowBuildingInfoScreen();
        }

        public void Deselected()
        {
            UIManager.Instanse.HideBuildingInfoScreen();
            BuildingsManager.Instanse.SetEditMode(false);

            if (_originalX != _currentX || _originalY != _currentY)
            {
                if (BuildingsManager.Instanse.grid.CanPlaceBuilding(this, _currentX, currentY))
                {
                    onBuldingMoved.Invoke(_buildingData.databaseID, _currentX, currentY);
                }
                else
                {
                    PlaceOnGrid(_originalX, _originalY);
                }
            }
            else
            {
                PlaceOnGrid(_originalX, _originalY);
            }
            
            _baseArea.gameObject.SetActive(false);
            //BuildingsManager.Instanse.selectedBuilding = null;
        }

        private void PlaySound()
        {
            switch (_buildingData.id)
            {
                case BuildingID.barracks:
                    SoundSystem.Instance.PlaySound("barracks_sound");
                    break;
                case BuildingID.trainingcamp:
                    SoundSystem.Instance.PlaySound("trainingcamp_sound");
                    break;
                case BuildingID.watchtower:
                    SoundSystem.Instance.PlaySound("watchtower_sound");
                    break;
                case BuildingID.powerplant:
                case BuildingID.powerstorage:
                    SoundSystem.Instance.PlaySound("powerplant_sound");
                    break;
                case BuildingID.supplydrop:
                case BuildingID.supplyvault:
                    SoundSystem.Instance.PlaySound("supplydrop_sound");
                    break;
                case BuildingID.rocketturret:
                case BuildingID.motor:
                    SoundSystem.Instance.PlaySound("rocketturret_sound");
                    break;
                case BuildingID.mine:
                    SoundSystem.Instance.PlaySound("mine_sound");
                    break;
            }
        }

        public virtual void EditMode(bool flag)
        {
            var position = _buildingSprite.transform.localPosition;
            _buildingSprite.transform.localPosition = new Vector3(position.x, position.y, flag ? -1f : -0.1f);
        }
        
        protected virtual void SetBuildingSprite()
        {
        }

        protected virtual void StartUpgradeProcess()
        {
            if (_buildingData.level == 0)
            {
                _buildingSprite.enabled = false;
                _constructionProcess.StartBuildProcess();
            }
            else
            {
                _constructionProcess.StartUpgradeProcess();
            }
        }

        protected virtual void FinishUpgradeProcess()
        {
            if (_buildingData.level == 1)
            {
                _buildingSprite.enabled = true;
            }
            _constructionProcess.StopProcess();
        }
    }
}