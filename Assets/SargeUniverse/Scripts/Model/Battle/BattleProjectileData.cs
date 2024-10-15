using SargeUniverse.Scripts.Enums;
using UnityEngine.Events;

namespace SargeUniverse.Scripts.Model.Battle
{
    public class BattleProjectileData
    {
        public class ProjectileStats
        {
            public int damage;
            public float splash;
            public float speed;
        }
        
        private readonly TargetType _targetType;
        private readonly long _targetId;
        private readonly int _damage = 0;
        private readonly float _splash = 0;
        private float _time = 0;

        private bool _complete = false;

        private Callbacks.DamageCallback _damageCallback = null;

        public long Id => _targetId;
        public TargetType TargetType => _targetType;

        public BattleProjectileData(long id, TargetType targetType, int damage, float splash, float time)
        {
            _targetType = targetType;
            _targetId = id;
            _damage = damage;
            _splash = splash;
            _time = time;
        }

        public void Init(Callbacks.DamageCallback damageCallback)
        {
            _damageCallback = damageCallback;
        }

        public bool IsDone()
        {
            return _time < 0 && _complete;
        }
        
        public void UpdateStats(float deltaTime, UnityAction<int> onDamage)
        {
            if (_time <= 0)
            {
                //_damageCallback.Invoke(_targetId, _targetType, _damage);
                onDamage.Invoke(_damage);
                _complete = true;
            }

            if (_time > 0)
            {
                _time -= deltaTime;
            }
        }
    }
}