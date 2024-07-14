using DG.Tweening;
using UnityEngine.UI;


public class StartPanel : BasePanel<StartPanel>
{
    public override void Init()
    {
        base.Init();
        GetControl<Button>("mulplay")?.onClick.AddListener(() => { MulPlayPanel.Instance.ShowMe(); });
        ShowMe();
    }
    

    public override void HideMe()
    {
        //确定要退出游戏吗？
    }
}