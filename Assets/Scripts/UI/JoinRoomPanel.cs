using FishNet.Transporting;
using TMPro;
using UnityEngine.UI;

public class JoinRoomPanel : BasePanel<JoinRoomPanel>
{
    public override void Init()
    {
        base.Init();
        var tugboat = NetworkMgr.Instance.tugboat;
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
        GetControl<TMP_InputField>("IP").onValueChanged.AddListener((s) => { tugboat.SetClientAddress(s); });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) => { tugboat.SetPort(ushort.Parse(s)); });
        GetControl<Button>("join").onClick.AddListener(() => { NetworkMgr.Instance.JoinOrExitRoom(); });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
    }
}