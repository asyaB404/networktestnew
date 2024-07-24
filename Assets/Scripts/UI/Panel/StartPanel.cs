using UnityEngine.UI;

namespace UI.Panel
{
    public class StartPanel : BasePanel<StartPanel>
    {
        public override void Init()
        {
            base.Init();
            GetControl<Button>("mulplay")?.onClick.AddListener(() => { MulPlayPanel.Instance.ShowMe(); });
            GetControl<Button>("setting")?.onClick.AddListener(() => { SettingPanel.Instance.ShowMe(); });
            ShowMe();
        }


        public override void HideMe(bool isPressedEsc = false)
        {
            
        }
    }
}