using System.Linq;
using Game.Entities;
using Godot;

namespace Game.Components;

public partial class LineOfSiteComponent : Area3D {
    private RayCast3D lineOfSiteRayCast;
    private Timer lineOfSiteTimer;
    public override void _Ready() {
        lineOfSiteRayCast = GetNode<RayCast3D>("%LineOfSiteRayCast");
        lineOfSiteTimer = GetNode<Timer>("%LineOfSiteTimer");

        lineOfSiteTimer.Timeout += OnLineOfSiteTimerTimeout;
    }

    private void OnLineOfSiteTimerTimeout() {
        Godot.Collections.Array<Node3D> overlaps = GetOverlappingBodies();
        if (overlaps.Any()) {
            GD.Print($"{overlaps.Count()}");
            foreach (Node3D overlap in overlaps) {
                // TODO dislike using groups change to player or entity class
                if (overlap.IsInGroup("target_dummy")) {
                    GD.Print("dummy in range");
                    Vector3 targetPosition = overlap.GlobalTransform.Origin;
                    lineOfSiteRayCast.LookAt(targetPosition, Vector3.Up);
                    lineOfSiteRayCast.ForceRaycastUpdate();

                    if (lineOfSiteRayCast.IsColliding()) {
                        GodotObject collider = lineOfSiteRayCast.GetCollider();

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
