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
        if (flag)
        {
            MyCanvasGroup.interactable = true;
            gameObject.SetActive(true);
            transform.DOLocalMoveX(0, 0.5f);
        }
        else
        {
            MyCanvasGroup.interactable = false;
            transform.DOLocalMoveX(-1500, 0.5f).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}