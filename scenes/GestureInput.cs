using Game;
using Godot;
using System.Collections.Generic;

public partial class GestureInput : Node2D {
    [Export]
    private int lineWitdth = 5;
    [Export]
    private bool lineAntiAlias = true;
    [Export]
    private Color lineColor = new Color("WHITE");

    private Line2D stroke = null;
    private int strokeIndex = -1;
    private List<Point> points = null;
    private QPointCloudRecognizer Recognizer = new QPointCloudRecognizer();

    public override void _Ready() {
        // Recognizer.Init();
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
            stroke = new Line2D();
            stroke.BeginCapMode = Line2D.LineCapMode.Round;
            stroke.EndCapMode = Line2D.LineCapMode.Round;
            stroke.DefaultColor = lineColor;
            stroke.Antialiased = lineAntiAlias;
            stroke.Width = lineWitdth;
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
        for (int i = 0; i < points.Count; i++) {
            GD.Print(points[i].ToString());
        } 
    }
}
