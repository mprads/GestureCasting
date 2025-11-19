using System.Linq;
using Game.Entities;
using Godot;

namespace Game.Components;

public partial class LineOfSightComponent : Area3D {
    [Export]
    public Godot.Collections.Array<Node3D> Overlaps = new();
    public Player Owner;

    private RayCast3D lineOfSightRayCast;
    private Timer lineOfSightTimer;
    private GodotObject target;

    public override void _Ready() {
        lineOfSightRayCast = GetNode<RayCast3D>("%LineOfSightRayCast");
        lineOfSightTimer = GetNode<Timer>("%LineOfSightTimer");

        lineOfSightTimer.Timeout += OnLineOfSightTimerTimeout;
        lineOfSightRayCast.AddException(Owner);
    }

    public Node3D GetTarget() {
        // Todo probably need to do some check to make sure the target is another player
        // or some sort of valid target
        return (Node3D)target;
    }

    private void OnLineOfSightTimerTimeout() {
        Godot.Collections.Array<Node3D> overlaps = GetOverlappingBodies();
        if (overlaps.Any()) {
            Overlaps = overlaps;
            foreach (Node3D overlap in overlaps) {
                if (overlap is Player) {
                    Vector3 targetPosition = overlap.GlobalTransform.Origin;
                    lineOfSightRayCast.LookAt(targetPosition, Vector3.Up);
                    lineOfSightRayCast.ForceRaycastUpdate();

                    if (lineOfSightRayCast.IsColliding()) {
                        GodotObject collider = lineOfSightRayCast.GetCollider();

                        if (collider == overlap) {
                            target = collider;
                            return;
                        }
                    }
                }
            }
        }

        target = null;
    }

}
