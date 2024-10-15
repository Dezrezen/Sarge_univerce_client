using System;
using System.Linq;
using Common.Model;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.Controller
{
    public class PlayerSyncController : MonoBehaviour
    {
        private Data_Player _dataPlayer = new Data_Player();
        
        private float _timer = 0;
        private bool _updating = false;
        private float _syncTime = 5;
        
        private int _hqLevel = 1;

        private bool _inBattle = false;
        
        public string PlayerName { get; private set; }
        public IntData Xp { get; private set; }
        public int Level { get; private set; }
        public IntData Supplies { get; private set; }
        public IntData MaxSupplies { get; private set; }
        public IntData Power { get; private set; }
        public IntData MaxPower { get; private set; }
        public IntData Energy { get; private set; }
        public IntData MaxEnergy { get; private set; }
        public IntData Gems { get; private set; }
        public IntData Workers { get; private set; }
        public IntData MaxWorkers { get; private set; }
        
        public IntData UnitsCapacity { get; private set; }
        public IntData MaxUnitsCapacity { get; private set; }

        public static DateTime ServerTime = DateTime.Now;

        private NetworkPacket _networkPacket;
        private GameController _gameController;
        private ResponseParser _responseParser;
        
        [Inject]
        private void Construct(NetworkPacket networkPacket, GameController gameController, ResponseParser responseParser)
        {
            _networkPacket = networkPacket;
            _gameController = gameController;
            _responseParser = responseParser;

            InitCallbacks();
        }

        private void InitCallbacks()
        {
            _responseParser.onReceivePlayerData.AddListener(SyncData);
            _responseParser.onBattleFound.AddListener(SetPlayerArmy);
        }
        
        private void Awake()
        {
            PlayerName = "Player";
            Xp = new IntData(0);
            Level = 1;
            
            Supplies = new IntData(0);
            MaxSupplies = new IntData(1);
            Power = new IntData(0);
            MaxPower = new IntData(1);
            Energy = new IntData(0);
            MaxEnergy = new IntData(1);
            Gems = new IntData(0);
            Workers = new IntData(1);
            MaxWorkers = new IntData(1);

            UnitsCapacity = new IntData(0);
            MaxUnitsCapacity = new IntData(0);
        }

        private void Update()
        {
            if (!_inBattle)
            {
                if (_timer <= 0)
                {
                    if (_updating == false)
                    {
                        _updating = true;
                        _timer = _syncTime;
                        _networkPacket?.SyncRequest();
                    }
                }
                else
                {
                    _timer -= Time.deltaTime;
                }
                _dataPlayer.nowTime = _dataPlayer.nowTime.AddSeconds(Time.deltaTime);
                ServerTime = _dataPlayer.nowTime;
            }
        }

        private void SyncData(Data_Player dataPlayer)
        {
            if (_inBattle)
            {
                return;
            }
            
            _dataPlayer = dataPlayer;
            ServerTime = _dataPlayer.nowTime;
            
            PlayerName = dataPlayer.name;
            Level = dataPlayer.level;
            Xp.Set(dataPlayer.xp);
            
            Gems.Set(dataPlayer.gems);
            
            
            foreach (var building in dataPlayer.buildings)
            {
                if (building.level == 0)
                {
                    continue;
                }
                
                switch (building.id)
                {
                    case BuildingID.hq:
                        _hqLevel = building.level;
                        MaxSupplies.Set(building.suppliesCapacity);
                        Supplies.Set(building.suppliesStorage);
                        MaxPower.Set(building.powerCapacity);
                        Power.Set(building.powerStorage);
                        MaxEnergy.Set(building.energyCapacity);
                        Energy.Set(building.energyStorage);

                        UnitsCapacity.Set(0);
                        MaxUnitsCapacity.Set(0);
                        break;
                    case BuildingID.supplyvault:
                        MaxSupplies.Add(building.suppliesCapacity);
                        Supplies.Add(building.suppliesStorage);
                        break;
                    case BuildingID.powerstorage:
                        MaxPower.Add(building.powerCapacity);
                        Power.Add(building.powerStorage);
                        break;
                    case BuildingID.trainingcamp:
                        MaxUnitsCapacity.Add(building.capacity);
                        break;
                }
            }

            foreach (var unit in dataPlayer.units)
            {
                UnitsCapacity.Add(unit.hosing);
            }
            
            Supplies.Set(Mathf.Clamp(Supplies.GetValue(), 0, MaxSupplies.GetValue()));
            Power.Set(Mathf.Clamp(Power.GetValue(), 0, MaxPower.GetValue()));
            Energy.Set(Mathf.Clamp(Energy.GetValue(), 0, MaxEnergy.GetValue()));
                
            BuildingsManager.Instanse?.SyncBuildings(dataPlayer.buildings);
            UpdateBuilderCount();
            UnitsManager.Instanse?.SyncUnits(dataPlayer.units);
            
            _updating = false;
        }
        
        private void SetPlayerArmy()
        {
            var playerArmy = _dataPlayer.units.Where(unitData => unitData.trained).ToList();
            BattleSync.Instance().SetPlayerData(playerArmy);
        }

        public bool HaveTrainedUnits()
        {
            return _dataPlayer.units.Count(unitData => unitData.trained) > 0;
        }

        public void EnterBattleMode()
        {
            _inBattle = true;
        }

        public void ExitBattleMode()
        {
            _inBattle = false;
        }
        
        public int GetNextLevelRequiredXp()
        {
            return Level switch
            {
                1 => 30,
                <= 200 => (Level - 1) * 50,
                <= 299 => ((Level - 200) * 500) + 9500,
                _ => ((Level - 300) * 1000) + 60000
            };
        }

        public Data_Unit GetUnit(long databaseId)
        {
            return _dataPlayer.units.Find(unit => unit.databaseID == databaseId);
        }
        
        public int GetTrainTime(UnitID unitId)
        {
            var barracksCount = _dataPlayer.buildings.FindAll(b => b.id == BuildingID.barracks).Count;
            var unit = _gameController.GetServerUnit(unitId);

            return barracksCount switch
            {
                4 => unit.trainTime_4,
                3 => unit.trainTime_3,
                2 => unit.trainTime_2,
                _ => unit.trainTime_1
            };
        }

        /*public float GetTrainingUnitTime(UnitID unitId)
        {
            var unit = _playerData.units.Find(u => u.id == unitId && u.trained == false && u.trainedTime > 0);
            return unit?.trainedTime ?? 0;
        }*/

        public bool CanTrainUnits()
        {
            return _dataPlayer.buildings.FindAll(b => b.id == BuildingID.barracks && b.isConstructing == false).Count > 0;
        }
        
        private void UpdateBuilderCount()
        {
            var maxWorkers = _dataPlayer.buildings.FindAll(b => b.id == BuildingID.builderstation).Count;
            var workers = maxWorkers;

            foreach (var b in _dataPlayer.buildings.Where(b => b.isConstructing))
            {
                workers--;
            }
            
            MaxWorkers.SetValue(maxWorkers);
            Workers.SetValue(workers);
        }
    }
}