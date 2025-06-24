using UnityEngine;
using UnityEngine.Events;

namespace Badbarbos.Network.Modules.Entities
{
    public class EntitiesModuleClient : ANetworkModuleClient
    {
        [HideInInspector] public UnityEvent<int> YouEnteredProp;

        [HideInInspector] public UnityEvent<int, int> AnotherEnteredProp;

        [HideInInspector] public UnityEvent<int, int, byte[]> PropDataReceived;

        [HideInInspector] public UnityEvent<int, int> GegActivated;

        public void Enter()
        {
            _socket.Traffic.AddOrUpdateHandler<PropEnterResponse>(ReceivePropEnterResponse);
            _socket.Traffic.AddOrUpdateHandler<PropAnotherEntered>(ReceivePropAnotherEntered);
            _socket.Traffic.AddOrUpdateHandler<ServerToClientPropDataPacket>(ServerToClientPropDataPacket);
            _socket.Traffic.AddOrUpdateHandler<GegActivationResponse>(GegActivationResponse);
        }

        private void ReceivePropEnterResponse(PropEnterResponse packet) => YouEnteredProp.Invoke(packet.EntityId);

        private void ReceivePropAnotherEntered(PropAnotherEntered packet) => AnotherEnteredProp.Invoke(packet.MemberId, packet.EntityId);

        private void ServerToClientPropDataPacket(ServerToClientPropDataPacket packet) => PropDataReceived.Invoke(packet.MemberId, packet.EntityId, packet.Buffer);

        private void GegActivationResponse(GegActivationResponse packet) => GegActivated.Invoke(packet.MemberId, packet.EntityId);

        public void SendPropEnterRequest(int entityId) => _socket.Send(new PropEnterRequest { EntityId = entityId });

        public void SendClientToServerPropDataPacket(byte[] buffer) => _socket.Send(new ClientToServerPropDataPacket { Buffer = buffer });

        public void SendGegActivationRequest(int entityId) => _socket.Send(new GegActivationRequest { EntityId = entityId });
    }
}
