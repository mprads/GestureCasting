using Godot;
using System;
using System.Collections.Generic;

namespace Game.Resources;

public partial class Gesture : Resource {
    [Export]
    private string GestureName;

    public Point[] Points = null;
    public Point[] PointsInt = null;

    private const int SAMPLING_RESOLUTION = 64;                             // default number of points on the gesture path
    private const int MAX_INT_COORDINATES = 1024;                           // $Q only: each point has two additional x and y integer coordinates in the interval [0..MAX_INT_COORDINATES-1] used to operate the LUT table efficiently (O(1))
    public static int LUT_SIZE = 64;                                        // $Q only: the default size of the lookup table is 64 x 64
    public static int LUT_SCALE_FACTOR = MAX_INT_COORDINATES / LUT_SIZE;    // $Q only: scale factor to convert between integer x and y coordinates and the size of the LUT
    public int[][] LUT = null;
}
