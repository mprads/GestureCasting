using Godot;
using Game.GameObjects.Projectile;

namespace Game.Resources.ProjectileBehaviours;

[GlobalClass]
public partial class AccelerateBehaviour : ProjectileBehaviour {
    private float acceleration = 250.0f;
    public override void Move(Projectile projectile, double delta) {
        projectile.CurrentSpeed = (float)Mathf.MoveToward((double)projectile.CurrentSpeed, (double)projectile.MaxSpeed, acceleration * delta);
        projectile.GlobalPosition += projectile.GlobalBasis * Vector3.Forward * projectile.CurrentSpeed * (float)delta;
        projectile.TargetPosition = Vector3.Forward * projectile.CurrentSpeed * (float)delta;
        projectile.ForceRaycastUpdate();
    }
}