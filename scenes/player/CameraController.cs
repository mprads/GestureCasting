using Godot;
using System;

namespace Game.Camera;

public partial class CameraController : Node3D {
    [Export]
    public float MouseSensitivity = 0.005f;

    private Vector2 mouseInput = Vector2.Zero;

    public override void _Ready() {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion castedEvent) {
            RotateY(-castedEvent.Relative.X * MouseSensitivity);
        }
    }
}
