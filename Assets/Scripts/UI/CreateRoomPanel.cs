using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
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
            tugboat.SetServerBindAddress(s, IPAddressType.IPv4);
        });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) => { tugboat.SetPort(ushort.Parse(s)); });
        GetControl<Button>("create").onClick.AddListener(() =>
        {
            RoomPanel.Instance.ShowMe();
            NetworkMgr.Instance.CreateOrCloseRoom(true);
            NetworkMgr.Instance.JoinOrExitRoom(true);
        });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
    }
}