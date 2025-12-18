using Game.Entities;
using Game.Autoload;
using Godot;
using System.Collections.Generic;

namespace Game.Managers;

public partial class GameManager : Node {
    public static List<PlayerInfo> PlayerList = new();

    public override void _Ready() {
        Events.Instance.Connect(Events.SignalName.PlayerDied, Callable.From<Player, Node3D>(OnPlayerDied));
        // Events.Instance.PlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied(Player player, Node3D source) {
        if (source is Player playerDamageSoruce) {
                GD.Print($"{player.Info.Name} killed by {playerDamageSoruce.Info.Name}");     
            } else {
                GD.Print($"{player.Info.Name} killed");
        }  
    }
}
