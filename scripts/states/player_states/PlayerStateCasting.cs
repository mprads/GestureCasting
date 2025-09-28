using Godot;
using System;

namespace Game.States;

public partial class PlayerStateCasting : PlayerState {

    public override void Enter() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void Exit() {
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("toggle_mouse_mode")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.IDLE);
        }
    }
}
