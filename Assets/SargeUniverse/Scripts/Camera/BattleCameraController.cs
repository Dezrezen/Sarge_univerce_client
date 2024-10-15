using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace SargeUniverse.Scripts.Camera
{
    public class BattleCameraController : CameraController
    {
        protected override void ScreenClicked()
        {
            var position = _input.Main.PointerPosition.ReadValue<Vector2>();
            
            var data = new PointerEventData(EventSystem.current){ position = position };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            
            
            if (results.Count <= 0 && BattleControl.Instance.DeployUnitId != UnitID.empty)
            {
                var planePosition = CameraUtils.CameraScreenPositionToWorldPosition(position);
                var gridPosition = _mapGrid.WorldToCell(planePosition);
                if (gridPosition.x is >= 0 - Constants.BattleGridOffset and < Constants.GridSize + Constants.BattleGridOffset &&
                    gridPosition.y is >= 0 - Constants.BattleGridOffset and < Constants.GridSize + Constants.BattleGridOffset)
                {
                    BattleControl.Instance.DeployUnit(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y));
                }
            }
        }
    }
}