using Game.Entities;
using Godot;

namespace Game.Autoload;

public partial class Events : Node {
    public static Events Instance { get; private set; }

    [Signal]
    public delegate void PlayerDiedEventHandler(Player player, Node3D damageSource);

    public override void _Notification(int what) {
    	if (what == NotificationSceneInstantiated) {
			Instance = this;
	   }
    }

    public static void EmitPlayerDied(Player player, Node3D damageSource) {
        Instance.EmitSignal(SignalName.PlayerDied, player, damageSource);
    }
}
