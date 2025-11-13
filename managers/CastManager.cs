using Godot;
using System.Collections.Generic;
using Game.Resources.Spells;
using Game.Entities;
using System;

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

    public void CheckSpell(Spell spell) {
        if (availableSpells.Contains(spell)) {
            Rpc(nameof(Cast), spell.ResourcePath);
        }
    }

     [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void Cast(String spellResourceId) {
        Spell spell = ResourceLoader.Load<Spell>(spellResourceId);
        spell.Cast(owner);
    }
}
