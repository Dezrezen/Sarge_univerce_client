using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Model.Battle
{
    public class BattleBuildingData
    {
        private readonly Data_Building _buildingData = null;
        private long _targetId = -1;
        
        private Callbacks.AttackCallback _attackCallback = null;
        private Callbacks.EmptyCallback _destroyCallback = null;
        private Callbacks.IntCallback _damageCallback = null;
        private Callbacks.AttackCallback _projectileCallback = null;

        private float _attackDelay = 0;
        private int _health = 0;

        public long Id => _buildingData.databaseID;
        public float AttackRange => _buildingData.radius;
        public AttackMode AttackMode => _buildingData.attackMode;

        public BattleProjectileData.ProjectileStats GetProjectileStats()
        {
            return new BattleProjectileData.ProjectileStats()
            {
                damage = Mathf.FloorToInt(_buildingData.damage),
                splash = _buildingData.splashRange,
                speed = _buildingData.speed
            };
        }

        public Vector2 Position => new(_buildingData.x,_buildingData.y);

        public int Size => _buildingData.rows;
        
        public BattleBuildingData(Data_Building data)
        {
            _buildingData = data;
        }
        
        public void Init(
            Callbacks.AttackCallback attackCallback,
            Callbacks.EmptyCallback destroyCallback,
            Callbacks.IntCallback damageCallback,
            Callbacks.AttackCallback projectileCallback
        )
        {
            _attackCallback = attackCallback;
            _destroyCallback = destroyCallback;
            _damageCallback = damageCallback;
            _projectileCallback = projectileCallback;

            _health = _buildingData.health;
            _attackDelay = 0;
        }

        public bool CanAttack()
        {
            return _buildingData.attackMode != AttackMode.None;
        }

        public bool IsAlive()
        {
            return _health > 0;
        }
        
        public void SetTarget(long targetId)
        {
            _targetId = targetId;
        }

        public void ClearTarget()
        {
            _targetId = -1;
        }

        public long GetTarget()
        {
            return _targetId;
        }

        public void UpdateStats(float deltaTime)
        {
            if (_health <= 0)
            {
                return;
            }
            
            if (_attackDelay <= 0 && _targetId > 0)
            {
                Attack();
                _attackDelay = _buildingData.speed;
            }

            if (_attackDelay > 0)
            {
                _attackDelay -= deltaTime;
            }
        }

        public void TakeDamage(int damage)
        {
            if (!IsAlive())
            {
                return;
            }
            
            _health -= damage;
            _damageCallback.Invoke(_buildingData.databaseID, damage);
            
            if (_health <= 0)
            {
                _health = 0;
                ClearTarget();
                _destroyCallback.Invoke(_buildingData.databaseID);
            }
        }
        
        private void Attack()
        {
            if (_buildingData.radius > 0)
            {
                _projectileCallback.Invoke(_buildingData.databaseID, _targetId, TargetType.Unit);
            }
            _attackCallback.Invoke(_buildingData.databaseID, _targetId, TargetType.Unit);
        }
    }
}