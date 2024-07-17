using System;
using FishNet.Transporting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPanel : BasePanel<CreateRoomPanel>
{
    public override void Init()
    {
        base.Init();
        var tugboat = NetworkMgr.Instance.tugboat;
        GetControl<TMP_InputField>("name").onValueChanged.AddListener((s) =>
        {
            if (string.IsNullOrEmpty(s)) return;
            
        });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) =>
        {
            if (string.IsNullOrEmpty(s)) return;
            tugboat.SetPort(ushort.Parse(s));
        });
        GetControl<Button>("create").onClick.AddListener(() => { NetworkMgr.Instance.CreateOrCloseRoom(true); });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
    }


    private void OnEnable()
    {
        NetworkMgr.Instance.networkManager.ServerManager.OnServerConnectionState += OnServerConnection;
    }

    private void OnDisable()
    {
        NetworkMgr.Instance.networkManager.ServerManager.OnServerConnectionState -= OnServerConnection;
    }

    private void OnServerConnection(ServerConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Started)
        {
            GamePanel.Instance.ShowMe();
            NetworkMgr.Instance.JoinOrExitRoom(true);
            Debug.Log("开启成功");
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

    private void UpdateAddress()
    {
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