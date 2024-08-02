using GamePlay.Room;
using TMPro;
using UnityEngine;

namespace UI.InfoPanel
{
    public class PlayerInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI line1;
        [SerializeField] private TextMeshProUGUI line2;

        public void Init(PlayerInfo info)
        {
            line1.text = info.id + 1 + " : " + info.playerName;
            line2.text = info.connection.GetAddress();
        }
    }
}