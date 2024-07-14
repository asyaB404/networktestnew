using TMPro;
using UnityEngine;

public class JoinRoomPanel : BasePanel<JoinRoomPanel>
{
    public override void Init()
    {
        base.Init();
        GetControl<TMP_InputField>("IP").onEndEdit.AddListener((s) => { Debug.Log(s); });
        GetControl<TMP_InputField>("port").onEndEdit.AddListener((s) => { Debug.Log(s); });
    }
}