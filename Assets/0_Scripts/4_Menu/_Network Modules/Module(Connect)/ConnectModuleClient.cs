using UnityEngine;
using UnityEngine.Events;

namespace Badbarbos.Network.Modules.Connect
{
    public class ConnectModuleClient : ANetworkModuleClient
    {
        [HideInInspector] public UnityEvent Connected = new UnityEvent();

        private UnityAction _onConnectedAction;

        [HideInInspector] public UnityEvent Disconnected = new UnityEvent();

        private UnityAction _onDisconnectedAction;

        private void Awake()
        {
            _onConnectedAction = () => Connected.Invoke();
            _onDisconnectedAction = () => Disconnected.Invoke();
        }

        public void Connect(string ip, ushort port)
        {
            _socket.Connected.AddListener(_onConnectedAction);
            _socket.Disconnected.AddListener(_onDisconnectedAction);

            _socket.Connect(ip, port);
        }

        public void Disconnect() => _socket.Disconnect(true);
        

        private void OnDestroy()
        {
            if (_socket != null) _socket.Connected.RemoveListener(_onConnectedAction);
            if (_socket != null) _socket.Disconnected.RemoveListener(_onDisconnectedAction);
        }
    }
}