using Godot;
using System;
using System.Collections.Generic;
using Game.Resources.Spells;
using System.Linq;
using Game.Entities;

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

    public void Cast(Spell spell) {
        if (availableSpells.Contains(spell)) {
            spell.Cast(owner);
        }
    }
}
