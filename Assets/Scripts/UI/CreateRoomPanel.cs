using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using TMPro;
using UnityEngine;

public class CreateRoomPanel : BasePanel<CreateRoomPanel>
{
    [SerializeField] private Tugboat tugboat;

    public override void Init()
    {
        base.Init();
        tugboat = FindObjectOfType<Tugboat>();
        GetControl<TMP_InputField>("IP").onValueChanged.AddListener((s) =>
        {
            tugboat.SetServerBindAddress(s, IPAddressType.IPv4);
        });
        GetControl<TMP_InputField>("port").onValueChanged.AddListener((s) => { tugboat.SetPort(ushort.Parse(s)); });
    }
}