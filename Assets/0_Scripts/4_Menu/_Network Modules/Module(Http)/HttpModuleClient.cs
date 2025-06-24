using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

namespace Badbarbos.Network.Modules.Http
{
    public class HttpModuleClient : ANetworkModuleClient
    {
        [Serializable]
        private class LobbyResponse
        {
            public string ip;
            public int port;
            public string lobbyId;
        }

        public void SendCreateRequest(string serverUrl, Action<string, int, string> onSuccess, Action<string> onError)
        {
            StartCoroutine(CreateLobbyCoroutine(serverUrl, onSuccess, onError));
        }

        public void SendJoinRequest(string serverUrl, string lobbyId, Action<string, int, string> onSuccess, Action<string> onError)
        {
            StartCoroutine(JoinLobbyCoroutine(serverUrl, lobbyId, onSuccess, onError));
        }

        public void SendCloseRequest(string serverUrl, string lobbyId, Action onComplete, Action<string> onError)
        {
            StartCoroutine(CloseLobbyCoroutine(serverUrl, lobbyId, onComplete, onError));
        }

        private IEnumerator CreateLobbyCoroutine(string serverUrl, Action<string, int, string> onSuccess, Action<string> onError)
        {
            using var req = UnityWebRequest.PostWwwForm($"{serverUrl.TrimEnd('/')}/lobby/create", "");
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<LobbyResponse>(req.downloadHandler.text);
                onSuccess?.Invoke(resp.ip, resp.port, resp.lobbyId);
            }
            else onError?.Invoke(req.error);
        }

        private IEnumerator JoinLobbyCoroutine(string serverUrl, string lobbyId, Action<string, int, string> onSuccess, Action<string> onError)
        {
            using var req = UnityWebRequest.Get($"{serverUrl.TrimEnd('/')}/lobby/find/{lobbyId}");
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                var resp = JsonUtility.FromJson<LobbyResponse>(req.downloadHandler.text);
                onSuccess?.Invoke(resp.ip, resp.port, resp.lobbyId);
            }
            else onError?.Invoke(req.error);
        }

        private IEnumerator CloseLobbyCoroutine(string serverUrl, string lobbyId, Action onComplete, Action<string> onError)
        {
            using var req = UnityWebRequest.PostWwwForm($"{serverUrl.TrimEnd('/')}/lobby/close/{lobbyId}", "");
            req.downloadHandler = new DownloadHandlerBuffer();
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success) onComplete?.Invoke();
            else  onError?.Invoke(req.error);
        }
    }
}
