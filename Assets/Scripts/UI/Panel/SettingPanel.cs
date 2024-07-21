using TMPro;

namespace UI.Panel
{
    public class SettingPanel : BasePanel<SettingPanel>
    {
        public override void Init()
        {
            base.Init();
            GetControl<TMP_InputField>("name").text = PlayerPrefsMgr.PlayerName;
            GetControl<TMP_InputField>("name").onValueChanged.AddListener((s) =>
            {
                if (string.IsNullOrEmpty(s))
                {
                    GetControl<TMP_InputField>("name").text = PlayerPrefsMgr.PlayerName;
                    return;
                }

                PlayerPrefsMgr.PlayerName = s;
            });
        }
    }
}