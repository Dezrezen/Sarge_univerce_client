using System.Collections.Generic;
using System.Linq;
using Controller;
using SargeUniverse.Scripts.Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;

namespace SargeUniverse.Scripts.Camera
{
    public class GameCameraController : CameraController
    {
        private Vector3 _buildBasePosition = Vector3.zero;
        private bool _movingBuilding = false;
        private Vector3 _replaceBasePosition = Vector3.zero;
        private bool _replacingBuilding = false;

        private BuildingsManager _buildingsManager;

        [Inject]
        private void Construct(BuildingsManager buildingsManager)
        {
            _buildingsManager = buildingsManager;
        }
        
        
        protected override void ScreenClicked()
        {
            var position = _input.Main.PointerPosition.ReadValue<Vector2>();
            
            var data = new PointerEventData(EventSystem.current) { position = position };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            
            if (UIManager.Instanse.mainMenuActive && !UIManager.Instanse.LockScreenMove)
            {
                if (results.Count <= 0)
                {
                    var found = false;
                    var planePosition = CameraUtils.CameraScreenPositionToWorldPosition(position);
                    var gridPosition = _mapGrid.WorldToCell(planePosition);
                    if (((BuildGrid)_mapGrid).IsGridPositionOverBuilding(new Vector2Int(gridPosition.x, gridPosition.y)))
                    {
                        found = true;
                        _buildingsManager.SelectBuilding(new Vector2Int(gridPosition.x, gridPosition.y));
                        //((BuildGrid)_mapGrid).SelectBuilding(new Vector2Int(gridPosition.x, gridPosition.y));
                    }

                    if (!found)
                    {
                        _buildingsManager.DeselectBuilding();
                        //BuildingsManager.Instanse.selectedBuilding?.Deselected();
                    }
                }
                else
                {
                    if (BuildingsManager.Instanse.selectedBuilding != null)
                    {
                        var handled = false;
                        foreach (var r in results.Where(r => r.gameObject.layer == LayerMask.NameToLayer("UI")))
                        {
                            handled = true;
                        }

                        if (!handled)
                        {
                            _buildingsManager.DeselectBuilding();
                        }
                    }
                }
            }
        }

        protected override void MoveStarted()
        {
            if (_zooming == false && UIManager.Instanse.mainMenuActive && !UIManager.Instanse.LockScreenMove)
            {
                if (BuildingsManager.Instanse.buildMode)
                {
                    _buildBasePosition = CameraUtils.CameraScreenPositionToWorldPosition(_input.Main.PointerPosition.ReadValue<Vector2>());
                    var gridPosition = _mapGrid.WorldToCell(_buildBasePosition);
                    var building = BuildingsManager.Instanse.activeBuilding;
                    if (_mapGrid.IsGridPositionIsOnZone(
                            new Vector2Int(gridPosition.x, gridPosition.y), 
                            building.currentX, 
                            building.currentY, 
                            building.Width, 
                            building.Height))
                    {
                        building.StartMovingOnGrid();
                        _movingBuilding = true;
                    }
                }

                if (BuildingsManager.Instanse.selectedBuilding != null)
                {
                    _replaceBasePosition = CameraUtils.CameraScreenPositionToWorldPosition(_input.Main.PointerPosition.ReadValue<Vector2>());
                    var gridPosition = _mapGrid.WorldToCell(_replaceBasePosition);
                    var building = BuildingsManager.Instanse.selectedBuilding;
                    if (_mapGrid.IsGridPositionIsOnZone(
                            new Vector2Int(gridPosition.x, gridPosition.y), 
                            building.currentX, 
                            building.currentY, 
                            building.Width, 
                            building.Height))
                    {
                        if (!BuildingsManager.Instanse.editMode)
                        {
                            BuildingsManager.Instanse.SetEditMode(true);
                        }
                        building.StartMovingOnGrid();
                        _replacingBuilding = true;
                    }
                }
                
                if (_movingBuilding == false && _replacingBuilding == false)
                {
                    base.MoveStarted();
                }
            }
        }

        protected override void MoveCanceled()
        {
            base.MoveCanceled();
            
            _movingBuilding = false;
            if (_replacingBuilding)
            {
                _replacingBuilding = false;
            }
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (BuildingsManager.Instanse.buildMode && _movingBuilding)
            {
                var position = CameraUtils.CameraScreenPositionToWorldPosition(_input.Main.PointerPosition.ReadValue<Vector2>());
                BuildingsManager.Instanse.activeBuilding.UpdateGridPosition(_buildBasePosition, position);
            }
            
            if (BuildingsManager.Instanse.editMode && _replacingBuilding)
            {
                var position = CameraUtils.CameraScreenPositionToWorldPosition(_input.Main.PointerPosition.ReadValue<Vector2>());
                BuildingsManager.Instanse.selectedBuilding.UpdateGridPosition(_replaceBasePosition, position);
            }
        }
    }
}