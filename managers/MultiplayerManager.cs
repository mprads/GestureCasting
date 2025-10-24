using Godot;
using System;

public partial class MultiplayerManager : Control {

    [Export]
    private int maxClients = 10;
    private const int PORT = 8910;
    private const string ADDRESS = "127.0.0.1";

    private Button hostButton;
    private Button joinButton;

    public override void _Ready() {
        hostButton = GetNode<Button>("%HostButton");
        joinButton = GetNode<Button>("%JoinButton");

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        hostButton.Pressed += OnHostButtonPressed;
        joinButton.Pressed += OnJoinButtonPressed;
    }

    private void OnPeerConnected(long id) {
    }

    private void OnPeerDisconnected(long id) {
    }

    private void OnConnectedToServer() {
    }
    
    private void OnConnectionFailed() {
    }

    private void OnHostButtonPressed() {
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        Error error = peer.CreateServer(PORT, maxClients);

        if (error != Error.Ok) {
            GD.Print($"Error Cannot Host {error}");
            return;
        }

        peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = peer;
    }

    private void OnJoinButtonPressed() {
    }
}
