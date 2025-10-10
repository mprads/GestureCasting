using Godot;
using Game.Autoloads;
using Game.Entities;

namespace Game;

[GlobalClass]
public partial class Projectile : RayCast3D, IPoolable {
    static readonly PackedScene PROJECTILE_SCENE = ResourceLoader.Load<PackedScene>("uid://c40d62btwmq5i");

    private Node3D owner;
    private float speed;

    public override void _PhysicsProcess(double delta) {
        Position += GlobalBasis * Vector3.Forward * speed * (float)delta;
        TargetPosition = Vector3.Forward * speed * (float)delta;
        ForceRaycastUpdate();
        GodotObject collider = GetCollider();

        if (IsColliding()) {
            if (collider != owner) {
                GlobalPosition = GetCollisionPoint();
                ObjectPool.Instance.ReturnInstance(this, PROJECTILE_SCENE);
                SetPhysicsProcess(false);
            }
        }
    }

    public void Prepare() {
        speed = 20.0f;
    }

    public static void CreateNew(Node3D caster) {
        Projectile newProjectile = (Projectile)ObjectPool.Instance.RequestInstantiate(PROJECTILE_SCENE);
        newProjectile.owner = caster;
        if (caster is Player player) {
            newProjectile.GlobalTransform = player.GetSpellOriginTransform();
        } else {
            newProjectile.GlobalTransform = caster.GlobalTransform;
        }
    }
}
