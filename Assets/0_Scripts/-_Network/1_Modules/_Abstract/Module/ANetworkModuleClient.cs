using ICENet;

using UnityEngine;

namespace Badbarbos.Network
{
    public abstract class ANetworkModuleClient : MonoBehaviour
    {
        protected IceClient _socket;

        public void Setup(IceClient socket) => _socket = socket;
    }
}