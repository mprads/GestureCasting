using Godot;
using Game.GameObjects.Projectile;

namespace Game.Resources.ProjectileBehaviours;

[GlobalClass]
public partial class DefaultBehaviour : ProjectileBehaviour {
    public override void Move(Projectile projectile, double delta) {
        projectile.GlobalPosition += projectile.GlobalBasis * Vector3.Forward * projectile.InitialSpeed * (float)delta;
        projectile.TargetPosition = Vector3.Forward * projectile.InitialSpeed * (float)delta;
        projectile.ForceRaycastUpdate();
    }
}
