using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine.UI;

public class JoinRoomPanel : BasePanel<JoinRoomPanel>
{
    private readonly Tugboat _tugboat = NetworkMgr.Instance.tugboat;

    public override void Init()
    {
        base.Init();
        GetControl<TMP_InputField>("IP").onValueChanged.AddListener((s) => { _tugboat.SetClientAddress(s); });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) => { _tugboat.SetPort(ushort.Parse(s)); });
        GetControl<Button>("join").onClick.AddListener(() => { NetworkMgr.Instance.JoinOrExitRoom(); });
        GetControl<Button>("exit").onClick.AddListener(HideMe);
    }
}