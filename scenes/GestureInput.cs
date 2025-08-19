using Game;
using Game.Resources;
using Godot;
using System;
using System.Collections.Generic;

namespace Game;

public partial class GestureInput : Node2D {
    [Export]
    private int lineWitdth = 5;
    [Export]
    private bool lineAntiAlias = true;
    [Export]
    private Color lineColor = new("WHITE");

    private Line2D stroke = null;
    private int strokeIndex = -1;
    private List<Point> points = null;
    private QPointCloudRecognizer Recognizer = new();

    public override void _Ready() {
        Recognizer.Init();
    }

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("validate_gesture")) {
            strokeIndex = -1;
            RecognizeGesture();
            // Clear line2d
        }
        if (@event.IsActionPressed("draw_line")) {
            if (strokeIndex == -1) {
                points = new List<Point>();
            }
            stroke = new Line2D {
                BeginCapMode = Line2D.LineCapMode.Round,
                EndCapMode = Line2D.LineCapMode.Round,
                DefaultColor = lineColor,
                Antialiased = lineAntiAlias,
                Width = lineWitdth
            };
            strokeIndex++;
            AddChild(stroke);
        }
        if (@event.IsActionReleased("draw_line")) {
            stroke = null;
        }
    }

    public override void _Process(double delta) {
        if (stroke != null) {
            Vector2 mousePos = GetGlobalMousePosition();
            stroke.AddPoint(mousePos);
            points.Add(new Point(mousePos.X, mousePos.Y, strokeIndex));
        }
    }

    private void RecognizeGesture() {
        Gesture candidate = new Gesture(points.ToArray());
        // GD.Print(candidate.Points);
        // string gestureClass = Recognizer.Classify(candidate);

        // GD.Print(gestureClass);
    }
}
