using FishNet.Transporting;
using TMPro;
using UnityEngine.UI;

public class CreateRoomPanel : BasePanel<CreateRoomPanel>
{
    public override void Init()
    {
        base.Init();
        var tugboat = NetworkMgr.Instance.tugboat;
        GetControl<TMP_InputField>("IP").onValueChanged.AddListener((s) =>
        {
            if (string.IsNullOrEmpty(s)) return;
            tugboat.SetServerBindAddress(s, IPAddressType.IPv4);
        });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) =>
        {
            if (string.IsNullOrEmpty(s)) return;
            tugboat.SetPort(ushort.Parse(s));
        });
        GetControl<Button>("create").onClick.AddListener(() =>
        {
            RoomPanel.Instance.ShowMe();
            NetworkMgr.Instance.CreateOrCloseRoom(true);
            NetworkMgr.Instance.JoinOrExitRoom(true);
        });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
    }

    private void UpdateAddress()
    {
        NetworkMgr.Instance.tugboat.SetServerBindAddress(GetControl<TMP_InputField>("IP").text, IPAddressType.IPv4);
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