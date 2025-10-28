using Godot;
using System.Collections.Generic;
using Game.Resources.Spells;
using Game.Entities;
using System.Threading.Tasks;

namespace Game.Managers;

public partial class CastManager : Node {

    [Export]
    private Godot.Collections.Array<Spell> defaultSpells = [];  // Cannot export a typed list

    private HashSet<Spell> availableSpells = new();
    private Player owner;

    public void Init(Player player) {
        owner = player;

        foreach (Spell spellResource in defaultSpells) {
            availableSpells.Add(spellResource);
        }
    }

    public async Task Cast(Spell spell) {
        if (availableSpells.Contains(spell)) {
            await spell.Cast(owner);
        }
    }
}
