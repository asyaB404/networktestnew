using UnityEngine;

namespace UI.GamingUI
{
    /// <summary>
    /// BasePanel的轻量版
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        public virtual void ShowMe()
        {
            gameObject.SetActive(true);
        }

        public virtual void HideMe()
        {
            gameObject.SetActive(false);
        }

        public virtual void ChangeMe()
        {
            if (gameObject.activeSelf)
                HideMe();
            else
                ShowMe();
        }
    }
}