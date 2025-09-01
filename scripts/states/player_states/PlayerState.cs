using Godot;

namespace Game.States;

[GlobalClass]
public partial class PlayerState : Node {

    [Signal]
    public delegate void TransitionRequestedEventHandler(PlayerState from, PlayerStateMachine.STATE to);

    public Entities.Player player;

    public virtual void Enter() {}

    public virtual void Exit() {}

    public virtual void PhysicsProcess(double _delta) {}

    public virtual void UnhandledInput(InputEvent @event) {}
}
