
using Godot;
using Game.GameObjects.Projectile;
using Game.Components;

namespace Game.Resources.ProjectileBehaviours;

[GlobalClass]
public partial class TrackingBehaviour : ProjectileBehaviour {
    public override void Move(Projectile projectile, double delta) {
        if (Owner.GetNode<LineOfSightComponent>("%LineOfSightComponent") != null) {
            Node3D target = Owner.GetNode<LineOfSightComponent>("%LineOfSightComponent").GetTarget();
            if (target != null) {
               TrackingMove(projectile, target, delta);
            } else {
                 DefaultMove(projectile, delta);
            }
        } else {
            DefaultMove(projectile, delta);
        }   
    }

    private void DefaultMove(Projectile projectile, double delta) {
        projectile.Position += projectile.GlobalBasis * Vector3.Forward * projectile.InitialSpeed * (float)delta;
        projectile.TargetPosition = Vector3.Forward * projectile.InitialSpeed * (float)delta;
        projectile.ForceRaycastUpdate();
    }

    private void TrackingMove(Projectile projectile, Node3D target, double delta) {
        Vector3 targetDirection = (target.Position - projectile.Position).Normalized();
        projectile.LookAt(targetDirection, Vector3.Up);
        projectile.Position += targetDirection * projectile.InitialSpeed * (float)delta;
        projectile.TargetPosition = targetDirection * projectile.InitialSpeed * (float)delta;
        projectile.ForceRaycastUpdate();
    }
}
