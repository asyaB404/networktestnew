using UnityEngine.UI;

namespace UI.Panel
{
    public class MulPlayPanel : BasePanel<MulPlayPanel>
    {
        public override void Init()
        {
            base.Init();
            GetControl<Button>("create")?.onClick.AddListener(() => { CreateRoomPanel.Instance.ShowMe(); });
            GetControl<Button>("join")?.onClick.AddListener(() => { JoinRoomPanel.Instance.ShowMe(); });
            GetControl<Button>("exit")?.onClick.AddListener(() => { HideMe(); });
        }
    }
}