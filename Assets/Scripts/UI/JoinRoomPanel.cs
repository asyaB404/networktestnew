using FishNet.Transporting;
using TMPro;
using UnityEngine.UI;

public class JoinRoomPanel : BasePanel<JoinRoomPanel>
{
    public override void Init()
    {
        base.Init();
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += (ClientConnectionStateArgs obj) =>
        {
            if (obj.ConnectionState == LocalConnectionState.Started)
            {
                RoomPanel.Instance.ShowMe();
            }
            else
            {
            }
        };
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
        GetControl<Button>("join").onClick.AddListener(() => { NetworkMgr.Instance.JoinOrExitRoom(); });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
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