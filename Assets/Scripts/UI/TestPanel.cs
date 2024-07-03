using System;
using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class TestPanel : BasePanel<TestPanel>
    {
        private readonly NetworkManager _networkManager = FindObjectOfType<NetworkManager>();
        private TextMeshProUGUI _ipPortText;
        private TMP_InputField _ipInput;
        private TMP_InputField _portInput;
        private Button _joinRoom;
        private Button _createRoom;
        private LocalConnectionState _clientState;
        private LocalConnectionState _serverState;

        protected override void Awake()
        {
            base.Awake();
            Init();
            _createRoom.onClick.AddListener(() =>
            {
                if (_networkManager == null)
                    return;

                if (_serverState != LocalConnectionState.Stopped)
                    _networkManager.ServerManager.StopConnection(true);
                else
                {
                    if (string.IsNullOrEmpty(_portInput.text))
                        _networkManager.ServerManager.StartConnection();
                    else
                        _networkManager.ServerManager.StartConnection(ushort.Parse(_portInput.text));
                }
            });
            _joinRoom.onClick.AddListener(() =>
            {
                if (_networkManager == null)
                    return;

                if (_clientState != LocalConnectionState.Stopped)
                    _networkManager.ClientManager.StopConnection();
                else
                {
                    if (string.IsNullOrEmpty(_ipInput.text))
                        _networkManager.ClientManager.StartConnection();
                    else
                    {
                        if (string.IsNullOrEmpty(_portInput.text))
                            _networkManager.ClientManager.StartConnection(_ipInput.text);
                        else
                            _networkManager.ClientManager.StartConnection(_ipInput.text, ushort.Parse(_portInput.text));
                    }
                }
            });
        }

        void Init()
        {
            _ipPortText = GetControl<TextMeshProUGUI>("ipPort");
            _ipInput = GetControl<TMP_InputField>("ipInput");
            _portInput = GetControl<TMP_InputField>("portInput");
            _joinRoom = GetControl<Button>("join");
            _joinRoom = GetControl<Button>("create");
        }

        private void Start()
        {
            _networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        private void OnDestroy()
        {
            _networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
        }

        private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
        {
            _clientState = obj.ConnectionState;
        }


        private void ServerManager_OnServerConnectionState(ServerConnectionStateArgs obj)
        {
            _serverState = obj.ConnectionState;
        }
    }
}