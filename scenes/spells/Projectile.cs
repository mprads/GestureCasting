using Godot;
using Game.Autoloads;
using Game.Entities;

namespace Game;

[GlobalClass]
public partial class Projectile : Area3D, IPoolable {
    static readonly PackedScene PROJECTILE_SCENE = ResourceLoader.Load<PackedScene>("uid://c40d62btwmq5i");

    public override void _Ready() {
        BodyEntered += OnBodyEntered;
        AreaEntered += OnAreaEntered;
        GD.Print("Ready");
    }

    public void Prepare() {
        GD.Print("Preparing");
    }

    private void OnBodyEntered(Node3D body) {
        GD.Print("Area entered");
        ObjectPool.Instance.ReturnInstance(this, PROJECTILE_SCENE);
    }

    private void OnAreaEntered(Area3D area) {
        GD.Print("Area entered");
        ObjectPool.Instance.ReturnInstance(this, PROJECTILE_SCENE);
    }

    public static void CreateNew(Node3D caster, Vector3 position) {
        Projectile newProjectile = (Projectile)ObjectPool.Instance.RequestInstantiate(PROJECTILE_SCENE);
        newProjectile.GlobalPosition = position;
    }
}
