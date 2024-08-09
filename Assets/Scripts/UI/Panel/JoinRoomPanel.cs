using FishNet.Transporting;
using GamePlay.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    public class JoinRoomPanel : BasePanel<JoinRoomPanel>
    {
        private void OnEnable()
        {
            NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += OnClientConnection;
        }

        private void OnDisable()
        {
            NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState -= OnClientConnection;
        }

        public override void Init()
        {
            base.Init();
            GetControl<TMP_InputField>("IP").onValueChanged.AddListener(s =>
            {
                if (string.IsNullOrEmpty(s)) return;
                NetworkMgr.Instance.tugboat.SetClientAddress(s);
            });
            GetControl<TMP_InputField>("port").onValueChanged.AddListener(s =>
            {
                if (string.IsNullOrEmpty(s)) return;
                NetworkMgr.Instance.tugboat.SetPort(ushort.Parse(s));
            });
            GetControl<TMP_InputField>("password").onValueChanged.AddListener(s =>
            {
                RoomMgr.Instance.authenticator.Password = s;
            });
            GetControl<Button>("join").onClick.AddListener(() => { NetworkMgr.Instance.JoinRoom(); });
            GetControl<Button>("exit").onClick.AddListener(() => { HideMe(); });
        }

        private void OnClientConnection(ClientConnectionStateArgs obj)
        {
            switch (obj.ConnectionState)
            {
                case LocalConnectionState.Started:
                    GamePanel.Instance.ShowMe();
                    break;
                case LocalConnectionState.Starting:
                    Debug.Log("连接中。。。");
                    break;
            }
        }

        private void UpdateModel()
        {
            RoomMgr.Instance.authenticator.Password = GetControl<TMP_InputField>("password").text;
            NetworkMgr.Instance.tugboat.SetClientAddress(GetControl<TMP_InputField>("IP").text);
            if (ushort.TryParse(GetControl<TMP_InputField>("port").text, out var res))
                NetworkMgr.Instance.tugboat.SetPort(res);
        }


        public override void CallBack(bool flag)
        {
            base.CallBack(flag);
            if (flag) UpdateModel();
        }
    }
}