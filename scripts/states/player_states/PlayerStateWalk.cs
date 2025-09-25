using Godot;

namespace Game.States;

public partial class PlayerStateWalk : PlayerState {
    public override void UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("jump")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.JUMP);
        } else if (@event.IsActionPressed("crouch")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.CROUCH);
        } else if (@event.IsActionPressed("sprint")) {
            if (player.Velocity != Vector3.Zero) {
                EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.SPRINT);
            }

        }
    }
}
