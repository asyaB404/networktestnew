using FishNet;
using FishNet.Object;
using UI.InfoPanel;
using UnityEngine;

namespace GamePlay.Room
{
    public class RPCInstance : NetworkBehaviour
    {
        [ContextMenu("test")]
        private void Test()
        {
            Debug.Log(Instance != null);
        }

        public static RPCInstance Instance { get; private set; }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (base.IsOwner)
            {
                if (Instance != null)
                {
                    Debug.Log("多余的RPC已经被移除");
                    Destroy(gameObject);
                }
                else
                {
                    Instance = this;
                }

                SendPlayerInfo(new PlayerInfo(-1, PlayerPrefsMgr.PlayerName, InstanceFinder.ClientManager.Connection));
                UpdatePlayerInfos();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        [ServerRpc]
        public void SendPlayerInfo(PlayerInfo info)
        {
            int i = 0;
            foreach (var con in RoomMgr.Instance.playersCon)
            {
                if (con.CustomData is string s && s == "init")
                {
                    info.id = i;
                    con.CustomData = info;
                    break;
                }
                i++;
            }
        }

        [ServerRpc]
        public void UpdatePlayerInfos()
        {
            PlayerInfoPanel.Instance.UpdateInfoPanel(RoomMgr.Instance.PlayerInfos);
        }
    }
}