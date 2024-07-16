using DG.Tweening;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel<GamePanel>
{
    private int _playerCount;

    public override void Init()
    {
        base.Init();
        
        GetControl<Button>("exit").onClick.AddListener(() =>
        {
            HideMe();
            NetworkMgr.Instance.CreateOrCloseRoom(false);
            NetworkMgr.Instance.JoinOrExitRoom(false);
        });
    }

    private void OnEnable()
    {
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += OnUpdateJoin;
        // NetworkMgr.Instance.networkManager.ClientManager.OnConnectedClients
    }

    private void OnDisable()
    {
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState -= OnUpdateJoin;
    }

    private void OnUpdateJoin(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("生成一个玩家");
        }

        if (obj.ConnectionState == LocalConnectionState.Stopped)
        {
            HideMe();
        }
    }
    
    private void SpawnPlayer()
    {
    }

    public override void CallBack(bool flag)
    {
        transform.DOKill(true);
        if (flag)
        {
            MyCanvasGroup.interactable = true;
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            transform.DOScale(1, Const.UIDuration);
        }
        else
        {
            MyCanvasGroup.interactable = false;
            transform.DOScale(0, Const.UIDuration).OnComplete(() => { gameObject.SetActive(false); });
        }
    }
}