using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;


namespace GamePlay.Coins
{
    public class Coin : NetworkBehaviour
    {
        public readonly SyncVar<CoinsPool> coinsPool = new();
        public SpriteRenderer sr;
        public CoinsType coinsType;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsClientOnlyStarted)
            {
                transform.SetParent(coinsPool.Value.coinsParent.transform, false);
                sr.sprite = CoinFactory.Instance.coinSprites[(int)coinsType];
            }
        }
    }
}