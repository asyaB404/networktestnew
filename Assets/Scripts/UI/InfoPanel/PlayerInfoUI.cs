using FishNet;
using FishNet.Managing.Server;
using GamePlay.Room;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InfoPanel
{
    public class PlayerInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI line1;
        [SerializeField] private TextMeshProUGUI line2;
        [SerializeField] private Button kickBtn;

        public void Init(PlayerInfo info)
        {
            line1.text = info.id + 1 + " : " + info.playerName;
            line2.text = info.connection.GetAddress();
            kickBtn.onClick.AddListener(() => { InstanceFinder.ServerManager.Kick(info.id, KickReason.Unset); });
        }

        private void OnDisable()
        {
            kickBtn.onClick.RemoveAllListeners();
        }
    }
}