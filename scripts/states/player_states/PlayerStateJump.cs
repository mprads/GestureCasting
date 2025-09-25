using Game.Entities;
using Godot;

namespace Game.States;

public partial class PlayerStateJump : PlayerState {
    public override void Enter() {
        Jump();
    }

    public override void Move(double delta) {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 moveDirection = (player.CameraController.GlobalBasis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y)).Normalized();

        if (moveDirection != Vector3.Zero) {
            Vector3 newVelocity = Vector3.Zero;
            newVelocity.X = Mathf.Lerp(player.Velocity.X, moveDirection.X * player.WalkSpeed, player.WalkAcceleration * (float)delta);
            newVelocity.Z = Mathf.Lerp(player.Velocity.Z, moveDirection.Z * player.WalkSpeed, player.WalkAcceleration * (float)delta);

            player.Velocity = newVelocity;
        }


        if (player.Velocity.Length() > player.MaxSpeed) {
            player.Velocity = player.Velocity.Normalized() * player.MaxSpeed;
        }

        player.MoveAndSlide();
    }

    private void Jump() {
        Vector3 jumpVelocity = player.Velocity;
        jumpVelocity.Y = player.JumpVelocity;
        player.Velocity = jumpVelocity;
    }
}
