using System;
using NativeWebSocket;
using UnityEngine;

namespace Server
{
    public class WebSocketClient
    {
        private WebSocket _webSocket = null;
        private readonly string _ip;
        private readonly int _port;

        public Action OnConnectToServer;
        public Action OnDisconnectedFromServer;
        public Action<string> OnErrorMessage;
        public Action<byte[]> OnPacketReceived;

        public WebSocketClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }
        
        public void ConnectToServer()
        {
            _webSocket = new WebSocket("ws://" + _ip + ":" + _port + "/");
            SubscribeForEvents();
            _webSocket.Connect();
        }

        public void DisconnectFromServer()
        {
            UnsubscribeFromEvents();
            _webSocket.Close();
        }
            
        public void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket?.DispatchMessageQueue();
#endif
        }

        private void SubscribeForEvents()
        {
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnMessage += OnMessage;
            _webSocket.OnError += OnError;
            _webSocket.OnClose += OnClose;
        }

        private void UnsubscribeFromEvents()
        {
            _webSocket.OnOpen -= OnOpen;
            _webSocket.OnMessage -= OnMessage;
            _webSocket.OnError -= OnError;
            _webSocket.OnClose -= OnClose;
        }
        
        public void CloseConnection()
        {
            _webSocket?.Close();
        }
        
        private void OnOpen()
        {
            OnConnectToServer.Invoke();
        }
        
        private void OnMessage(byte[] bytes)
        {
            OnPacketReceived.Invoke(bytes);
        }
        
        private void OnError(string errorMsg)
        {
            Debug.Log("Error! " + errorMsg);
            OnErrorMessage.Invoke(errorMsg);
        }
        
        private void OnClose(WebSocketCloseCode closeCode)
        {
            UnsubscribeFromEvents();
            OnDisconnectedFromServer.Invoke();
        }

        public void SendData(byte[] bytes)
        {
            _webSocket.Send(bytes);
        }
    }
}