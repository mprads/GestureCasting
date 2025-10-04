using Godot;

namespace Game.Resources.Spells;

[GlobalClass]
public partial class Spell : Resource {
    [Export]
    public float MaxDuration = 4.0f;
    [Export]
    public float Speed = 4.0f;
    [Export]
    public float Cooldown = 2.0f;
    [Export]
    public PackedScene ProjectileReference;

    public void Cast(Node3D caster, Vector3 position) {
        Projectile projectile = (Projectile)ProjectileReference.Instantiate();
        caster.AddChild(projectile);
        projectile.GlobalPosition = position;
    }
}
