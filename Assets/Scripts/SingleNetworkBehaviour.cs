using FishNet.Object;

public class SingleNetworkBehaviour : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            enabled = false;
        }
    }
}