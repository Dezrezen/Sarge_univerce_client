using System;
using System.Collections.Generic;
using Config;
using SargeUniverse.Common.View;
using SargeUniverse.Scripts.BattleSystem.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Map;
using SargeUniverse.Scripts.Model;
using UnityEngine;
using UnityEngine.Events;
using WebSocketSharp;
using Zenject;
using BattleBuildingData = SargeUniverse.Scripts.Model.Battle.BattleBuildingData;
using BattleUnitData = SargeUniverse.Scripts.Model.Battle.BattleUnitData;
using Random = UnityEngine.Random;

namespace SargeUniverse.Scripts.Controller
{
    public class BattleControl : MonoBehaviour
    {
        private class UnitToSpawn
        {
            public readonly UnitID id;
            public readonly long databaseId;
            public readonly int x;
            public readonly int y;

            public UnitToSpawn(UnitID id, long databaseId, int x, int y)
            {
                this.id = id;
                this.databaseId = databaseId;
                this.x = x;
                this.y = y;
            }
        }

        [SerializeField] private UnitsConfig _unitsConfig = null;
        [SerializeField] private BattleMap _grid = null;
        
        //private BattleSystem.Battle _battle = null;
        private BattleFix _battle = null;
        private List<BattleBuildingData> _battleBuildings = new();
        private List<BattleUnitData> _defUnits = new();

        private Queue<UnitToSpawn> _unitsToSpawn = new();
        
        private UnityEvent<UnitID> _deployCallback = new();
        public UnitID DeployUnitId { get; private set; } = UnitID.empty;


        private DateTime _baseTime;
        private bool _surrended = false;

        public static BattleControl Instance { get; private set; } = null;
        
        private NetworkPacket _networkPacket;
        
        [Inject]
        private void Construct(NetworkPacket networkPacket)
        {
            _networkPacket = networkPacket;
        }
        
        private void Awake()
        {
            Instance ??= this;

            _battleBuildings = new List<BattleBuildingData>();
            _defUnits = new List<BattleUnitData>();
        }

        public void Stop()
        {
            BattleSync.Instance().SetBattlePhase(BattlePhase.End);
        }
        
        private void Start()
        {
            _baseTime = DateTime.Now;
            LoadBattleMap();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            if (_battle != null && BattleSync.Instance().Phase != BattlePhase.End)
            {
                if (BattleSync.Instance().Phase != BattlePhase.Start)
                {
                    var span = DateTime.Now - _baseTime;
                    
                    // TODO: Update Time text on battle UI
                    
                    var frame = (int)Math.Floor(span.TotalSeconds / Constants.BattleFrameRate);
                    if (frame > _battle.FrameCount)
                    {
                        // TODO: To add Units

                        if (_unitsToSpawn.Count > 0)
                        {
                            /*var battleFrame = new BattleFrame();
                            battleFrame.frame = _battle.frameCount + 1;*/
                            
                            // Add Unit
                            if (_unitsToSpawn.Count > 0)
                            {
                                while (_unitsToSpawn.Count > 0)
                                {
                                    var unitToSpawn = _unitsToSpawn.Dequeue();
                                    var unitData = BattleSync.Instance().GetUnit(unitToSpawn.databaseId);
                                    _battle.AddDeployUnit(
                                        unitData,
                                        unitToSpawn.x, 
                                        unitToSpawn.y, 
                                        UnitSpawnCallBack, 
                                        UnitAttackCallBack, 
                                        UnitDiedCallBack, 
                                        UnitDamageCallBack, 
                                        UnitHealCallBack, 
                                        UnitTargetSelectedCallBack,
                                        UnitIdleCallBack,
                                        PositionCallback);
                                }
                            }
                            
                            // Add Ability
                        }
                        
                        /*if (toAddUnits.Count > 0 || toAddSpells.Count > 0)
                        {
                            Data.BattleFrame battleFrame = new Data.BattleFrame();
                            battleFrame.frame = battle.frameCount + 1;

                            if (toAddUnits.Count > 0)
                            {
                                for (int i = toAddUnits.Count - 1; i >= 0; i--)
                                {
                                    for (int j = 0; j < Player.instanse.data.units.Count; j++)
                                    {
                                        if (Player.instanse.data.units[j].databaseID == toAddUnits[i].id)
                                        {
                                            battle.AddUnit(Player.instanse.data.units[j], toAddUnits[i].x,
                                                toAddUnits[i].y, UnitSpawnCallBack, UnitAttackCallBack,
                                                UnitDiedCallBack, UnitDamageCallBack, UnitHealCallBack,
                                                UnitTargetSelectedCallBack);
                                            Data.BattleFrameUnit bfu = new Data.BattleFrameUnit();
                                            bfu.id = Player.instanse.data.units[j].databaseID;
                                            bfu.x = toAddUnits[i].x;
                                            bfu.y = toAddUnits[i].y;
                                            battleFrame.units.Add(bfu);
                                            break;
                                        }
                                    }

                                    toAddUnits.RemoveAt(i);
                                }
                            }

                            Packet packet = new Packet();
                            packet.Write((int)Player.RequestsID.BATTLEFRAME);
                            byte[] bytes = Data.Compress(Data.Serialize<Data.BattleFrame>(battleFrame));
                            packet.Write(bytes.Length);
                            packet.Write(bytes);
                            Sender.TCP_Send(packet);
                        }*/
                        
                        _battle.ExecuteFrame(Time.deltaTime);/*
                        if (_battle.frameCount * Constants.BattleFrameRate >= _battle.duration ||
                            Math.Abs(_battle.percentage - 1d) <= Mathf.Epsilon)
                        {
                            EndBattle(false, _battle.frameCount);
                        }
                        else if (_surrended || (!_battle.IsAliveUnitsOnGrid() && !HaveUnitsLeftToPlace()))
                        {
                            EndBattle(true, _battle.frameCount);
                        }*/
                    }
                }
                else
                {
                    var span = DateTime.Now - _baseTime;
                    if (span.TotalSeconds >= Constants.BattlePrepDuration)
                    {
                        StartBattle();
                    }
                    else
                    {
                        // TODO: Show Preparation Timer
                        //_timerText.text = TimeSpan.FromSeconds(Data.battlePrepDuration - span.TotalSeconds).ToString(@"mm\:ss");
                    }
                }
            }

            UpdateUnits();
        }

        private void StartBattle()
        {
            SendStartBattleRequest();
        }

        public void StartBattleConfirm(
            bool confirmed, 
            List<BattleStartBuildingData> buildings, 
            int winTrophies,
            int loseTrophies)
        {
            if (confirmed)
            {
                /*_battle.winTrophies = winTrophies;
                _battle.loseTrophies = loseTrophies;*/
                /*for (int i = 0; i < _battle._buildings.Count; i++)
                {
                    bool resource = false;
                    switch (battle._buildings[i].building.id)
                    {
                        case Data.BuildingID.townhall:
                        case Data.BuildingID.goldmine:
                        case Data.BuildingID.goldstorage:
                        case Data.BuildingID.elixirmine:
                        case Data.BuildingID.elixirstorage:
                        case Data.BuildingID.darkelixirmine:
                        case Data.BuildingID.darkelixirstorage:
                            resource = true;
                            break;
                    }
                    if (!resource)
                    {
                        continue;
                    }
                    for (int j = 0; j < buildings.Count; j++)
                    {
                        if (battle._buildings[i].building.databaseID != buildings[j].databaseID)
                        {
                            continue;
                        }
                        battle._buildings[i].lootGoldStorage = buildings[j].lootGoldStorage;
                        battle._buildings[i].lootElixirStorage = buildings[j].lootElixirStorage;
                        battle._buildings[i].lootDarkStorage = buildings[j].lootDarkStorage;
                        break;
                    }
                }*/
                BattleSync.Instance().SetBattlePhase(BattlePhase.Start);
            }
        }

        private async void SendStartBattleRequest()
        {
            var data = await BattleSync.Instance().GetOpponentData().SerializeAsync();
            var opponentData = await DataUtils.CompressAsync(data);
            _networkPacket.SendStartBattleRequest(opponentData, 1);
        }

        private void EndBattle(bool suirrended, int surrendedFrame)
        {
            /*
            _findButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(false);
            _surrenderButton.gameObject.SetActive(false);
            battle.end = true;
            battle.surrender = surrender;
            isStarted = false;
            Packet packet = new Packet();
            packet.Write((int)Player.RequestsID.BATTLEEND);
            packet.Write(surrender);
            packet.Write(surrenderFrame);
            Sender.TCP_Send(packet);
            */ 
        }
        
        public void FinishBattle(int stars, int unitsDeployed, int lootedSupplies, int lootedPower, int lootedEnergy, int trophies, int frame)
        {
            // _battlePhase = BattlePhase.End;
            
            _deployCallback.RemoveAllListeners();
        }

        public void Surrender()
        {
            
        }

        public void SetPlayerData()
        {
            
        }

        private void LoadBattleMap()
        {
            var opponentData = BattleSync.Instance().GetOpponentData();
            var hqLevel = opponentData.buildings.Find(b => b.id == BuildingID.hq).level;
            
            foreach (var buildingData in opponentData.buildings)
            {
                var battleBuilding = new BattleBuildingData(buildingData);
                
                /*battleBuilding.Initialize();
                battleBuilding.InitStoredResources(hqLevel);
                battleBuilding.buildingData.x += Constants.BattleGridOffset;
                battleBuilding.buildingData.y += Constants.BattleGridOffset;*/
                
                _battleBuildings.Add(battleBuilding);
                
                _grid.AddBuilding(buildingData);
            }

            var campPosition = _grid.GetCampPosition();
            foreach (var unitData in opponentData.data.units)
            {
                var battleUnit = new BattleUnitData(unitData, campPosition.x, campPosition.y);
                /*{
                    dataUnit = unitData,
                    health = unitData.health,
                    defMode = true
                };*/
                /*battleUnit.Initialize(campPosition.x, campPosition.y);*/
                _defUnits.Add(battleUnit);
                
                _grid.AddDefenceUnit(unitData, campPosition.x, campPosition.y);
            }

            _battle = new BattleFix();
            _battle.Init(
                _battleBuildings, 
                _defUnits, 
                BuildingAttackCallBack, 
                BuildingDestroyedCallBack, 
                BuildingDamageCallBack,
                UnitAttackCallBack,
                UnitDiedCallBack,
                UnitDamageCallBack,
                UnitHealCallBack,
                UnitTargetSelectedCallBack,
                UnitIdleCallBack,
                ProjectileCallback,
                PositionCallback);
        }

        public void SetDeployUnit(UnitID deployUnitId, UnityAction<UnitID> callback)
        {
            DeployUnitId = deployUnitId;
            _deployCallback.RemoveAllListeners();
            _deployCallback.AddListener(callback);
        }

        public void DeployUnit(int x, int y)
        {
            if (_battle != null)
            {
                var unitData = BattleSync.Instance().GetUnitToDeploy(DeployUnitId);
                _unitsToSpawn.Enqueue(new UnitToSpawn(unitData.id, unitData.databaseID, x, y));
                _deployCallback.Invoke(unitData.id);
            }
        }

        private bool HaveUnitsLeftToPlace()
        {
            // TODO: Check if we have units to deploy
            return true;
        }

        private void UpdateUnits()
        {
            /*var units = _grid.GetAllUnits();
            foreach (var unitData in _battle.DefenceUnits)
            {
                if (unitData.health > 0)
                {
                    var unit = units.Find(u => u.DatabaseId == unitData.dataUnit.databaseID);
                    var positionOnGrid = unitData.PositionOnGrid();
                    unit.UpdateTargetPosition(unitData.moving, positionOnGrid.x, positionOnGrid.y);
                }
            }
            
            foreach (var unitData in _battle.AttackUnits)
            {
                if (unitData.health > 0)
                {
                    var unit = units.Find(u => u.DatabaseId == unitData.dataUnit.databaseID);
                    var positionOnGrid = unitData.PositionOnGrid();
                    unit.UpdateTargetPosition(unitData.moving, positionOnGrid.x, positionOnGrid.y);
                }
            }*/
        }
        
        // Callbacks
        
        private void UnitSpawnCallBack(long databaseId, float x, float y)
        {
            var unitData = BattleSync.Instance().GetUnit(databaseId);
            if (unitData != null)
            {
                _grid.PlaceUnit(unitData, x, y);
            }
        }

        public void UnitAttackCallBack(long id, long targetId, TargetType targetType)
        {
            var unit = _grid.GetAllUnits().Find(u => u.DatabaseId == id);
            if (targetType == TargetType.Building)
            {
                var b = _grid.GetBuilding(targetId);
                var offset = _grid.GetCenterPosition(b.currentX, b.currentY, b.Width, b.Height) - _grid.GetStartPosition(b.currentX, b.currentY); 
                //var offset = _grid.GetRandomPositionInZone(b.currentX, b.currentY, b.Width, b.Height);
                unit.Attack(b.transform, offset);
            }
            else
            {
                var u = _grid.GetAllUnits().Find(u => u.DatabaseId == targetId);
                unit.Attack(u.transform, Vector3.zero);
            }
            
            /*int u = -1;
            int b = -1;
            for (int i = 0; i < unitsOnGrid.Count; i++)
            {
                if (unitsOnGrid[i].databaseID == unitDatabaseId)
                {
                    u = i;
                    break;
                }
            }
            if (u >= 0)
            {
                for (int i = 0; i < buildingsOnGrid.Count; i++)
                {
                    if (buildingsOnGrid[i].building.databaseID == targetDatabaseId)
                    {
                        b = i;
                        break;
                    }
                }
                if (b >= 0)
                {
                    if (unitsOnGrid[u].projectilePrefab && unitsOnGrid[u].shootPoint && unitsOnGrid[u].data.attackRange > 0f && unitsOnGrid[u].data.rangedSpeed > 0f)
                    {
                        UI_Projectile projectile = Instantiate(unitsOnGrid[u].projectilePrefab);
                        projectile.Initialize(unitsOnGrid[u].shootPoint.position, buildingsOnGrid[b].building.shootTarget, unitsOnGrid[u].data.rangedSpeed * UI_Main.instanse._grid.cellSize);
                    }
                    unitsOnGrid[u].Attack(buildingsOnGrid[b].building.transform.position);
                }
                else
                {
                    unitsOnGrid[u].Attack();
                }
            }*/
        }

        public void UnitDiedCallBack(long databaseId)
        {
            var unit = _grid.GetAttackUnit(databaseId);
            if (unit != null)
            {
                //_grid.RemoveAttackUnit(unit);
            }
            else
            {
                unit = _grid.GetDefenceUnit(databaseId);
                //_grid.RemoveDefenceUnit(unit);
            }

            unit.Die();
        }
        
        public void UnitDamageCallBack(long targetId, int damage)
        {
            var u = _grid.GetAllUnits().Find(u => u.DatabaseId == targetId);
            u.TakeDamage(damage);
        }
        
        public void UnitHealCallBack(long targetId, int value)
        {

        }
        
        public void UnitTargetSelectedCallBack(long id)
        {
            /*int targetIndex = -1;
            for (int i = 0; i < battle._units.Count; i++)
            {
                if (battle._units[i].unit.databaseID == id)
                {
                    targetIndex = battle._units[i].target;
                    break;
                }
            }

            if(targetIndex >= 0 && battle._buildings.Count > targetIndex)
            {
                long buildingID = battle._buildings[targetIndex].building.databaseID;
                for (int i = 0; i < buildingsOnGrid.Count; i++)
                {
                    if (buildingsOnGrid[i].building.databaseID == buildingID)
                    {
                        //Vector3 pos = buildingsOnGrid[i].transform.position; // This is the target
                        // You can instantiate target point here and delete it after a few seconds for example:
                        // Transform tp = Instantiate(prefab, ...)
                        // Destroy(tp.gameObject, 2f);
                        break;
                    }
                }
            }*/
        }
        
        private void UnitIdleCallBack(long databaseId)
        {
            var unit = _grid.GetAllUnits().Find(u => u.DatabaseId == databaseId);
            unit.Idle();
        }

        private void ProjectileCallback(long targetId, TargetType targetType, int damage)
        {
            if (targetType == TargetType.Building)
            {
                var b = _grid.GetBuilding(targetId);
                b.TakeDamage(damage);
            }
            else
            {
                var u = _grid.GetAllUnits().Find(u => u.DatabaseId == targetId);
                u.TakeDamage(damage);
            }
        }

        private void PositionCallback(long id, Vector2 position)
        {
            _grid.GetAllUnits().Find(u => u.DatabaseId == id).UpdateTargetPosition(true, position.x, position.y);
        }

        private void BuildingAttackCallBack(long buildingId, long targetId, TargetType targetType)
        {
            var building = _grid.GetBuilding(buildingId);
            var unit = _grid.GetAttackUnit(targetId);
            building.Attack(unit.transform);
            // TODO: Building attack
            /*int u = -1;
            int b = -1;
            for (int i = 0; i < buildingsOnGrid.Count; i++)
            {
                if (buildingsOnGrid[i].id == id)
                {
                    if (buildingsOnGrid[i].building.data.radius > 0 && buildingsOnGrid[i].building.data.rangedSpeed > 0)
                    {
                        b = i;
                    }
                    break;
                }
            }
            if (b >= 0)
            {
                for (int i = 0; i < unitsOnGrid.Count; i++)
                {
                    if (unitsOnGrid[i].databaseID == target)
                    {
                        u = i;
                        break;
                    }
                }
                if (u >= 0)
                {
                    buildingsOnGrid[b].building.LookAt(unitsOnGrid[u].transform.position);
                    UI_Projectile projectile = buildingsOnGrid[b].building.GetProjectile();
                    Transform muzzle = buildingsOnGrid[b].building.GetMuzzle();
                    if (projectile != null && muzzle != null)
                    {
                        projectile = Instantiate(projectile, muzzle.position, Quaternion.LookRotation(unitsOnGrid[u].transform.position - muzzle.position, Vector3.up));
                        projectile.Initialize(muzzle.position, unitsOnGrid[u].targetPoint != null ? unitsOnGrid[u].targetPoint : unitsOnGrid[u].transform, buildingsOnGrid[b].building.data.rangedSpeed * UI_Main.instanse._grid.cellSize, UI_Projectile.GetCutveHeight(buildingsOnGrid[b].building.id));
                    }
                }
            }*/
        }

        private void BuildingDestroyedCallBack(long value)
        {
            /*if (percentage > 0)
            {
                // TODO: Update precentage info on UI
            }*/
            
            // TODO: Play destroy building animation
            _grid.GetBuilding(value).DestroyBuilding();
        }

        private void BuildingDamageCallBack(long buildingId, int damage)
        {
            var building = _grid.GetBuilding(buildingId);
            building.TakeDamage(damage);

            // TODO: Update loot info on UI
        }

        private void StarGained()
        {
            //var stars = _battle.GetStars();
            // TODO: Update Stars Info on UI
        }
        
        /*private void BuildingProjectileCallback(int databaseId, BattleVector2 current, BattleVector2 target)
        {
            // TODO: Spawn Projectiles
        }*/
    }
}