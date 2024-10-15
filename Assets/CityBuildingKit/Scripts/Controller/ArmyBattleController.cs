using System.Collections.Generic;
using Assets.Scripts.UIControllersAndData.Store;
using UnityEngine;

namespace CityBuildingKit.Scripts.Controller
{
    public class ArmyBattleController : MonoBehaviour
    {
        private List<GrassSelector> _buildingTargets = new();
        private List<FighterController> _troops = new();
        
        public void SpawnTroop(FighterController troop)
        {
            _troops.Add(troop);
            GetTargetAndAttack(troop);
        }

        private void GetTargetAndAttack(FighterController troop, bool attack = false)
        {
            if (_buildingTargets == null || _buildingTargets.Count == 0)
            {
                _buildingTargets = Helios.Instance.GetAllBuildingTargets();
            }
            
            var target = GetPreferredTarget(troop.transform.position, troop.UnitData.GetPreferredTarget());
            troop.SetTarget(target, () => GetTargetAndAttack(troop, true));
            
            if (attack && troop.GrassTarget != null)
            {
                troop.Attack();
            }
        }

        private GrassSelector GetPreferredTarget(Vector3 unitPosition, EntityType preferredTarget = EntityType.Any)
        {
            var aliveBuildings = _buildingTargets.FindAll(building => !building.isDestroyed);
            if (aliveBuildings.Count == 0)
            {
                Helios.Instance.CompleteMission();
                return null;
            }

            List<GrassSelector> targets;
            if (preferredTarget != EntityType.Any)
            {
                targets = aliveBuildings.FindAll(building => building.EntityType == preferredTarget);
                return targets.Count > 0 ? GetClosestTarget(targets, unitPosition) : GetPreferredTarget(unitPosition);
            }
            
            targets = aliveBuildings;
            return GetClosestTarget(targets, unitPosition);
        }

        private GrassSelector GetClosestTarget(List<GrassSelector> buildingsList, Vector3 unitPosition)
        {
            var closestBuilding = buildingsList[0];
            var distance = Vector2.Distance(closestBuilding.transform.position, unitPosition);
            
            foreach (var building in buildingsList)
            {
                var distanceToBuilding = Vector2.Distance(building.transform.position, unitPosition);
                
                if (distanceToBuilding >= distance) continue;
                
                closestBuilding = building;
                distance = distanceToBuilding;
            }
            
            return closestBuilding;
        }
    }
}