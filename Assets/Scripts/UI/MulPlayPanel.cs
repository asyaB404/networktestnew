using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MulPlayPanel : BasePanel<MulPlayPanel>
{
    public override void Init()
    {
        base.Init();
        GetControl<Button>("create")?.onClick.AddListener(() => { CreateRoomPanel.Instance.ShowMe(); });
        GetControl<Button>("join")?.onClick.AddListener(() => { JoinRoomPanel.Instance.ShowMe(); });
        GetControl<Button>("exit")?.onClick.AddListener(HideMe);
    }

    public override void CallBack(bool flag)
    {
        transform.DOKill(true);
        if (flag)
        {
            MyCanvasGroup.interactable = true;
            gameObject.SetActive(true);
            transform.localPosition = new Vector3(-1500, 0, 0);
            transform.DOLocalMoveX(0, Const.UIDuration);
        }
        else
        {
            MyCanvasGroup.interactable = false;
            transform.DOLocalMoveX(-1500, Const.UIDuration).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}