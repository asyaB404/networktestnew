using System;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomPanel : BasePanel<JoinRoomPanel>
{
    public override void Init()
    {
        base.Init();
        GetControl<TMP_InputField>("IP").onValueChanged.AddListener((s) =>
        {
            if (string.IsNullOrEmpty(s)) return;
            NetworkMgr.Instance.tugboat.SetClientAddress(s);
        });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) =>
        {
            if (string.IsNullOrEmpty(s)) return;
            NetworkMgr.Instance.tugboat.SetPort(ushort.Parse(s));
        });
        GetControl<Button>("join").onClick.AddListener(() => { NetworkMgr.Instance.JoinOrExitRoom(true); });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
    }

    private void OnEnable()
    {
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += OnClientConnection;
    }

    private void OnDisable()
    {
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState -= OnClientConnection;
    }

    private void OnClientConnection(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Started)
        {
            RoomPanel.Instance.ShowMe();
        }
        else if (obj.ConnectionState == LocalConnectionState.Starting)
        {
            Debug.Log("连接中。。。");
        }
    }

    private void UpdateAddress()
    {
        NetworkMgr.Instance.tugboat.SetClientAddress(GetControl<TMP_InputField>("IP").text);
        if (ushort.TryParse(GetControl<TMP_InputField>("port").text, out var res))
        {
            NetworkMgr.Instance.tugboat.SetPort(res);
        }
    }


    public override void CallBack(bool flag)
    {
        base.CallBack(flag);
        if (flag)
        {
            UpdateAddress();
        }
    }
}