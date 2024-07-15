using System.Collections.Generic;
using DG.Tweening;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel<RoomPanel>
{
    [SerializeField] private List<RoomPanel> rooms = new(4);
    [SerializeField] private GameObject playerPrefab;
    private int _playerCount;

    public override void Init()
    {
        base.Init();
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState += OnUpdateJoin;
        GetControl<Button>("exit").onClick.AddListener(() =>
        {
            HideMe();
            NetworkMgr.Instance.CreateOrCloseRoom(false);
        });
    }

    private void OnDestroy()
    {
        NetworkMgr.Instance.networkManager.ClientManager.OnClientConnectionState -= OnUpdateJoin;
    }

    private void OnUpdateJoin(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Started)
        {
        }
        else
        {
        }
    }

    private void SpawnPlayer()
    {
        GameObject playerObj = Instantiate(playerPrefab);
        NetworkMgr.Instance.networkManager.ServerManager.Spawn(playerObj);
        _playerCount++;
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