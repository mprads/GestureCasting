using Godot;
using System;

namespace Game.Camera;

public partial class CameraController : Node3D {
    [Export]
    public float MouseSensitivity = 0.005f;
    [Export]
    public float MaxVerticalAngle =  Mathf.Pi / 2;
    [Export]
    public float MinVerticalAngle = -Mathf.Pi / 2;

    private Camera3D playerCamera;
    private bool cameraEnabled = true;
    private Vector2 mouseInput = Vector2.Zero;

    public override void _Ready() {
        playerCamera = GetNode<Camera3D>("%PlayerCamera");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion mouseMotion) {
            if (cameraEnabled) {
                RotateY(-mouseMotion.Relative.X * MouseSensitivity);
                playerCamera.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);
                playerCamera.Rotation = new Vector3(
                    Mathf.Clamp(
                        playerCamera.Rotation.X,
                        MinVerticalAngle,
                        MaxVerticalAngle
                    ),
                    playerCamera.Rotation.Y,
                    playerCamera.Rotation.Z);
            }
        }
    }

    public void EnableCamera() {
        cameraEnabled = true;
    }

    public void DisableCamera() {
        cameraEnabled = false;
    }
}
