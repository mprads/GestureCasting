using System.Threading.Tasks;
using Godot;
using Game.GameObjects.Projectile;

namespace Game.Resources.Spells;

[GlobalClass]
public partial class Spell : Resource {
    [Export]
    private PackedScene projectileScene = ResourceLoader.Load<PackedScene>("uid://c40d62btwmq5i");
    [Export]
    private ProjectileBehaviour projectileBehaviour;

    [Export]
    public float LifeTime = 4.0f;
    [Export]
    public float CastDelay = 0.5f;
    [Export]
    public int ProjectileCount = 1;
    [Export]
    public float InitialSpeed = 50.0f;
    [Export]
    public float MaxSpeed = 100.0f;

    public Node Owner;

    public async Task Cast(Node3D caster) {
        Owner = caster;
        for (int i = 0; i < ProjectileCount; i++) {
            await ToSignal(caster.GetTree().CreateTimer(CastDelay), "timeout");
            Projectile.CreateNew(caster, projectileScene, projectileBehaviour);
        }
    }
}
