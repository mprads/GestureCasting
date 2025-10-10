using Godot;
using Game.Autoloads;
using Game.Entities;

namespace Game;

[GlobalClass]
public partial class Projectile : RayCast3D, IPoolable {
    static readonly PackedScene PROJECTILE_SCENE = ResourceLoader.Load<PackedScene>("uid://c40d62btwmq5i");

    private Timer lifeSpanTimer;
    private Node3D owner;
    private RemoteTransform3D remoteTransform;
    private float speed;

    public override void _Ready() {
        lifeSpanTimer = GetNode<Timer>("%LifeSpanTimer");
        lifeSpanTimer.Timeout += CleanUp;
        remoteTransform = new RemoteTransform3D();

        lifeSpanTimer.Start();
    }

    public override void _PhysicsProcess(double delta) {
        Position += GlobalBasis * Vector3.Forward * speed * (float)delta;
        TargetPosition = Vector3.Forward * speed * (float)delta;
        ForceRaycastUpdate();
        GodotObject collider = GetCollider();

        if (IsColliding()) {
            if (collider != owner) {
                GlobalPosition = GetCollisionPoint();
                SetPhysicsProcess(false);
                if (collider is Node collisionNode) {
                    collisionNode.AddChild(remoteTransform);
                    remoteTransform.GlobalTransform = GlobalTransform;
                    remoteTransform.RemotePath = remoteTransform.GetPathTo(this);
                    remoteTransform.TreeExited += CleanUp;
                }  
            }
        }
    }

    public void Prepare() {
        speed = 20.0f;
    }

    private void CleanUp() {
        ObjectPool.Instance.ReturnInstance(this, PROJECTILE_SCENE);
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
