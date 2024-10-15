using System.Collections.Generic;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model;
using SargeUniverse.Scripts.Model.Battle;
using UnityEngine;

namespace SargeUniverse.Scripts.Controller
{
    public class BattleFix
    {
        private List<BattleBuildingData> _enemyBuilding = new();
        private List<BattleUnitData> _enemyUnits = new();

        private List<BattleProjectileData> _projectiles = new();
        
        private List<BattleUnitData> _attackUnits = new();
        private List<BattleDeployUnit> _deployUnits = new();
        
        private Callbacks.IntCallback _buildingDamageCallback = null;
        private Callbacks.IntCallback _unitDamageCallback = null;
        
        private Callbacks.DamageCallback _projectileCallback = null;
        
        private int _frameCount = 0;

        public int FrameCount => _frameCount;
        
        public void Init(
            List<BattleBuildingData> enemyBuilding, 
            List<BattleUnitData> enemyUnits, 
            Callbacks.AttackCallback buildingAttackCallback,
            Callbacks.EmptyCallback buildingDestroyCallback,
            Callbacks.IntCallback buildingDamageCallback,
            Callbacks.AttackCallback unitAttackCallback,
            Callbacks.EmptyCallback unitUnitDieCallback, 
            Callbacks.IntCallback unitUnitDamageCallback, 
            Callbacks.IntCallback unitUnitHealCallback,
            Callbacks.EmptyCallback unitUnitTargetCallback,
            Callbacks.EmptyCallback unitUnitIdleCallback,
            Callbacks.DamageCallback projectileCallback,
            Callbacks.PositionCallback positionCallback)
        {
            _projectileCallback = projectileCallback;
            
            _enemyBuilding = enemyBuilding;
            foreach (var building in _enemyBuilding)
            {
                building.Init(
                    buildingAttackCallback,
                    buildingDestroyCallback,
                    buildingDamageCallback,
                    BuildingProjectileCallback
                );
            }

            _enemyUnits = enemyUnits;
            foreach (var unit in _enemyUnits)
            {
                unit.Init(
                    unitAttackCallback,
                    unitUnitDieCallback,
                    unitUnitDamageCallback,
                    unitUnitHealCallback,
                    unitUnitTargetCallback,
                    unitUnitIdleCallback,
                    EnemyProjectileCallback,
                    positionCallback
                    );
            }
        }

        public void AddDeployUnit(Data_Unit unitData,
            float x,
            float y,
            Callbacks.UnitSpawned spawnCallback,
            Callbacks.AttackCallback attackCallback,
            Callbacks.EmptyCallback dieCallback,
            Callbacks.IntCallback damageCallback,
            Callbacks.IntCallback healCallback,
            Callbacks.EmptyCallback targetCallback,
            Callbacks.EmptyCallback idleCallback,
            Callbacks.PositionCallback positionCallback)
        {
            if (BattleSync.Instance().Phase == BattlePhase.End)
            {
                return;
            }

            var deployUnit = new BattleDeployUnit(unitData, x, y);
            deployUnit.Init(
                spawnCallback,
                attackCallback,
                dieCallback,
                damageCallback,
                healCallback,
                targetCallback,
                idleCallback,
                AttackProjectileCallback,
                positionCallback
            );
            _deployUnits.Add(deployUnit);
        }
        
        private void BuildingProjectileCallback(long unitId, long targetId, TargetType targetType)
        {
            var building = GetEnemyBuilding(unitId);
            var targetPosition = GetAttackUnit(targetId).Position;
            var ps = building.GetProjectileStats();
            var time = Vector3.Distance(building.Position, targetPosition) / ps.speed;
            var projectileData = new BattleProjectileData(targetId, targetType, ps.damage, ps.splash, time);
            projectileData.Init(_projectileCallback);
            _projectiles.Add(projectileData);
        }
        
        private void EnemyProjectileCallback(long unitId, long targetId, TargetType targetType)
        {
            var unit = GetEnemyUnit(unitId);
            var targetPosition = GetAttackUnit(targetId).Position;
            var ps = unit.GetProjectileStats();
            var time = Vector3.Distance(unit.Position, targetPosition) / ps.speed;
            var projectileData = new BattleProjectileData(targetId, targetType, ps.damage, ps.splash, time);
            projectileData.Init(_projectileCallback);
            _projectiles.Add(projectileData);
        }
        
        private void AttackProjectileCallback(long unitId, long targetId, TargetType targetType)
        {
            var unit = GetAttackUnit(unitId);
            var targetPosition = (unit.GetTargetType() == TargetType.Unit) ? GetEnemyUnit(targetId).Position : GetEnemyBuilding(targetId).Position;
            var ps = unit.GetProjectileStats();
            var time = Vector3.Distance(unit.Position, targetPosition) / ps.speed;
            var projectileData = new BattleProjectileData(targetId, targetType, ps.damage, ps.splash, time);
            projectileData.Init(_projectileCallback);
            _projectiles.Add(projectileData);
        }
        
        public void ExecuteFrame(float deltaTime)
        {
            foreach (var deployUnit in _deployUnits)
            {
                var unit = deployUnit.DeployInBattle();
                _attackUnits.Add(unit);
            }
            _deployUnits.Clear();

            foreach (var building in _enemyBuilding)
            {
                if (building.CanAttack() && building.IsAlive())
                {
                    HandleBuilding(building, deltaTime);
                }
            }

            foreach (var unit in _attackUnits)
            {
                HandleAttackUnit(unit, deltaTime);
            }
            
            foreach (var unit in _enemyUnits)
            {
                HandleEnemyUnit(unit, deltaTime);
            }

            foreach (var projectile in _projectiles)
            {
                HandleProjectile(projectile, deltaTime);
            }
            _projectiles.RemoveAll(p => p.IsDone());
        }

        private void HandleBuilding(BattleBuildingData buildingData, float deltaTime)
        {
            if (buildingData.CanAttack())
            {
                var targetId = buildingData.GetTarget();
                if (targetId < 0)
                {
                    FindTargetForBuilding(buildingData);
                }
                else
                {
                    if (!GetAttackUnit(targetId).IsAlive())
                    {
                        FindTargetForBuilding(buildingData);
                    }
                }
            }
            buildingData.UpdateStats(deltaTime);
        }

        private void HandleAttackUnit(BattleUnitData unitData, float deltaTime)
        {
            if (!unitData.IsAlive())
            {
                return;
            }
            
            FindTargetForAttack(unitData);
            var targetId = unitData.GetTarget();
            /*if (targetId < 0)
            {
                FindTargetForAttack(unitData);
            }
            else
            {
                if (unitData.GetTargetType() == TargetType.Building && GetEnemyBuilding(targetId).IsAlive())
                {
                    FindTargetForAttack(unitData);
                }
                else if (unitData.GetTargetType() == TargetType.Unit && GetEnemyUnit(targetId).IsAlive())
                {
                    FindTargetForAttack(unitData);
                }
            }*/

            if (targetId > 0)
            {
                if (unitData.GetTargetType() == TargetType.Building)
                {
                    var b = GetEnemyBuilding(targetId);
                    var point = GetBuildingClosestPoint(b, unitData.Position);
                    var distanceToTarget = Vector2.Distance(unitData.Position, point);
                    if (distanceToTarget > unitData.AttackRange)
                    {
                        unitData.UpdatePosition(point, deltaTime);
                    }
                    else
                    {
                        unitData.SetTargetPosition(point);
                    }
                }
                else if (unitData.GetTargetType() == TargetType.Unit)
                {
                    var u = GetEnemyUnit(targetId);
                    var distanceToTarget = Vector2.Distance(unitData.Position, u.Position);
                    if (distanceToTarget > unitData.AttackRange)
                    {
                        unitData.UpdatePosition(GetEnemyUnit(targetId).Position, deltaTime);
                    }
                    else
                    {
                        unitData.SetTargetPosition(u.Position);
                    }
                }
            }
            else
            {
                unitData.Stop();
            }
            unitData.UpdateStats(deltaTime,
                (damage) =>
                {
                    if (unitData.GetTargetType() == TargetType.Building)
                    {
                        var b = GetEnemyBuilding(targetId);
                        unitData.TakeDamage(damage);
                    }
                    
                    if (unitData.GetTargetType() == TargetType.Unit)
                    {
                        var u = GetEnemyUnit(targetId);
                        unitData.TakeDamage(damage);
                    }
                },
                (heal) =>
                {
                    if (unitData.GetTargetType() == TargetType.Unit)
                    {
                        var u = GetAttackUnit(targetId);
                        unitData.TakeHeal(heal);
                    }
                }
            );
        }

        private void HandleEnemyUnit(BattleUnitData unitData, float deltaTime)
        {
            if (!unitData.IsAlive())
            {
                return;
            }
            
            FindTargetForDefence(unitData);
            var targetId = unitData.GetTarget();
            /*if (targetId < 0)
            {
                FindTargetForDefence(unitData);
            }
            else
            {
                if (!GetAttackUnit(targetId).IsAlive())
                {
                    FindTargetForDefence(unitData);
                }
            }*/

            if (targetId > 0)
            {
                var u = GetAttackUnit(targetId);
                var distanceToTarget = Vector2.Distance(unitData.Position, u.Position);
                if (distanceToTarget > unitData.AttackRange)
                {
                    unitData.UpdatePosition(u.Position, deltaTime);
                }
                else
                {
                    unitData.SetTargetPosition(u.Position);
                }
            }
            else
            {
                unitData.Stop();
            }
            unitData.UpdateStats(deltaTime, (damage) =>
                {
                    var u = GetAttackUnit(targetId);
                    unitData.TakeDamage(damage);
                },
            (heal) =>
                {
                    var u = GetEnemyUnit(targetId);
                    unitData.TakeHeal(heal);
                }
            );
        }

        private void HandleProjectile(BattleProjectileData projectileData, float deltaTime)
        {
            if (!projectileData.IsDone())
            {
                projectileData.UpdateStats(deltaTime, (damage) =>
                {
                    if (projectileData.TargetType == TargetType.Building)
                    {
                        GetEnemyBuilding(projectileData.Id).TakeDamage(damage);
                    }

                    if (projectileData.TargetType == TargetType.Unit)
                    {
                        var unit = GetEnemyUnit(projectileData.Id) ?? GetAttackUnit(projectileData.Id);
                        unit.TakeDamage(damage);
                    }
                });
            }
        }
        
        // =========
        private BattleUnitData GetAttackUnit(long id)
        {
            return _attackUnits.Find(u => u.Id == id);
        }

        private BattleUnitData GetEnemyUnit(long id)
        {
            return _enemyUnits.Find(u => u.Id == id);
        }

        private BattleBuildingData GetEnemyBuilding(long id)
        {
            return _enemyBuilding.Find(b => b.Id == id);
        }
        
        // =========
        private void FindTargetForBuilding(BattleBuildingData buildingData)
        {
            var minDistanceToTarget = 999f;
            long id = -1;
            foreach (var unit in _attackUnits)
            {
                if (!unit.IsAlive())
                {
                    continue;
                }

                if (buildingData.AttackMode == AttackMode.Ground && unit.MovementType == UnitMovementType.Fly)
                {
                    continue;
                }

                if (buildingData.AttackMode == AttackMode.Air && unit.MovementType == UnitMovementType.Ground)
                {
                    continue;
                }

                var distanceToTarget = Vector2.Distance(buildingData.Position, unit.Position);
                if (distanceToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distanceToTarget;
                    id = unit.Id;
                }
            }
            buildingData.SetTarget(id);
        }
        
        private void FindTargetForDefence(BattleUnitData unitData)
        {
            var minDistanceToTarget = 999f;
            long id = -1;
            var targetPoint = unitData.Position;
            
            foreach (var unit in _attackUnits)
            {
                if (!unit.IsAlive())
                {
                    continue;
                }

                if (unitData.AttackMode == AttackMode.Ground && unit.MovementType == UnitMovementType.Fly)
                {
                    continue;
                }

                if (unitData.AttackMode == AttackMode.Air && unit.MovementType == UnitMovementType.Ground)
                {
                    continue;
                }

                var distanceToTarget = Vector2.Distance(unitData.Position, unit.Position);
                if (distanceToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distanceToTarget;
                    id = unit.Id;
                    targetPoint = unitData.Position;
                }
            }
            unitData.SetTarget(id, targetPoint);
        }

        private void FindTargetForAttack(BattleUnitData unitData)
        {
            var minDistanceToTarget = 999f;
            long id = -1;
            var targetType = TargetType.Unit;
            var targetPoint = unitData.Position;
            
            foreach (var unit in _enemyUnits)
            {
                if (!unit.IsAlive())
                {
                    continue;
                }

                if (unitData.AttackMode == AttackMode.Ground && unit.MovementType == UnitMovementType.Fly)
                {
                    continue;
                }

                if (unitData.AttackMode == AttackMode.Air && unit.MovementType == UnitMovementType.Ground)
                {
                    continue;
                }

                var distanceToTarget = Vector2.Distance(unitData.Position, unit.Position);
                if (distanceToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distanceToTarget;
                    id = unit.Id;
                    targetType = TargetType.Unit;
                    targetPoint = unitData.Position;
                }
            }

            foreach (var building in _enemyBuilding)
            {
                if (!building.IsAlive())
                {
                    continue;
                }

                var point = GetBuildingClosestPoint(building, unitData.Position);
                var distanceToTarget = Vector2.Distance(unitData.Position, point);
                if (distanceToTarget < minDistanceToTarget)
                {
                    minDistanceToTarget = distanceToTarget;
                    id = building.Id;
                    targetType = TargetType.Building;
                    targetPoint = point;
                }
            }
            unitData.SetTarget(id, targetPoint, targetType);
        }

        private Vector2 GetBuildingClosestPoint(BattleBuildingData buildingData, Vector2 targetPoint)
        {
            var point1 = buildingData.Position;
            var point2 = buildingData.Position + Vector2.one * buildingData.Size;

            return NearestPointOnLine(point1, point2, targetPoint);
        }

        private Vector2 NearestPointOnLine(Vector2 lineStartPoint, Vector2 lineEndPoint, Vector2 targetpoint)
        {
            var line = lineEndPoint - lineStartPoint;
            var len = line.magnitude;
            line.Normalize();

            var v = targetpoint - lineStartPoint;
            var d = Vector2.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return lineStartPoint + line * d;
        }
    }
}