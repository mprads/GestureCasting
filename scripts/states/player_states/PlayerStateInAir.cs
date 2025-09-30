using Godot;

namespace Game.States;

public partial class PlayerStateInAir : PlayerState {

    public override void Enter() {
        if (player.Velocity.Y <= 0) {
            player.CoyoteTimer.Start();
        }
        
    }

    public override void Exit() {
        player.CoyoteTimer.Stop();
        player.JumpBufferTimer.Stop();
    }


    public override void UnhandledInput(InputEvent @event) {
        if (@event.IsActionPressed("jump")) {
            if (player.CoyoteTimer.TimeLeft > 0.0f) {
                EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.JUMP);
            } else {
                player.JumpBufferTimer.Start();
            }
        }
    }

    public override void Move(double delta) {
        if (player.IsOnFloor()) return;

        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 moveDirection = (player.CameraController.GlobalBasis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y)).Normalized();

        if (!player.IsOnFloor()) {
            if (moveDirection != Vector3.Zero) {
                Vector3 newVelocity = Vector3.Zero;
                newVelocity.X = Mathf.Lerp(player.Velocity.X, moveDirection.X * player.SprintSpeed, player.SprintAcceleration * (float)delta);
                newVelocity.Z = Mathf.Lerp(player.Velocity.Z, moveDirection.Z * player.SprintSpeed, player.SprintAcceleration * (float)delta);
                newVelocity.Y = player.Velocity.Y;

                player.Velocity = newVelocity;
            }
        }

        player.MoveAndSlide();
    }

    public override void CheckFloor() {
        if (player.IsOnFloor()) {
            if (player.JumpBufferTimer.TimeLeft > 0.0f) {
                EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.JUMP);
            } else {
                EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.WALK);
            }
        }
    }
}
