using Game.Entities;
using Game.Managers;
using Godot;

public partial class SceneManager : Node {
 
    public override void _Ready() {
        int index = 0;
        foreach (PlayerInfo info in GameManager.PlayerList) {
            Player newPlayer = Player.CreateNew(info);
            AddChild(newPlayer);

            // TODO still not a fan of groups, but need the scene manage to have access
            // to the level scene to get access to the spawnpoint nodes
            foreach (Node3D spawnPoint in GetTree().GetNodesInGroup("player_spawn_point")) {
                if (int.Parse(spawnPoint.Name) == index) {
                    newPlayer.GlobalPosition = spawnPoint.GlobalPosition;
                }
            }

            index++;
        }
    }
}
