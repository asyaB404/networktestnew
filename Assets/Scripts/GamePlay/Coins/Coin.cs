using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;


namespace GamePlay.Coins
{
    public enum CoinStatus
    {
        Idle,
        Moving,
        Transforming,
        Catching
    }

    public class Coin : NetworkBehaviour
    {
        public readonly SyncVar<CoinsPool> coinsPool = new();

        [AllowMutableSyncType]
        public SyncVar<CoinStatus> coinStatus =
            new(new SyncTypeSettings(WritePermission.ClientUnsynchronized, ReadPermission.ExcludeOwner));

        [SerializeField] [AllowMutableSyncType]
        public SyncVar<CoinsType> coinsType = new();

        public SpriteRenderer sr;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsClientOnlyStarted) return;
            transform.SetParent(coinsPool.Value.coinsParent.transform, false);
            sr.sprite = CoinsFactory.Instance.coinSprites[(int)coinsType.Value];
            // Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.localPosition.x),Mathf.FloorToInt(transform.localPosition.y));
            // coinsPool.Value.CoinsMap[pos] = pos;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
        }
    }
}