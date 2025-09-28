using Godot;
using System;

namespace Game.Camera;

public partial class CameraController : Node3D {
    [Export]
    public float MouseSensitivity = 0.005f;

    private bool cameraEnabled = true;
    private Vector2 mouseInput = Vector2.Zero;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion castedEvent) {
            if (cameraEnabled) {
                RotateY(-castedEvent.Relative.X * MouseSensitivity);
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
