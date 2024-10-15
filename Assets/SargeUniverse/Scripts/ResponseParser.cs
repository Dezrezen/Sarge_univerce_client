using System;
using System.Collections.Generic;
using CityBuildingKit.Scripts.Enums;
using Controller;
using SargeUniverse.Scripts.BattleSystem.Data;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model;
using Server;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace SargeUniverse.Scripts
{
    public class ResponseParser
    {
        private readonly INetworkClient _networkClient = null;
        private readonly NetworkPacket _networkPacket = null;
        
        private int _unreadBattleReports = 0;

        public readonly UnityEvent<InitializationData> onReceiveInitializationData = new ();
        public readonly UnityEvent<Data_Player> onReceivePlayerData = new();
        public readonly UnityEvent onBattleFound = new();
        public readonly UnityEvent onResetGame = new();

        [Inject]
        public ResponseParser(
            INetworkClient networkClient, 
            NetworkPacket networkPacket)
        {
            _networkClient = networkClient;
            _networkPacket = networkPacket;
            
            _networkClient.OnPacketReceivedCallback(ReceivedPaket);
        }
        
        public void ReceivedPaket(byte[] bytes)
        {
            ReceivedPaket(new Packet(bytes));
        }
        
        public void ReceivedPaket(Packet packet)
        {
            try
            {
                var id = packet.ReadInt();
                switch ((RequestID)id)
                {
                    case RequestID.Init:
                        InitRequest(packet);
                        break;
                    case RequestID.Auth:
                        AuthResponse(packet);
                        break;
                    case RequestID.Sync:
                        SyncResponse(packet);
                        break;
                    case RequestID.Build:
                        BuildResponse(packet);
                        break;
                    case RequestID.Replace:
                        ReplaceResponse(packet);
                        break;
                    case RequestID.Collect:
                        CollectResponse(packet);
                        break;
                    case RequestID.Upgrade:
                        UpgradeResponse(packet);
                        break;
                    case RequestID.InstantBuild:
                        InstantBuildResponse(packet);
                        break;
                    case RequestID.TrainUnit:
                        TrainUnit(packet);
                        break;
                    case RequestID.CancelTrainUnit:
                        CancelTrainUnit(packet);
                        break;
                    case RequestID.DeleteUnit:
                        DeleteUnit(packet);
                        break;
                    case RequestID.BattleFind:
                        BattleFind(packet);
                        break;
                    case RequestID.BattleStart:
                        BattleStart(packet);
                        break;
                    case RequestID.BattleFrame:
                        BattleFrame(packet);
                        break;
                    case RequestID.BattleEnd:
                        BattleEnd(packet);
                        break;
                    case RequestID.RestartGame:
                        RestartGame(packet);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReceivedPaket exception - " + ex.Message);
                throw;
            }
        }

        private void InitRequest(Packet packet)
        {
            
        }
        
        private void AuthResponse(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((AuthResponseID)response)
            {
                case AuthResponseID.Unknown:
                    break;
                case AuthResponseID.Ok:
                    var bytesLength = packet.ReadInt();
                    var bytes = packet.ReadBytes(bytesLength);
                    _unreadBattleReports = packet.ReadInt();
                    // TODO: Add notification on UI

                    var initializationData = DataUtils.Decompress(bytes).Deserialize<InitializationData>();
                    PlayerPrefs.SetString("password", initializationData.password);
                    
                    onReceiveInitializationData.Invoke(initializationData);
                    _networkClient.SetConnectionState(true);
                    
                    UIManager.Instanse.ShowMainUI();
                    _networkPacket.SyncRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SyncResponse(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((SyncResponseID)response)
            {
                case SyncResponseID.Unknown:
                    break;
                case SyncResponseID.Ok:
                    var bytesLength = packet.ReadInt();
                    var bytes = packet.ReadBytes(bytesLength);
                    var data = DataUtils.Decompress(bytes);
                    var syncData = data.Deserialize<Data_Player>();
                    
                    onReceivePlayerData.Invoke(syncData);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void BuildResponse(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((BuildResponseID)response)
            {
                case BuildResponseID.Unknown:
                    // TODO: Log "Unknown"
                    break;
                case BuildResponseID.Ok:
                    _networkPacket.SyncRequest();
                    break;
                case BuildResponseID.NoResources:
                    BuildingsManager.Instanse.CancelBuild();
                    // TODO: Log "No Resources"
                    break;
                case BuildResponseID.MaxLevel:
                    BuildingsManager.Instanse.CancelBuild();
                    // TODO: Log "Max Level"
                    break;
                case BuildResponseID.PlaceTaken:
                    // TODO: Log "Place Taken"
                    break;
                case BuildResponseID.NoBuilder:
                    BuildingsManager.Instanse.CancelBuild();
                    // TODO: Show message "No Builder"
                    break;
                case BuildResponseID.Limit:
                    BuildingsManager.Instanse.CancelBuild();
                    // TODO: Show message "Max limit reached"
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ReplaceResponse(Packet packet)
        {
            var response = packet.ReadInt();

            switch ((ReplaceResponseID)response)
            {
                case ReplaceResponseID.NoBuilding:
                    UIManager.Instanse.ShowMessage("No Building");
                    break;
                case ReplaceResponseID.Ok:
                    var xPos = packet.ReadInt();
                    var yPos = packet.ReadInt();
                    var databaseId = packet.ReadLong();
                    BuildingsManager.Instanse.MoveBuilding(databaseId, xPos, yPos);
                    break;
                case ReplaceResponseID.PlaceTaken:
                    UIManager.Instanse.ShowMessage("Place Taken");
                    break;
            }
        }

        private void CollectResponse(Packet packet)
        {
            var buildingID = packet.ReadLong();
            var collected = packet.ReadInt();

            BuildingsManager.Instanse.CollectResources(buildingID, collected);
            _networkPacket.SyncRequest();
        }

        private void UpgradeResponse(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((UpgradeResponseID)response)
            {
                case UpgradeResponseID.Unknown:
                    // TODO: Log "Unknown"
                    break;
                case UpgradeResponseID.Ok:
                    _networkPacket.SyncRequest();
                    break;
                case UpgradeResponseID.NoResources:
                    // TODO: Log "No Resources"
                    break;
                case UpgradeResponseID.MaxLevel:
                    // TODO: Log "Max Level"
                    break;
                case UpgradeResponseID.NoBuilder:
                    // TODO: Show message "No Builder"
                    break;
                case UpgradeResponseID.Limit:
                    // TODO: Show message "Max limit reached"
                    break;
            }
        }

        private void InstantBuildResponse(Packet packet)
        {
            var response = packet.ReadInt();
            switch (response)
            {
                case 0:
                    // TODO: Log "Unknown"
                    break;
                case 1:
                    _networkPacket.SyncRequest();
                    break;
                case 2:
                    UIManager.Instanse.ShowMessage("No Gems");
                    break;
            }
        }

        private void TrainUnit(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((TrainUnitResponseID)response)
            {
                case TrainUnitResponseID.Unknown:
                    UIManager.Instanse.ShowMessage("TrainUnit - Unknown");
                    break;
                case TrainUnitResponseID.Ok:
                    _networkPacket.SyncRequest();
                    break;
                case TrainUnitResponseID.NoResources:
                    UIManager.Instanse.ShowMessage("No resources");
                    break;
                case TrainUnitResponseID.NoCapacity:
                    UIManager.Instanse.ShowMessage("No capacity");
                    break;
                case TrainUnitResponseID.ServerUnitNotFound:
                    UIManager.Instanse.ShowMessage("Server unit not found");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CancelTrainUnit(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((CancelTrainResponseID)response)
            {
                case CancelTrainResponseID.Unknown:
                    UIManager.Instanse.ShowMessage("CancelTrainUnit - Unknown");
                    break;
                case CancelTrainResponseID.Ok:
                    var databaseId = packet.ReadLong();
                    UnitsManager.Instanse.CancelTrain(databaseId);
                    _networkPacket.SyncRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DeleteUnit(Packet packet)
        {
            var response = packet.ReadInt();
            switch ((DeleteUnitResponseID)response)
            {
                case DeleteUnitResponseID.Unknown:
                    UIManager.Instanse.ShowMessage("DeleteUnitResponseID - Unknown");
                    break;
                case DeleteUnitResponseID.Ok:
                    var databaseId = packet.ReadLong();
                    UnitsManager.Instanse.DeleteUnit(databaseId);
                    _networkPacket.SyncRequest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void BattleFind(Packet packet)
        {
            var opponentId = packet.ReadLong();
            OpponentData opponentData = null;
            if (opponentId > 0)
            {
                var bytesLength = packet.ReadInt();
                var bytes = packet.ReadBytes(bytesLength);
                opponentData = DataUtils.Decompress(bytes).Deserialize<OpponentData>();
            }
            //PlayerSyncController.Instance.EnterBattleMode();
            BattleSync.Instance().SetOpponentData(opponentId, opponentData);
            onBattleFound.Invoke();
            //SceneManager.LoadScene(Constants.BattleScene);
        }

        private void BattleStart(Packet packet)
        {
            var matched = packet.ReadBool();
            var attack = packet.ReadBool();
            var confirmed = matched && attack;
            List<BattleStartBuildingData> buildings = null;
            var winTrophies= 0;
            var loseTrophies = 0;
            if (confirmed)
            {
                winTrophies = packet.ReadInt();
                loseTrophies = packet.ReadInt();
                var bytesLength = packet.ReadInt();
                var bytes = packet.ReadBytes(bytesLength);
                buildings = DataUtils.Decompress(bytes).Deserialize<List<BattleStartBuildingData>>();
            }
            BattleControl.Instance.StartBattleConfirm(confirmed, buildings, winTrophies, loseTrophies);
        }

        private void BattleFrame(Packet packet)
        {
            
        }

        private void BattleEnd(Packet packet)
        {
            var stars = packet.ReadInt();
            var unitsDeployed = packet.ReadInt();
            var lootedSupplies = packet.ReadInt();
            var lootedPower = packet.ReadInt();
            var lootedEnergy = packet.ReadInt();
            var trophies = packet.ReadInt();
            var frame = packet.ReadInt();
            BattleControl.Instance.FinishBattle(
                stars, 
                unitsDeployed, 
                lootedSupplies, 
                lootedPower, 
                lootedEnergy, 
                trophies, 
                frame
            );
        }

        private void RestartGame(Packet packet)
        {
            var responce = packet.ReadInt();
            if (responce == 1)
            {
                _networkClient.GetWsClient().CloseConnection();
                onResetGame.Invoke();
            }
        }
    }
}