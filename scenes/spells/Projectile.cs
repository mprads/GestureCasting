using Godot;

namespace Game;

[GlobalClass]
public partial class Projectile : Area3D {
    static readonly PackedScene PROJECTILE_SCENE = ResourceLoader.Load<PackedScene>("uid://c40d62btwmq5i");

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

    public static Projectile CreateNew() {
        Projectile newProjectile = (Projectile)PROJECTILE_SCENE.Instantiate();
        return newProjectile;
    }
}
