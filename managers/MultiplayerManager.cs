using Godot;

public partial class MultiplayerManager : Control {
    [Export]
    public PackedScene GameLevel;

    [Export]
    private int maxClients = 10;
    private const int PORT = 8910;
    private const string ADDRESS = "127.0.0.1";
    private ENetMultiplayerPeer peer;

    private Button hostButton;
    private Button joinButton;
    private Button startButton;

    public override void _Ready() {
        hostButton = GetNode<Button>("%HostButton");
        joinButton = GetNode<Button>("%JoinButton");
        startButton = GetNode<Button>("%StartButton");


        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        hostButton.Pressed += OnHostButtonPressed;
        joinButton.Pressed += OnJoinButtonPressed;
        startButton.Pressed += OnStartButtonPressed;
    }

    private void OnPeerConnected(long id) {
        GD.Print($"Player Connected: {id}");
    }

    private void OnPeerDisconnected(long id) {
        GD.Print($"Player Disonnected: {id}");
    }

    private void OnConnectedToServer() {
        GD.Print($"Connected to Server");
    }
    
    private void OnConnectionFailed() {
        GD.Print($"Connection Failed");
    }

    private void OnHostButtonPressed() {
        peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(PORT, maxClients);

        if (error != Error.Ok) {
            GD.Print($"Error Cannot Host {error}");
            return;
        }

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;

        GD.Print($"Waiting For Players");
    }

    private void OnJoinButtonPressed() {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(ADDRESS, PORT);

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;

        GD.Print($"Joined Server");
    }

    private void OnStartButtonPressed() {
        Node3D level = GameLevel.Instantiate<Node3D>();
        GetTree().Root.AddChild(level);
        this.Hide();
    }
}
