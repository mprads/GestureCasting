using System;
using Game.Camera;
using Game.Components;
using Game.Managers;
using Godot;

namespace Game.Entities;

public partial class Player : CharacterBody3D {
    static readonly PackedScene PLAYER_SCENE = ResourceLoader.Load<PackedScene>("uid://d0gb5yk3bkywt");

    public PlayerInfo Info;

    public float WalkSpeed = 8.0f;
    public float WalkAcceleration = 8.0f;

    public float SprintSpeed = 12.0f;
    public float SprintAcceleration = 5.0f;

    public float CrouchSpeed = 4.0f;
    public float CrouchAcceleration = 2.0f;

    public float Deceleration = 8.0f;

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

    public CastManager CastManager;
    public CameraController CameraController;
    public Camera3D PlayerCamera;
    public GestureInput GestureInput;
    public Timer CoyoteTimer;
    public Timer JumpBufferTimer;

    private PlayerStateMachine playerStateMachine;

    // DEBUG lables
    private Label stateLabel;
    private Label velocityLabel;
    private Label horizontalVelocityLabel;
    private Label targetLabel;

    public override void _Ready() {
        CameraController = GetNode<CameraController>("%CameraController");
        PlayerCamera = GetNode<Camera3D>("%PlayerCamera");
        CastManager = GetNode<CastManager>("%CastManager");
        GestureInput = GetNode<GestureInput>("%GestureInput");
        CoyoteTimer = GetNode<Timer>("%CoyoteTimer");
        JumpBufferTimer = GetNode<Timer>("%JumpBufferTimer");
        playerStateMachine = GetNode<PlayerStateMachine>("%StateMachine");
        stateLabel = GetNode<Label>("%StateLabel");
        velocityLabel = GetNode<Label>("%VelocityLabel");
        horizontalVelocityLabel = GetNode<Label>("%HorizontalVelocityLabel");
        targetLabel = GetNode<Label>("%TargetLabel");

        GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(Info.Id);

        playerStateMachine.Init(this);
        CastManager.Init(this);

        if (CheckMultiplayerAuthority()) {
            PlayerCamera.Current = true;
        }
    }

    public override void _Process(double delta) {
         if (CheckMultiplayerAuthority()) {
            stateLabel.Text = playerStateMachine.GetCurrentStateName();
            velocityLabel.Text = Velocity.ToString();
            horizontalVelocityLabel.Text = $"{MathF.Abs(Velocity.X) + MathF.Abs(Velocity.Z)}";
            targetLabel.Text = GetNode<LineOfSightComponent>("%LineOfSightComponent").GetTarget() != null ? "Has Target" : "No Target";
        }
    }

    public override void _PhysicsProcess(double delta) {
        if (CheckMultiplayerAuthority()) {
            PreviousPosition = Position;
            PreviousVelocity = Velocity;
            WasOnFloor = IsOnFloor();
        }
    }

    public Transform3D GetSpellOriginTransform() {
        return CameraController.GetNode<Camera3D>("%PlayerCamera").GlobalTransform;
    }

    public bool CheckMultiplayerAuthority() {
        return GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId();
    }

    public static Player CreateNew(PlayerInfo info) {
        Player newPlayer = PLAYER_SCENE.Instantiate<Player>();
        newPlayer.Info = info;

        return newPlayer;
    }
}
