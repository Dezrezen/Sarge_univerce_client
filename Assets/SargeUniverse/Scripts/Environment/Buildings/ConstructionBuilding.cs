using System;
using Controller;
using SargeUniverse.Scripts.Controller;
using UI.Elements;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace SargeUniverse.Scripts.Environment.Buildings
{
    public class ConstructionBuilding : Building
    {
        public UIBar BuildBar { get; set; }
        [HideInInspector] public UnityEvent onSyncUpdataRequest = new();
        
        protected override void Update()
        {
            var showBar = false;
            var isConstructing = _buildingData.isConstructing && _buildingData.buildTime > 0;
            if (isConstructing)
            {
                var span = TimeSpan.Zero;
                if (PlayerSyncController.ServerTime < _buildingData.constructionTime)
                {
                    span = _buildingData.constructionTime - PlayerSyncController.ServerTime;
                }

                if (span.TotalSeconds <= 0)
                {
                    isConstructing = false;
                    onSyncUpdataRequest.Invoke();
                }
                else
                {
                    showBar = true;
                }

                if (BuildBar && showBar)
                {
                    BuildBar.texts[0].text = Utils.Tools.SecondsToTimeFormat(span);
                    BuildBar.texts[0].ForceMeshUpdate(true);
                    BuildBar.bar.fillAmount = Mathf.Abs(1f - (float)span.TotalSeconds / _buildingData.buildTime);

                    if (span.TotalSeconds <= 0)
                    {
                        BuildBar.gameObject.SetActive(false);
                    }
                    else
                    {
                        BuildBar.gameObject.SetActive(true);
                        var end = _grid.GetEndPosition(_currentX, _currentY, Width, Height);

                        var plainDownLeft = CameraUtils.plainDownLeft;
                        var plainTopRight = CameraUtils.plainTopRight;

                        var w = plainTopRight.x - plainDownLeft.x;
                        var h = plainTopRight.y - plainDownLeft.y;

                        var endW = end.x - plainDownLeft.x;
                        var endH = end.y - plainDownLeft.y;

                        var screenPoint = new Vector2(endW / w * Screen.width, endH / h * Screen.height);
                        BuildBar.rect.anchoredPosition = screenPoint;
                    }
                }
            }
            
            if (BuildBar)
            {
                BuildBar.gameObject.SetActive(showBar);
            }
            
            if (isConstructing)
            {
                // TODO: Construction animation
            }
        }

        protected override void OnDestroy()
        {
            if (BuildBar && BuildBar != null)
            {
                Destroy(BuildBar.gameObject);
            }
        }

        public void PlaceOnGrid(int x, int y, bool build = false)
        {
            base.PlaceOnGrid(x, y);
            
            if (build)
            {
                if (_baseArea)
                {
                    _baseArea.gameObject.SetActive(true);
                }
            }
            else
            {
                SetBaseColor();
            }
        }

        public override void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
        {
            base.UpdateGridPosition(basePosition, currentPosition);
            if(_x != _currentX || _y != _currentY)
            {
                if (_baseArea)
                {
                    _baseArea.gameObject.SetActive(true);
                }
            }
            SetBaseColor();
        }

        private void SetBaseColor()
        {
            // TODO: Disable confirm button if we can't place building
            if (((BuildGrid)_grid).CanPlaceBuilding(this, _currentX, _currentY))
            {
                if (_baseArea)
                {
                    _baseArea.sharedMaterial.color = Color.white;
                }
            }
            else
            {
                if (_baseArea)
                {
                    _baseArea.sharedMaterial.color = Color.red;
                }
            }
        }
        
        protected override void SetBuildingSprite()
        {
            var buildingLevel = BuildingData.level == 0 ? 1 : BuildingData.level;
            _buildingSprite.sprite = _buildingsConfig.GetBuildingData(_buildingData.id).GetBuildingSprite(buildingLevel);
        }
    }
}