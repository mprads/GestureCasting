using Godot;
using System;

namespace Game;

public class Point {
    public float X, Y;
    public int StrokeID;
    public int intX, intY;

    public Point(float x, float y, int strokeId) {
        this.X = x;
        this.Y = y;
        this.StrokeID = strokeId;
        this.intX = 0;
        this.intY = 0;
    }
}