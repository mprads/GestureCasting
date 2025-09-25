using System;
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
    public float CrouchDeceleration = 5.0f;

    public float AirSpeed = 30.0f;

    public float MaxSpeed = 30.0f;
    static public float JumpHeight = 2.0f;
    static public float JumpTimeToPeak = 0.3f;
    static public float JumpTimeToFall = .24f;
    public float JumpVelocity = 2.0f * JumpHeight / JumpTimeToPeak;
    public float JumpGravity = -2.0f * JumpHeight / (JumpTimeToPeak * JumpTimeToPeak);
    public float FallGravity = -2.0f * JumpHeight / (JumpTimeToFall * JumpTimeToFall);

    public Vector3 PreviousPosition;
    public Vector3 PreviousVelocity;
    public bool WasOnFloor;

    public Node3D CameraController;

    private PlayerStateMachine playerStateMachine;

    private Label stateLabel;
    private Label velocityLabel;


    public override void _Ready() {
        CameraController = GetNode<Node3D>("%CameraController");
        playerStateMachine = GetNode<PlayerStateMachine>("%StateMachine");
        stateLabel = GetNode<Label>("%StateLabel");
        velocityLabel = GetNode<Label>("%VelocityLabel");
        playerStateMachine.Init(this);
    }

    public override void _Process(double delta) {
        stateLabel.Text = playerStateMachine.GetCurrentStateName();
        velocityLabel.Text = Velocity.ToString();
    }


    public override void _PhysicsProcess(double delta) {
        PreviousPosition = Position;
        PreviousVelocity = Velocity;
        WasOnFloor = IsOnFloor();
    }

}
