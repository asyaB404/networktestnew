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

    public override void CallBack(bool flag)
    {
        transform.DOKill(true);
        if (flag)
        {
            MyCanvasGroup.interactable = true;
            gameObject.SetActive(true);
            transform.DOLocalMoveX(0, Const.UIDuration);
        }
        else
        {
            MyCanvasGroup.interactable = false;
            transform.DOLocalMoveX(-1500, Const.UIDuration).OnComplete(() => { gameObject.SetActive(false); });
        }
    }

    public override void HideMe()
    {
        //确定要退出游戏吗？
    }
}