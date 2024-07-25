using UI.Panel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GamingUI
{
    public class MenuPanel : GameUI
    {
        private Button[] _buttons;

        private void Awake()
        {
            _buttons = GetComponentsInChildren<Button>(true);
            _buttons[0].onClick.AddListener(HideMe);
            _buttons[1].onClick.AddListener(() =>
            {
                HideMe();
                GamePanel.Instance.HideMe(false);
            });
        }

        public override void ShowMe()
        {
            gameObject.SetActive(true);
        }

        public override void HideMe()
        {
            gameObject.SetActive(false);
        }
    }
}