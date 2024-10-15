using System.Collections.Generic;
using System.Linq;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;

namespace SargeUniverse.Scripts.Model
{
    public class BattleSync
    {
        // Opponent
        private long _lastOpponent = 0;
        private OpponentData _opponentData = null;
        
        // Player
        private List<Data_Unit> _playerArmy = null;
        private Dictionary<UnitID, DeployUnit> _deployUnits = new();
        
        // Battle
        private BattlePhase _phase = BattlePhase.Deploy;
        public BattlePhase Phase => _phase;

        private static BattleSync _instance = null;

        public static BattleSync Instance()
        {
            return _instance ??= new BattleSync();
        }
        
        public void SetBattlePhase(BattlePhase phase)
        {
            _phase = phase;
        }
        
        public void SetOpponentData(long opponentId, OpponentData opponentData)
        {
            if (opponentId > 0 && opponentData != null && opponentId != _lastOpponent)
            {
                _lastOpponent = opponentId;
                _opponentData = opponentData;
                
                _phase = BattlePhase.Deploy;
            }
            else
            {
                // TODO: Show message - No opponents found
            }
        }

        public OpponentData GetOpponentData()
        {
            return _opponentData;
        }

        public void SetPlayerData(List<Data_Unit> playerArmy)
        {
            _deployUnits.Clear();
            _playerArmy = playerArmy;
            
            foreach (var unitData in playerArmy)
            {
                if (_deployUnits.ContainsKey(unitData.id))
                {
                    _deployUnits[unitData.id].unitsData.Add(unitData);
                }
                else
                {
                    _deployUnits.Add(unitData.id, new DeployUnit(unitData));
                }
            }
        }

        public Dictionary<UnitID, DeployUnit> GetDeployUnits()
        {
            return _deployUnits;
        }

        public Data_Unit GetUnit(long databaseId)
        {
            return _playerArmy.Find(u => u.databaseID == databaseId);
        }

        public Data_Unit GetUnitToDeploy(UnitID id, long databaseId = 0)
        {
            Data_Unit unitData = null; 
            if (_deployUnits.ContainsKey(id) && _deployUnits[id].unitsData.Count > 0)
            {
                if (databaseId == 0)
                {
                    unitData = _deployUnits[id].unitsData.First();
                    _deployUnits[id].unitsData.RemoveAt(0);
                }
                else
                {
                    {
                        unitData = _deployUnits[id].unitsData.Find(unit => unit.databaseID == databaseId);
                        _deployUnits[id].unitsData.Remove(unitData);
                    }
                }

                if (_deployUnits[id].unitsData.Count == 0)
                {
                    _deployUnits.Remove(id);
                }
            }
            return unitData;
        }
    }
}