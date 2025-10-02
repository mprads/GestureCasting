using Godot;
using System;

namespace Game.States;

public partial class PlayerStateCasting : PlayerState {

    public override void Enter() {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        player.GestureInput.GestureRecognized += OnGestureRecognized;
        player.CameraController.DisableCamera();
        player.GestureInput.EnableInput();
    }

    public override void Exit() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        player.GestureInput.GestureRecognized -= OnGestureRecognized;
        player.CameraController.EnableCamera();
        player.GestureInput.DisableInput();
    }

    public override void UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("toggle_mouse_mode")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.IDLE);
        }
    }

    public override void Move(double delta) {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 moveDirection = (player.CameraController.GlobalBasis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y)).Normalized();
        Vector3 newVelocity = Vector3.Zero;
        newVelocity.Y = player.Velocity.Y;

        if (moveDirection != Vector3.Zero) {
            newVelocity.X = Mathf.Lerp(player.Velocity.X, moveDirection.X * player.WalkSpeed, player.WalkAcceleration * (float)delta);
            newVelocity.Z = Mathf.Lerp(player.Velocity.Z, moveDirection.Z * player.WalkSpeed, player.WalkAcceleration * (float)delta);
        }

        player.Velocity = newVelocity;
        player.MoveAndSlide();
    }

    private void OnGestureRecognized(string gestureName) {
        EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.IDLE);
    }
}
