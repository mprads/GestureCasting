using Godot;

namespace Game.Entities;

public partial class Player : CharacterBody3D {
    public float WalkSpeed = 8.0f;
    public float WalkAcceleration = 8.0f;
    public float WalkDeceleration = 8.0f;

    public float SprintSpeed = 12.0f;
    public float SprintAcceleration = 5.0f;
    public float SprintDeceleration = 5.0f;

     public float CrouchSpeed = 4.0f;
    public float CrouchAcceleration = 2.0f;
    public float CrouchDeceleration = 2.0f;

    public float JumpHeight = 2.0f;
    public float JumpTimeToPeak = 0.3f;
    public float JumpTimeToFall = .24f;
    public float JumpVelocity = 0.2f;

    public Node3D CameraController;

    private PlayerStateMachine playerStateMachine;


    public override void _Ready() {
        CameraController = GetNode<Node3D>("%CameraController");
        playerStateMachine = GetNode<PlayerStateMachine>("%StateMachine");
        playerStateMachine.Init(this);
    }
}
