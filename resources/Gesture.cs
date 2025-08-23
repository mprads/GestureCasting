using Godot;

namespace Game.Resources;

[GlobalClass]
public partial class Gesture : Resource {
    [Export]
    public string Name = "";

    [Export]
    public Point[] Points, PointsRaw = [];

    private const int SAMPLING_RESOLUTION = 64;                             // default number of points on the gesture path
    private const int MAX_INT_COORDINATES = 1024;                           // $Q only: each point has two additional x and y integer coordinates in the interval [0..MAX_INT_COORDINATES-1] used to operate the LUT table efficiently (O(1))
    public static int LUT_SIZE = 64;                                        // $Q only: the default size of the lookup table is 64 x 64
    public static int LUT_SCALE_FACTOR = MAX_INT_COORDINATES / LUT_SIZE;    // $Q only: scale factor to convert between integer x and y coordinates and the size of the LUT
    public int[][] LUT = null;


    public Gesture() : this([], null) { }

    public Gesture(Point[] points, string gestureName = "") {
        this.Name = gestureName;
        this.PointsRaw = new Point[points.Length];
        for (int i = 0; i < points.Length; i++) {
            this.PointsRaw[i] = new Point(points[i].X, points[i].Y, points[i].StrokeID);
        }

        if (points.Length <= 0) return;

        this.Normalize();
    }

    public void Normalize(bool computeLUT = true) {
        this.Points = Resample(PointsRaw, SAMPLING_RESOLUTION);
        this.Points = Scale(Points);
        this.Points = TranslateTo(Points, Centroid(Points));

        if (computeLUT) {
            this.TransformCoordinatesToIntegers();
            this.ConstructLUT();
        }
    }

    public Point[] Resample(Point[] points, int resolution) {
        Point[] newPoints = new Point[resolution];
        newPoints[0] = new Point(points[0].X, points[0].Y, points[0].StrokeID);
        int numPoints = 1;

        float interval = PathLength(points) / (resolution - 1);
        float sum = 0f;
        for (int i = 1; i < points.Length; i++) {
            if (points[i].StrokeID == points[i - 1].StrokeID) {
                float distance = QPointCloudRecognizer.EuclideanDistance(points[i - 1], points[i]);
                if (sum + distance >= interval) {
                    Point firstPoint = points[i - 1];
                    while (sum + distance >= interval) {
                        float minInterpolation = Mathf.Min(Mathf.Max((interval - sum) / distance, 0.0f), 1.0f);
                        if (float.IsNaN(minInterpolation)) minInterpolation = 0.5f;
                        newPoints[numPoints++] = new Point(
                            (1.0f - minInterpolation) * firstPoint.X + minInterpolation * points[i].X,
                            (1.0f - minInterpolation) * firstPoint.Y + minInterpolation * points[i].Y,
                            points[i].StrokeID
                        );
                        distance = sum + distance - interval;
                        sum = 0f;
                        firstPoint = newPoints[numPoints - 1];
                    }
                    sum = distance;
                } else {
                    sum += distance;
                }
            }
        }

        if (numPoints == resolution - 1) { // sometimes we fall a rounding-error short of adding the last point, so add it if so
            newPoints[numPoints++] = new Point(points[points.Length - 1].X, points[points.Length - 1].Y, points[points.Length - 1].StrokeID);
        }

        return newPoints;
    }

    public void ConstructLUT() {
        this.LUT = new int[LUT_SIZE][];
        for (int i = 0; i < LUT_SIZE; i++) {
            LUT[i] = new int[LUT_SIZE];
        }

        for (int i = 0; i < LUT_SIZE; i++) {
            for (int j = 0; j < LUT_SIZE; j++) {
                int minDistance = int.MaxValue;
                int indexMin = -1;
                for (int t = 0; t < Points.Length; t++) {
                    int row = Points[t].intY / LUT_SCALE_FACTOR;
                    int col = Points[t].intX / LUT_SCALE_FACTOR;
                    int dist = (row - i) * (row - i) + (col - j) * (col - j);
                    if (dist < minDistance) {
                        minDistance = dist;
                        indexMin = t;
                    }
                }
                LUT[i][j] = indexMin;
            }
        }
    }

    private Point[] Scale(Point[] points) {
        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        for (int i = 0; i < points.Length; i++) {
            if (minX > points[i].X) minX = points[i].X;
            if (minY > points[i].Y) minY = points[i].Y;
            if (maxX < points[i].X) maxX = points[i].X;
            if (maxY < points[i].Y) maxY = points[i].Y;
        }

        Point[] newPoints = new Point[points.Length];
        float scale = Mathf.Max(maxX - minX, maxY - minY);
        for (int i = 0; i < points.Length; i++) {
            newPoints[i] = new Point((points[i].X - minX) / scale, (points[i].Y - minY) / scale, points[i].StrokeID);
        }

        return newPoints;
    }

    private Point Centroid(Point[] points) {
        float centerX = 0, centerY = 0;
        for (int i = 0; i < points.Length; i++) {
            centerX += points[i].X;
            centerY += points[i].Y;
        }

        return new Point(centerX / points.Length, centerY / points.Length, 0);
    }

    private void TransformCoordinatesToIntegers() {
        for (int i = 0; i < Points.Length; i++) {
            Points[i].intX = (int)((Points[i].X + 1.0f) / 2.0f * (MAX_INT_COORDINATES - 1));
            Points[i].intY = (int)((Points[i].Y + 1.0f) / 2.0f * (MAX_INT_COORDINATES - 1));
        }
    }

    private Point[] TranslateTo(Point[] points, Point point) {
        Point[] newPoints = new Point[points.Length];
        for (int i = 0; i < points.Length; i++)
            newPoints[i] = new Point(points[i].X - point.X, points[i].Y - point.Y, points[i].StrokeID);
        return newPoints;
    }

    private float PathLength(Point[] points) {
        float length = 0;
        for (int i = 1; i < points.Length; i++) {
            if (points[i].StrokeID == points[i - 1].StrokeID)
                length += QPointCloudRecognizer.EuclideanDistance(points[i - 1], points[i]);
        }

        return length;
    }
}
