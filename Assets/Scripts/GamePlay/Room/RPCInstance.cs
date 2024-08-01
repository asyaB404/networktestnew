using FishNet.Object;
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
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}