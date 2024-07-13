using UnityEngine.UI;


public class StartPanel : BasePanel<StartPanel>
{
    public override void Init()
    {
        base.Init();
        GetControl<Button>("Button2")?.onClick.AddListener(() => { });
    }

    public override void CallBack(bool flag)
    {
        if (flag)
        {
            MyCanvasGroup.interactable = true;
            gameObject.SetActive(true);
        }
        else
        {
            MyCanvasGroup.interactable = false;
            gameObject.SetActive(false);
        }
    }
}