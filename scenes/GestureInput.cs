using Game.Resources;
using Godot;
using System.Collections.Generic;

namespace Game;

public partial class GestureInput : Control {
    const string GESTURE_LIBRARY_PATH = "C:/Users/myles/Documents/GameDev/GestureCasting/resources/gesture_library";

    [Export]
    private int lineWitdth = 5;
    [Export]
    private bool lineAntiAlias = true;
    [Export]
    private Color lineColor = new("WHITE");
    [Export]
    private Godot.Collections.Array<Gesture> gestureLibray = []; // Cannot export a typed list

    private Button saveButton;
    private TextEdit gestureNameTextEdit;
    private Label gestureLabel;

    private Line2D stroke = null;
    private int strokeIndex = -1;
    private List<Point> points = null;
    private QPointCloudRecognizer Recognizer = new();
    
    public override void _Ready() {
        saveButton = GetNode<Button>("%SaveButton");
        gestureNameTextEdit = GetNode<TextEdit>("%GestureNameTextEdit");
        gestureLabel = GetNode<Label>("%GestureLabel");

        saveButton.Pressed += OnSaveButtonPressed;

        Recognizer.Init(gestureLibray);
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
        string gestureClass = Recognizer.Classify(candidate);

        gestureLabel.Text = gestureClass;
    }

    private void SaveGesture() {
        if (points.Count == 0) {
            return;
        }

        Gesture newGesture = new Gesture(points.ToArray());
        newGesture.Name = gestureNameTextEdit.Text;
        string path = GESTURE_LIBRARY_PATH + "/" + gestureNameTextEdit.Text + ".tres";

        ResourceSaver.Save(newGesture, path);
    }

    private void OnSaveButtonPressed() {
        SaveGesture();
    }
}
