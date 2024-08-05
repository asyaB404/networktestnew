namespace UI.GamingUI
{
    public class ReadOrStartPanel:GameUI
    {
        protected override void Awake()
        {
            base.Awake();
            buttons[0].onClick.AddListener(() => { });
        }
    }
}