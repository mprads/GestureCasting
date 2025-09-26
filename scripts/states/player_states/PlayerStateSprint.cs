using Godot;

namespace Game.States;

public partial class PlayerStateSprint : PlayerState {
    public override void UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed("jump")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.JUMP);
        } else if (@event.IsActionPressed("crouch")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.CROUCH);
        }

        if (@event.IsActionReleased("sprint")) {
            if (player.Velocity != Vector3.Zero) {
                EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.WALK);
            }

        }
    }

    public override void Move(double delta) {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 moveDirection = (player.CameraController.GlobalBasis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y)).Normalized();

        if (moveDirection != Vector3.Zero) {
            Vector3 newVelocity = Vector3.Zero;
            newVelocity.X = Mathf.Lerp(player.Velocity.X, moveDirection.X * player.SprintSpeed, player.SprintAcceleration * (float)delta);
            newVelocity.Z = Mathf.Lerp(player.Velocity.Z, moveDirection.Z * player.SprintSpeed, player.SprintAcceleration * (float)delta);
            newVelocity.Y = player.Velocity.Y;

            player.Velocity = newVelocity;
            player.MoveAndSlide();
        } else {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.WALK);
        }
    }
}
