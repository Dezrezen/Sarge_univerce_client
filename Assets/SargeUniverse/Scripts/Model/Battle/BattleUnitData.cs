using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace SargeUniverse.Scripts.Model.Battle
{
    public class BattleUnitData
    {
        private readonly Data_Unit _dataUnit = null;
        private TargetType _targetType = TargetType.Unit;
        public long _targetId = -1;
        
        private Callbacks.AttackCallback _attackCallback = null;
        private Callbacks.EmptyCallback _dieCallback = null;
        private Callbacks.IntCallback _damageCallback = null;
        private Callbacks.IntCallback _healCallback = null;
        private Callbacks.EmptyCallback _targetCallback = null;
        private Callbacks.EmptyCallback _idleCallback = null;
        private Callbacks.AttackCallback _projectileCallback = null;
        private Callbacks.PositionCallback _positionCallback = null;
        
        private float _attackDelay = 0;
        private int _health = 0;
        
        private Vector2 _position;
        private Vector2 _targetPosition;

        public long Id => _dataUnit.databaseID;
        public AttackMode AttackMode => _dataUnit.attackMode;
        public UnitMovementType MovementType => _dataUnit.movement;
        public float AttackRange => _dataUnit.attackRange;
        public Vector2 Position => _position;
        
        public BattleProjectileData.ProjectileStats GetProjectileStats()
        {
            return new BattleProjectileData.ProjectileStats()
            {
                damage = GetDamage(),
                splash = _dataUnit.splashRange,
                speed = _dataUnit.attackSpeed
            };
        }

        public BattleUnitData(Data_Unit data, float x, float y)
        {
            _dataUnit = data;
            _position = new Vector2(x, y);
        }
        
        public void Init(
            Callbacks.AttackCallback attackCallback,
            Callbacks.EmptyCallback dieCallback,
            Callbacks.IntCallback damageCallback,
            Callbacks.IntCallback healCallback,
            Callbacks.EmptyCallback targetCallback,
            Callbacks.EmptyCallback idleCallback,
            Callbacks.AttackCallback projectileCallback,
            Callbacks.PositionCallback positionCallback
        )
        {
            _attackCallback = attackCallback;
            _dieCallback = dieCallback;
            _damageCallback = damageCallback;
            _healCallback = healCallback;
            _targetCallback = targetCallback;
            _idleCallback = idleCallback;
            _projectileCallback = projectileCallback;
            _positionCallback = positionCallback;

            _health = _dataUnit.health;
            _attackDelay = _dataUnit.attackSpeed;
        }

        public bool IsAlive()
        {
            return _health > 0;
        }

        public void SetTarget(long id, Vector2 targetPosition, TargetType targetType = TargetType.Unit)
        {
            _targetId = id;
            _targetPosition = targetPosition;
            _targetType = targetType;
        }

        public long GetTarget()
        {
            return _targetId;
        }

        public TargetType GetTargetType()
        {
            return _targetType;
        }

        public void UpdatePosition(Vector2 targetPosition, float deltaTime)
        {
            _targetPosition = targetPosition;
            _position = Vector2.MoveTowards(_position, targetPosition, _dataUnit.moveSpeed * deltaTime / 10f);
            _positionCallback.Invoke(_dataUnit.databaseID, _position);
        }

        public void SetTargetPosition(Vector2 targetPosition)
        {
            _targetPosition = targetPosition;
        }
        
        public void UpdateStats(float deltaTime, UnityAction<int> onDamage, UnityAction<int> onHeal)
        {
            if (_health <= 0)
            {
                return;
            }

            var distanceToTarget = Vector2.Distance(_position, _targetPosition);
            if (distanceToTarget < _dataUnit.attackRange)
            {
                if (_attackDelay <= 0 && _targetId > 0)
                {
                    Attack(() => onDamage(GetDamage()), () => onHeal(GetDamage()));
                    _attackDelay = _dataUnit.attackSpeed;
                }
            }

            if (_attackDelay > 0)
            {
                _attackDelay -= deltaTime;
            }
        }

        public void TakeDamage(int damage)
        {
            if (_health > damage)
            {
                _health -= damage;
                _damageCallback.Invoke(_dataUnit.databaseID, damage);
            }
            else
            {
                _health = 0;
                _dieCallback.Invoke(_dataUnit.databaseID);
            }
        }

        public void TakeHeal(int heal)
        {
            if (_health < _dataUnit.health)
            {
                _health += heal;
                _health = _health > _dataUnit.health ? _dataUnit.health : _health;
                _healCallback.Invoke(_targetId, heal);
            }
        }

        public void Stop()
        {
            _targetId = -1;
            _idleCallback.Invoke(_dataUnit.databaseID);
        }

        private int GetDamage()
        {
            return Mathf.FloorToInt(_dataUnit.damage);
        }
        
        private void Attack(UnityAction onDamage, UnityAction onHeal)
        {
            _attackCallback.Invoke(_dataUnit.databaseID, _targetId, _targetType);
            if (_dataUnit.attackRange > 0)
            {
                _projectileCallback.Invoke(_dataUnit.databaseID, _targetId, _targetType);
            }
            else
            {
                _damageCallback.Invoke(_targetId, GetDamage());
                onDamage.Invoke();
            }
            
            // TODO: Heal Callback for Heal drone
            // onHeal.Invoke();
        }
    }
}