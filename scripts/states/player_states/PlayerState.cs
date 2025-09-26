using Godot;

namespace Game.States;

[GlobalClass]
public partial class PlayerState : Node {

    [Signal]
    public delegate void TransitionRequestedEventHandler(PlayerState from, PlayerStateMachine.STATE to);

    public Entities.Player player;

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void PhysicsProcess(double delta) {
        ApplyGravity(delta);
        CheckFloor();
        Move(delta);
    }

    public virtual void UnhandledInput(InputEvent @event) { }

    public virtual void Move(double delta) {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        Vector3 moveDirection = (player.CameraController.GlobalBasis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y)).Normalized();

        if (moveDirection != Vector3.Zero) {
            Vector3 newVelocity = Vector3.Zero;
            newVelocity.X = Mathf.Lerp(player.Velocity.X, moveDirection.X * player.WalkSpeed, player.WalkAcceleration * (float)delta);
            newVelocity.Z = Mathf.Lerp(player.Velocity.Z, moveDirection.Z * player.WalkSpeed, player.WalkAcceleration * (float)delta);
            newVelocity.Y = player.Velocity.Y;

            player.Velocity = newVelocity;
            player.MoveAndSlide();
        }
    }

    public virtual void ApplyGravity(double delta) {
        if (player.Velocity.Y >= 0.0f) {
            Vector3 jumpVelocity = player.Velocity;
            jumpVelocity.Y += player.JumpGravity * (float)delta;
            player.Velocity = jumpVelocity;
        } else if (player.Velocity.Y < 0.0f) {
            Vector3 fallVelocity = player.Velocity;
            fallVelocity.Y += player.FallGravity * (float)delta;
            player.Velocity = fallVelocity;
        }
    }

    public virtual void CheckFloor() {
        if (!player.IsOnFloor()) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.INAIR);
        }
    }
}
