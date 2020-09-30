using Mirror;

public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar]
    private string _displayName = "Loading...";

    private MazeNetworkManager _room;
    public MazeNetworkManager Room
    {
        get
        {
            if(_room != null) { return _room; }
            return _room = NetworkManager.singleton as MazeNetworkManager;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    public override void OnNetworkDestroy()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this._displayName = displayName;
    }
}
