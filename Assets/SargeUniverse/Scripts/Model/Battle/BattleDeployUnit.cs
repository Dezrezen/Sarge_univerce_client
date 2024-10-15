using System.Numerics;
using SargeUniverse.Scripts.Data;

namespace SargeUniverse.Scripts.Model.Battle
{
    public class BattleDeployUnit
    {
        private readonly Data_Unit _unitData = null;
        private readonly float _x = 0;
        private readonly float _y = 0;

        private Callbacks.UnitSpawned _spawnCallback = null;
        private Callbacks.AttackCallback _attackCallback = null;
        private Callbacks.EmptyCallback _dieCallback = null;
        private Callbacks.IntCallback _damageCallback = null;
        private Callbacks.IntCallback _healCallback = null;
        private Callbacks.EmptyCallback _targetCallback = null;
        private Callbacks.EmptyCallback _idleCallback = null;
        private Callbacks.AttackCallback _projectileCallback = null;
        private Callbacks.PositionCallback _positionCallback = null;
        
        public BattleDeployUnit(Data_Unit data, float x, float y)
        {
            _unitData = data;
            _x = x;
            _y = y;
        }

        public Vector2 DeployPoint => new Vector2(_x, _y);

        public void Init(
            Callbacks.UnitSpawned spawnCallback,
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
            _spawnCallback = spawnCallback;
            _attackCallback = attackCallback;
            _dieCallback = dieCallback;
            _damageCallback = damageCallback;
            _healCallback = healCallback;
            _targetCallback = targetCallback;
            _idleCallback = idleCallback;
            _projectileCallback = projectileCallback;
            _positionCallback = positionCallback;
        }

        public BattleUnitData DeployInBattle()
        {
            var unit = new BattleUnitData(_unitData, _x, _y);
            unit.Init(
                _attackCallback, 
                _dieCallback, 
                _damageCallback, 
                _healCallback, 
                _targetCallback, 
                _idleCallback, 
                _projectileCallback,
                _positionCallback
            );
            _spawnCallback.Invoke(_unitData.databaseID, _x, _y);
            
            return unit;
        }
    }
}