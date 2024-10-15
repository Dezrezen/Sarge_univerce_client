using System;
using UnityEngine;
using Server;
using Server.Scripts;
using UnityEngine.SceneManagement;
using Zenject;

namespace SargeUniverse.Scripts
{
    public interface INetworkClient
    {
        WebSocketClient GetWsClient();
        void OnPacketReceivedCallback(Action<byte[]> callback);
        
        void SetConnectionState(bool value);
        bool GetConnectionState();
    }
    
    public class NetworkClient : MonoBehaviour, INetworkClient
    {
        private WebSocketClient _wsClient = null;
        
        private Settings _settings = null;
        private bool _connected = false;
        
        [Inject]
        private void Construct(Settings settings)
        {
            _settings = settings;
            StartConnection();
        }
        
        private void Awake()
        {
            Application.runInBackground = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Update()
        {
            _wsClient?.Update();
        }

        private void OnDestroy()
        {
            if (_wsClient == null)
            {
                return;
            }
            
            _wsClient.OnDisconnectedFromServer -= DisconnectedFromServer;
            _wsClient.DisconnectFromServer();
        }

        private void OnApplicationQuit()
        {
            _wsClient?.CloseConnection();
        }
        
        private void StartConnection()
        {
            _wsClient = new WebSocketClient(_settings.IP, _settings.Port);
            _wsClient.OnConnectToServer += () => Debug.Log("ConnectToServer - OK");
            _wsClient.OnDisconnectedFromServer += DisconnectedFromServer;
            // _wsClient.OnPacketReceived += _responseParser.ReceivedPaket;
        }

        private void DisconnectedFromServer()
        {
            ThreadDispatcher.instance.Enqueue(Disconnected);
        }
        
        private void Disconnected()
        {
            _connected = false;
            RealtimeNetworking.OnDisconnectedFromServer -= DisconnectedFromServer;
            /*MessageBox.Open(0, 
                0.8f, 
                false,
                (layoutIndex, buttonIndex) =>
                {
                    if (layoutIndex == 0 && buttonIndex == 0)
                    {
                        SceneManager.LoadScene(sceneBuildIndex: 0);
                    }
                }, 
                new[] { "Failed to connect to server." }, 
                new[] { "Connect" }
            );*/
        }

        public WebSocketClient GetWsClient()
        {
            return _wsClient;
        }

        public void OnPacketReceivedCallback(Action<byte[]> callback)
        {
            _wsClient.OnPacketReceived += callback;
        }
        
        public void SetConnectionState(bool value)
        {
            _connected = value;
        }

        public bool GetConnectionState()
        {
            return _connected;
        }
    }
}
