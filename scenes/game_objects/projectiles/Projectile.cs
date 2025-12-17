using Godot;
using System;
using Game.Autoloads;
using Game.Entities;
using Game.Resources;
using Game.Components;

namespace Game.GameObjects.Projectile;

[GlobalClass]
public partial class Projectile : RayCast3D, IPoolable {
    public float InitialSpeed;
    public float MaxSpeed;
    public float CurrentSpeed;
    public Node3D Target;
    private ProjectileBehaviour behaviour;
    private Timer lifeSpanTimer;
    private Timer trackingCooldownTimer;
    private Label3D timerLabel;
    private Label3D poolLabel;
    private Node3D owner;
    private PackedScene scene;
    private RemoteTransform3D remoteTransform;

    private int damage;
    

    public override void _Process(double delta) {
        timerLabel.Text = $"{Math.Round(lifeSpanTimer.TimeLeft, 2)}";
    }

    public override void _PhysicsProcess(double delta) {
        if (behaviour != null) {
            behaviour.Move(this, delta);
        }

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

                if (collider is Player player) {
                    player.TakeDamage(damage, owner);
                }
            }
        }
    }

    public void SetUp() {
        lifeSpanTimer = GetNode<Timer>("%LifeSpanTimer");
        trackingCooldownTimer = GetNode<Timer>("%TrackingCooldownTimer");
        timerLabel = GetNode<Label3D>("%TimerLabel");
        poolLabel = GetNode<Label3D>("%PoolLabel");

        lifeSpanTimer.Timeout += CleanUp;
        remoteTransform = new();

        InitialSpeed = 50.0f;
        MaxSpeed = 500.0f;
        CurrentSpeed = InitialSpeed;
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
        owner = null;
        behaviour = null;

        lifeSpanTimer.Stop();
        ObjectPool.Instance.ReturnInstance(this, scene);
    }

    public static void CreateNew(Node3D caster, PackedScene projectileScene, ProjectileBehaviour projectileBehaviour, int spellDamage) {
        (Node newProjectile, bool fromPool) = ObjectPool.Instance.RequestInstantiate(projectileScene);
        Projectile castedProjectile = (Projectile)newProjectile;

        if (!fromPool) {
            castedProjectile.scene = projectileScene;
        }

        castedProjectile.behaviour = projectileBehaviour;
        castedProjectile.owner = caster;
        castedProjectile.damage = spellDamage;

        if (caster is Player player) {
            castedProjectile.GlobalTransform = player.GetSpellOriginTransform();
        } else {
            castedProjectile.GlobalTransform = caster.GlobalTransform;
        }

        if (caster.GetNode<LineOfSightComponent>("%LineOfSightComponent") != null) {
            castedProjectile.Target = caster.GetNode<LineOfSightComponent>("%LineOfSightComponent").GetTarget();
        } else {
            castedProjectile.Target = null;
        }
    }
}
