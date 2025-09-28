using Godot;

namespace Game.States;

public partial class PlayerStateIdle : PlayerState {

     public override void UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("jump")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.JUMP);
        } else if (@event.IsActionPressed("crouch")) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.CROUCH);
        }
    }

     public override void Move(double delta) {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 moveDirection = (player.CameraController.GlobalBasis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y)).Normalized();
        Vector3 newVelocity = Vector3.Zero;
        newVelocity.Y = player.Velocity.Y;

        if (moveDirection != Vector3.Zero) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.WALK);
        } else {
            newVelocity.X = Mathf.Lerp(player.Velocity.X, 0.0f, player.Deceleration * (float)delta);
            newVelocity.Z = Mathf.Lerp(player.Velocity.Z, 0.0f, player.Deceleration * (float)delta);
        }

        player.Velocity = newVelocity;
        player.MoveAndSlide();
    }

}
