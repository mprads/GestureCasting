using System.Collections.Generic;
using System.Threading.Tasks;
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
    public int ProjectileCount = 1;
    [Export]
    public float CastDelay = 0.5f;
    public Node Owner;

    public async Task Cast(Node3D caster) {
        Owner = caster;
        
        for (int i = 0; i < ProjectileCount; i++) {
            await ToSignal(caster.GetTree().CreateTimer(CastDelay), "timeout");
            Projectile.CreateNew(caster);
        }
    }
}
