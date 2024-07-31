using FishNet.Object;
using UnityEngine;

namespace GamePlay.Room
{
    public class RPCInstance : NetworkBehaviour
    {
        public static RPCInstance Instance { get; private set; }

        private void Awake()
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
    }
}