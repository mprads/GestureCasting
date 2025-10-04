using Godot;

namespace Game;

[GlobalClass]
public partial class Projectile : Area3D {
    public override void _Ready() {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
    }

    private void OnBodyEntered(Node3D body) {
        GD.Print("Area entered");
        QueueFree();
    }

    private void OnAreaEntered(Area3D area) {
        GD.Print("Area entered");
        QueueFree();
    }
}
