using Godot;

namespace Game.Resources;

[GlobalClass]
public partial class Point : Resource {

    [Export]
    public float X, Y;
    [Export]
    public int intX, intY, StrokeID;

    public Point() : this(0f, 0f, 0) {}

    public Point(float x, float y, int strokeId) {
        this.X = x;
        this.Y = y;
        this.StrokeID = strokeId;
        this.intX = 0;
        this.intY = 0;
    }

    public override string ToString() {
        return $"X: {this.X}, Y: {this.Y}, ID: {this.StrokeID}";
    }
}