using Godot;

namespace Game.States;

public partial class PlayerStateInAir : PlayerState {

    public override void Enter() {
        // Set up timers for buffer and coyote jump
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

                player.Velocity = newVelocity;
            }
        }

        player.MoveAndSlide();
    }

}
