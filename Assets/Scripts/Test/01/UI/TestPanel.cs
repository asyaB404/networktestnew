using FishNet.Managing;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Test.UI
{
    public class TestPanel : BasePanel<TestPanel>
    {
        [SerializeField] private NetworkManager networkManager;
        private TextMeshProUGUI _ipPortText;
        private TMP_InputField _ipInput;
        private TMP_InputField _portInput;
        private Button _joinRoom;
        private Button _createRoom;
        private LocalConnectionState _clientState;
        private LocalConnectionState _serverState;

        protected void Awake()
        {
            Init();
            _createRoom.onClick.AddListener(() =>
            {
                if (networkManager == null)
                    return;

                if (_serverState != LocalConnectionState.Stopped)
                    networkManager.ServerManager.StopConnection(true);
                else
                {
                    if (string.IsNullOrEmpty(_portInput.text))
                        networkManager.ServerManager.StartConnection();
                    else
                        networkManager.ServerManager.StartConnection(ushort.Parse(_portInput.text));
                }
            });
            _joinRoom.onClick.AddListener(() =>
            {
                if (networkManager == null)
                    return;

                if (_clientState != LocalConnectionState.Stopped)
                    networkManager.ClientManager.StopConnection();
                else
                {
                    if (string.IsNullOrEmpty(_ipInput.text))
                        networkManager.ClientManager.StartConnection();
                    else
                    {
                        if (string.IsNullOrEmpty(_portInput.text))
                            networkManager.ClientManager.StartConnection(_ipInput.text);
                        else
                            networkManager.ClientManager.StartConnection(_ipInput.text, ushort.Parse(_portInput.text));
                    }
                }
            });
        }

        public override void Init()
        {
            base.Init();
            _ipPortText = GetControl<TextMeshProUGUI>("ipPort");
            _ipInput = GetControl<TMP_InputField>("ipInput");
            _portInput = GetControl<TMP_InputField>("portInput");
            _joinRoom = GetControl<Button>("join");
            _createRoom = GetControl<Button>("create");
        }

        private void Start()
        {
            networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
        }

        private void Update()
        {
            _ipPortText.text = networkManager.ClientManager.Connection.ToString();
            // Transport transport = networkManager.TransportManager.Transport;
            // transport.GetConnectionAddress(networkManager.ClientManager.Connection.ClientId);
        }

        private void OnDestroy()
        {
            networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;
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