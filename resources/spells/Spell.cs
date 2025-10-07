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
    public Node Owner;

    public void Cast(Node3D caster, Vector3 position) {
        Owner = caster;
        Projectile projectile = Projectile.CreateNew();
        Owner.GetParent().AddChild(projectile);
        projectile.GlobalPosition = position;
    }
}
