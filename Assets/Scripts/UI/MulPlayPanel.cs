using DG.Tweening;
using UnityEngine.UI;

public class MulPlayPanel : BasePanel<MulPlayPanel>
{
    public override void Init()
    {
        base.Init();
        GetControl<Button>("create")?.onClick.AddListener(() => { });
        GetControl<Button>("join")?.onClick.AddListener(() => { });
        GetControl<Button>("exit")?.onClick.AddListener(HideMe);
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