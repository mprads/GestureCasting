using Godot;

namespace Game.Managers;

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
    private LineEdit nameInput;
    private VBoxContainer playerContainer;

    public override void _Ready() {
        hostButton = GetNode<Button>("%HostButton");
        joinButton = GetNode<Button>("%JoinButton");
        startButton = GetNode<Button>("%StartButton");
        nameInput = GetNode<LineEdit>("%NameInput");
        playerContainer = GetNode<VBoxContainer>("%PlayerContainer");

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        hostButton.Pressed += OnHostButtonPressed;
        joinButton.Pressed += OnJoinButtonPressed;
        startButton.Pressed += OnStartButtonPressed;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void StartGame() {
        Node3D level = GameLevel.Instantiate<Node3D>();
        GetTree().Root.AddChild(level);
        this.Hide();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void SendPlayerInformation(string name, int id) {
        PlayerInfo playerInfo = new PlayerInfo() {
            Name = name,
            Id = id
        };

        if (!GameManager.PlayerList.Contains(playerInfo)) {
            GameManager.PlayerList.Add(playerInfo);
            foreach (var child in playerContainer.GetChildren()) {
                child.QueueFree();
            }

            foreach (PlayerInfo info in GameManager.PlayerList) {
                Label playerLabel = new Label();
                playerLabel.Text = $"{info.Name}: {info.Id}";
                playerContainer.AddChild(playerLabel);
            }
        }

        if (Multiplayer.IsServer()) {
            foreach (var item in GameManager.PlayerList) {
                Rpc(nameof(SendPlayerInformation), item.Name, item.Id);
            }
        }
    }

    private void OnPeerConnected(long id) {
        GD.Print($"Player Connected: {id}");
    }

    private void OnPeerDisconnected(long id) {
        GD.Print($"Player Disonnected: {id}");
    }

    private void OnConnectedToServer() {
        GD.Print($"Connected to Server");
        RpcId(1, nameof(SendPlayerInformation), GetNode<LineEdit>("%NameInput").Text, Multiplayer.GetUniqueId());
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

        SendPlayerInformation(GetNode<LineEdit>("%NameInput").Text, 1);

        startButton.Disabled = false;
        hostButton.Disabled = true;
    }

    private void OnJoinButtonPressed() {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(ADDRESS, PORT);

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;

        GD.Print($"Joined Server");
        hostButton.Disabled = true;
        joinButton.Disabled = true;
    }

    private void OnStartButtonPressed() {
        Rpc(nameof(StartGame));
    }
}
