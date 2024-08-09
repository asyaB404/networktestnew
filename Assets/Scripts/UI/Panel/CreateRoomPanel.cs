using System.Linq;
using FishNet.Transporting;
using GamePlay.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Panel
{
    /// <summary>
    ///     用于创建房间的panel
    /// </summary>
    public class CreateRoomPanel : BasePanel<CreateRoomPanel>
    {
        private void OnEnable()
        {
            NetworkMgr.Instance.networkManager.ServerManager.OnServerConnectionState += OnServerConnection;
        }

        private void OnDisable()
        {
            NetworkMgr.Instance.networkManager.ServerManager.OnServerConnectionState -= OnServerConnection;
        }

        public override void Init()
        {
            base.Init();
            var tugboat = NetworkMgr.Instance.tugboat;
            GetControl<TMP_InputField>("name").onValueChanged.AddListener(s =>
            {
                if (string.IsNullOrEmpty(s)) return;
            });
            GetControl<TMP_InputField>("port").onValueChanged.AddListener(s =>
            {
                if (string.IsNullOrEmpty(s)) return;
                tugboat.SetPort(ushort.Parse(s));
            });
            GetControl<TMP_InputField>("password").onValueChanged.AddListener(s =>
            {
                RoomMgr.Instance.authenticator.Password = s;
            });
            GetControl<Button>("create").onClick.AddListener(() =>
            {
                var i = GetControl<ToggleGroup>("selectMode").ActiveToggles().FirstOrDefault()!.transform
                    .GetSiblingIndex();
                NetworkMgr.Instance.CreateRoom(RoomType.T1V1 + i, GetControl<TMP_InputField>("name").text);
            });
            GetControl<Button>("exit").onClick.AddListener(() => { HideMe(); });
        }

        private void OnServerConnection(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
            {
                GamePanel.Instance.ShowMe();
                NetworkMgr.Instance.JoinRoom();
            }
            else if (obj.ConnectionState == LocalConnectionState.Starting)
            {
                Debug.Log("连接中。。。");
            }
            else if (obj.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.Log("开启失败，请检查端口是否被占用");
            }
        }

        private void UpdateModel()
        {
            if (ushort.TryParse(GetControl<TMP_InputField>("port").text, out var res))
                NetworkMgr.Instance.tugboat.SetPort(res);
            RoomMgr.Instance.authenticator.Password = GetControl<TMP_InputField>("password").text;
        }


        public override void CallBack(bool flag)
        {
            base.CallBack(flag);
            if (flag) UpdateModel();
        }
    }
}