using System.Linq;
using Game.Entities;
using Godot;

namespace Game.Components;

public partial class LineOfSightComponent : Area3D {
    private RayCast3D lineOfSightRayCast;
    private Timer lineOfSightTimer;
    public override void _Ready() {
        lineOfSightRayCast = GetNode<RayCast3D>("%LineOfSightRayCast");
        lineOfSightTimer = GetNode<Timer>("%LineOfSightTimer");

        lineOfSightTimer.Timeout += OnLineOfSightTimerTimeout;
    }

    private void OnLineOfSightTimerTimeout() {
        Godot.Collections.Array<Node3D> overlaps = GetOverlappingBodies();
        if (overlaps.Any()) {
            foreach (Node3D overlap in overlaps) {
                // TODO dislike using groups change to player or entity class
                if (overlap.IsInGroup("target_dummy")) {
                    Vector3 targetPosition = overlap.GlobalTransform.Origin;
                    lineOfSightRayCast.LookAt(targetPosition, Vector3.Up);
                    lineOfSightRayCast.ForceRaycastUpdate();

                    if (lineOfSightRayCast.IsColliding()) {
                        GodotObject collider = lineOfSightRayCast.GetCollider();

                        // TODO add ownership, only for target dummy testing
                        if (collider is not Player) {
                            if (collider == overlap) {
                                 GD.Print("dummy line of sight");
                            }
                        }
                    }
                }
            }
        }
    }

}
