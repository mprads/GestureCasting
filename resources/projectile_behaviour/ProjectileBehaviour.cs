using Godot;
using Game.GameObjects.Projectile;

namespace Game.Resources;

[GlobalClass]
public abstract partial class ProjectileBehaviour : Resource {
    public abstract void Move(Projectile projectile, double delta);
}
