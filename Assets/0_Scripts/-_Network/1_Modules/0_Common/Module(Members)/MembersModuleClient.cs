using System.Collections.Generic;
using System.Linq;

using UnityEngine.Events;
using UnityEngine;

namespace Badbarbos.Network.Modules.Members
{
    public class MembersModuleClient : ANetworkModuleClient
    {
        [HideInInspector] public UnityEvent<Dictionary<int, Member>> Update;

        private readonly Dictionary<int, MemberClient> _members = new();

        private int _myId;

        private UnityAction _registerAction;

        public void Enter()
        {
            _registerAction = () => _socket.Send(new MemberRegisterPacket { MemberName = "player" + Random.Range(0, 100) });
            _socket.Connected.AddListener(_registerAction);

            _socket.Traffic.AddOrUpdateHandler<MemberAddedPacket>(MemberAdd);
            _socket.Traffic.AddOrUpdateHandler<MemberRemovedPacket>(MemberRemove);
            _socket.Traffic.AddOrUpdateHandler<MemberAcceptedPacket>(MemberReceiveId);
        }

        private void MemberAdd(MemberAddedPacket packet)
        {
            _members[packet.MemberId] = new MemberClient(packet.MemberName, packet.MemberId);
            Update.Invoke(_members.ToDictionary(kv => kv.Key, kv => (Member)kv.Value));
        }

        private void MemberRemove(MemberRemovedPacket packet)
        {
            _members.Remove(packet.MemberId);
            Update.Invoke(_members.ToDictionary(kv => kv.Key, kv => (Member)kv.Value));
        }

        private void MemberReceiveId(MemberAcceptedPacket packet)
        {
            _myId = packet.MemberId;
        }

        public int GetMyId() => _myId;

        public Member GetMember(int id) => _members[id];

        public Dictionary<int, MemberClient> GetMembers() => _members.ToDictionary(kv => kv.Key, kv => kv.Value);

        private void OnDestroy()
        {
            if (_registerAction != null) _socket.Connected.RemoveListener(_registerAction);
        }
    }
}