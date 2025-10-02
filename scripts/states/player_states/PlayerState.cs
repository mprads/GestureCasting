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

    // TODO change this to accept speed and acceleration, but defualt to walk
    // maybe add flag to exclude transistioning to idle for in air and casting
    public virtual void Move(double delta) { }

    public virtual void ApplyGravity(double delta) {
        if (!player.IsOnFloor()) {
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
    }

    public virtual void CheckFloor() {
        if (!player.IsOnFloor()) {
            EmitSignal(SignalName.TransitionRequested, this, (int)PlayerStateMachine.STATE.INAIR);
        }
    }
}
