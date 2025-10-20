using Godot;
using Game.GameObjects.Projectile;

namespace Game.Resources;

[GlobalClass]
public abstract partial class ProjectileBehaviour : RefCounted {
    public Node3D Owner;

    //TODO Having a resource just to hold unique movement logic seems like a waste
    // Ideally the projectile would have a movement component but
    // struggling to find a way to inject the scripts at run time
    public abstract void Move(Projectile projectile, double delta);
}
