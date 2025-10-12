using Godot;
using Game.Autoloads;
using Game.Entities;
using System;

namespace Game;

[GlobalClass]
public partial class Projectile : RayCast3D, IPoolable {
    private Timer lifeSpanTimer;
    private Label3D timerLabel;
    private Label3D poolLabel;
    private Node3D owner;
    private PackedScene scene;
    private RemoteTransform3D remoteTransform;
    private float speed;
    private float damage;

    public override void _Process(double delta) {
        timerLabel.Text = $"{Math.Round(lifeSpanTimer.TimeLeft, 2)}";
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
                }  
            }
        }
    }

    public void SetUp() {
        lifeSpanTimer = GetNode<Timer>("%LifeSpanTimer");
        timerLabel = GetNode<Label3D>("%TimerLabel");
        poolLabel = GetNode<Label3D>("%PoolLabel");

        lifeSpanTimer.Timeout += CleanUp;
        remoteTransform = new();

        speed = 20.0f;
    }

    public void Prepare() {
        SetPhysicsProcess(true);
        remoteTransform = new();
        lifeSpanTimer.Start();
    }

    // DEBUG delete later
    public void SetPoolLabel(string fromPool) {
        poolLabel.Text = $"{fromPool}";
    }

    private void CleanUp() {
        RemoteTransform3D previousTransform = remoteTransform;
        remoteTransform = new();
        previousTransform.QueueFree();

        lifeSpanTimer.Stop();
        ObjectPool.Instance.ReturnInstance(this, scene);
    }

    public static void CreateNew(Node3D caster, PackedScene projectileScene) {
        Projectile newProjectile = (Projectile)ObjectPool.Instance.RequestInstantiate(projectileScene);
        newProjectile.owner = caster;
        newProjectile.scene = projectileScene;
        if (caster is Player player) {
            newProjectile.GlobalTransform = player.GetSpellOriginTransform();
        } else {
            newProjectile.GlobalTransform = caster.GlobalTransform;
        }
    }
}
