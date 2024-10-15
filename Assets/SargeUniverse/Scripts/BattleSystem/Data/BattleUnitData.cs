using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Data;
using UnityEngine;

namespace SargeUniverse.Scripts.BattleSystem.Data
{
    public class BattleUnitData
    {
        public Data_Unit dataUnit = null;
        public float health = 0;
        public int target = -1;
        public int mainTarget = -1;
        public BattleVector2 position;
        public Path path = null;
        public double pathTime = 0;
        public double pathTraveledTime = 0;
        public double attackTimer = 0;
        public bool moving = false;
        
        public Dictionary<int, float> resourceTargets = new();
        public Dictionary<int, float> defenceTargets = new();
        public Dictionary<int, float> otherTargets = new();
        public Callbacks.AttackCallback attackCallback = null;
        public Callbacks.IndexCallback dieCallback = null;
        public Callbacks.FloatCallback damageCallback = null;
        public Callbacks.FloatCallback healCallback = null;
        public Callbacks.IndexCallback targetCallback = null;
        public Callbacks.IndexCallback idleCallback = null;


        public BattleBuildingData targetBuilding = null;
        public BattleUnitData targetUnit = null;
        public bool defMode = false;

        public BattleVector2 PositionOnGrid()
        {
            return new BattleVector2(position.x - Constants.BattleGridOffset, position.y - Constants.BattleGridOffset);
        }
        
        public Dictionary<int, float> GetAllTargets()
        {
            var temp = new Dictionary<int, float>();
            if (otherTargets.Count > 0)
            {
                temp = temp.Concat(otherTargets).ToDictionary(x => x.Key, x => x.Value);
            }
            if (resourceTargets.Count > 0)
            {
                temp = temp.Concat(resourceTargets).ToDictionary(x => x.Key, x => x.Value);
            }
            if (defenceTargets.Count > 0)
            {
                temp = temp.Concat(defenceTargets).ToDictionary(x => x.Key, x => x.Value);
            }
            return temp;
        }
        
        public void AssignTarget(int target, Path pathToTarget)
        {
            attackTimer = dataUnit.attackSpeed;
            this.target = target;
            path = pathToTarget;
            
            if (pathToTarget != null)
            {
                pathTraveledTime = 0;
                pathTime = pathToTarget.length / ((dataUnit.moveSpeed / 10f) * Constants.GridCellSize);
            }
            targetCallback?.Invoke(dataUnit.databaseID);
        }
        
        public void AssignTarget(Path pathToTarget)
        {
            attackTimer = dataUnit.attackSpeed;
            path = pathToTarget;
            
            if (pathToTarget != null)
            {
                pathTraveledTime = 0;
                pathTime = /*pathTraveledTime + */pathToTarget.length / ((dataUnit.moveSpeed / 10f) * Constants.GridCellSize);
            }
            targetCallback?.Invoke(dataUnit.databaseID);
        }
        
        public void TakeDamage(float damage)
        {
            if (health <= 0)
            {
                return;
            }
            
            health -= damage;
            damageCallback?.Invoke(dataUnit.databaseID, damage);

            if (health < 0)
            {
                health = 0;
            }
            
            if (health <= 0)
            {
                dieCallback?.Invoke(dataUnit.databaseID);
            }
        }

        public void Stop()
        {
            moving = false;
            path = null;
            targetUnit = null;
            targetBuilding = null;
            idleCallback?.Invoke(dataUnit.databaseID);
        }
        
        public void Initialize(int x, int y)
        {
            if (dataUnit == null)
            {
                return;
            }
            position = BattleMapUtils.GridToWorldPosition(new Vector2Int(x, y));
        }
    }
}