using Badbarbos.Network;
using Badbarbos.Network.Modules.Http;
using Badbarbos.Network.Modules.Connect;
using Badbarbos.Network.Modules.Members;

using UnityEngine.SceneManagement;
using UnityEngine;

using TMPro;

using System.Linq;

using Badbarbos.Network.Modules.Scenes;

namespace Badbarbos.Menu
{
    public class MenuClient : AClient
    {
        [SerializeField] private string _ip;

        [SerializeField] private int   _port;

        [SerializeField] private const string _menuChooseSceneName = "MenuGame";

        [SerializeField] private const string _menuCreateSceneName = "MenuCreateLobby";

        [SerializeField] private const string _menuJoinSceneName = "MenuJoinLobby";

        private MenuClientUiContainer _menuUi { get => FindFirstObjectByType<MenuClientUiContainer>(); }

        private MenuController _menuController { get => FindFirstObjectByType<MenuController>(); }

        #region Common

        private MembersModuleClient _membersModule;

        private HttpModuleClient _httpModule;

        private ConnectModuleClient _connectModule;

        private ScenesModuleClient _scenesModule;

        #endregion

        #region MenuChoose

        private string _receivedCode;

        #endregion

        private void Awake() => DontDestroyOnLoad(gameObject);

        private void Update()
        {
            TextMeshProUGUI obj = null;

            if (SceneManager.GetActiveScene().name == _menuCreateSceneName) obj = _menuUi.CREATE_PlayerListText;

            if (SceneManager.GetActiveScene().name == _menuJoinSceneName) obj = _menuUi.JOIN_PlayerListText;

            if (obj == null) return;

            obj.text = "Player list: " + string.Join(", ", _membersModule.GetMembers().Values.Select(x => x.Name));
        }

        public override void Initialize()
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case _menuChooseSceneName:
                    MenuChooseInit();
                    break;

                case _menuCreateSceneName:
                    MenuCreateInit();
                    break;

                case _menuJoinSceneName:
                    MenuJoinInit();
                    break;

                default:
                    Destroy(gameObject);
                    break;
            }
        }

        private void MenuChooseInit()
        {
            _membersModule = GetModule<MembersModuleClient>();
            _httpModule = GetModule<HttpModuleClient>();
            _connectModule = GetModule<ConnectModuleClient>();
            _scenesModule = GetModule<ScenesModuleClient>();

            _menuUi.CHOOSE_CreateMenuButton.onClick.AddListener(() =>
            {
                _httpModule.SendCreateRequest($"http://{_ip}:{_port}",
                    (ip, port, lobbyId) =>
                    {
                        _menuUi.CHOOSE_CreateMenuButton.enabled = false;
                        _menuUi.CHOOSE_JoinMenuButton.enabled = false;

                        _connectModule.Connected.RemoveAllListeners();
                        _connectModule.Connected.AddListener(() => { _menuController.CreateLobby(); });
                        _connectModule.Disconnected.AddListener(Exit);

                        _membersModule.Enter();

                        _connectModule.Connect(_ip, (ushort)port);

                        _receivedCode = lobbyId;
                    },
                    (error) => { Debug.LogError(error); Exit(); });
            });

            _menuUi.CHOOSE_JoinMenuButton.onClick.AddListener(() =>
            {
                _httpModule.SendJoinRequest($"http://{_ip}:{_port}", _menuUi.CHOOSE_CodeForFriendField.text,
                    (ip, port, lobbyId) =>
                    {
                        _menuUi.CHOOSE_CreateMenuButton.enabled = false;
                        _menuUi.CHOOSE_JoinMenuButton.enabled = false;

                        _connectModule.Connected.RemoveAllListeners();
                        _connectModule.Connected.AddListener(() => { _menuController.JoinLobby(); });
                        _connectModule.Disconnected.AddListener(Exit);

                        _membersModule.Enter();

                        _connectModule.Connect(_ip, (ushort)port);

                        _receivedCode = lobbyId;
                    },
                    (error) => { Debug.LogError(error); Exit(); });
            });

            _menuUi.CHOOSE_ReturnButton.onClick.AddListener(Exit);
        }

        private void MenuCreateInit()
        {
            _menuUi.CREATE_CodeForFriendText.text = "Code for friend: " + _receivedCode;

            _menuUi.CREATE_ReturnButton.onClick.AddListener(() => { _connectModule.Disconnect(); });

            _menuUi.CREATE_StartButton.onClick.AddListener(() =>
            {
                _menuUi.CREATE_StartButton.enabled = false;

                _httpModule.SendCloseRequest($"http://{_ip}:{_port}", _receivedCode,
                    () => { _scenesModule.NewScene(5); },
                    (error) => { Debug.LogError(error); Exit(); });
            });

            _scenesModule.NewSceneWait();
        }

        private void MenuJoinInit()
        {
            _menuUi.JOIN_CodeForFriendText.text = "Code for friend: " + _receivedCode;

            _menuUi.JOIN_ReturnButton.onClick.AddListener(() => { _connectModule.Disconnect(); });

            _scenesModule.NewSceneWait();
        }

        private void Exit()
        {
            Destroy(FindFirstObjectByType<Network.Network>().gameObject);

            Destroy(gameObject);

            _menuController.ReturnToMainMenu();
        }
    }
}