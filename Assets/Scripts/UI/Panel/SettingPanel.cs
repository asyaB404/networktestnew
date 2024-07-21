using TMPro;

namespace UI.Panel
{
    public class SettingPanel : BasePanel<SettingPanel>
    {
        public override void Init()
        {
            base.Init();
            GetControl<TMP_InputField>("Name").onValueChanged.AddListener((s) =>
            {
                if (string.IsNullOrEmpty(s)) return;
            });
        }
    }
}