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
        } else if (@event.IsActionPressed("toggle_mouse_mode")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.CASTING);
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
        } else {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.IDLE);
        }

        player.Velocity = newVelocity;
        player.MoveAndSlide();
    }
}
