using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Badbarbos.Network.Modules.Scenes
{
    public class ScenesModuleClient : ANetworkModuleClient
    {
        [HideInInspector] public UnityEvent<int> SceneChanged;

        public void NewSceneWait()
        {
            _socket.Traffic.AddOrUpdateHandler<NewScenePacketResponse>(NewScene);
        }

        public void NewScene(int scene)
        {
            _socket.Send(new NewScenePacketRequest { SceneId = scene });
        }

        private void NewScene(NewScenePacketResponse packet)
        {
            SceneChanged.Invoke(packet.SceneId);
            SceneManager.LoadScene(packet.SceneId);
        }
    }
}