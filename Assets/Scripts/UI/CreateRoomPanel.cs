using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using TMPro;

public class CreateRoomPanel : BasePanel<CreateRoomPanel>
{
    private readonly Tugboat _tugboat = NetworkMgr.Instance.tugboat;

    public override void Init()
    {
        base.Init();
        GetControl<TMP_InputField>("IP").onValueChanged.AddListener((s) =>
        {
            _tugboat.SetServerBindAddress(s, IPAddressType.IPv4);
        });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) => { _tugboat.SetPort(ushort.Parse(s)); });
    }
}