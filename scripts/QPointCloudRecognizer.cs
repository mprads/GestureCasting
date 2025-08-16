// Source code adapted from https://depts.washington.edu/acelab/proj/dollar/qdollar.html

//	Vatavu, R.-D., Anthony, L. and Wobbrock, J.O. (2018).  
//	  $Q: A Super-Quick, Articulation-Invariant Stroke-Gesture
//    Recognizer for Low-Resource Devices. Proceedings of 20th International Conference on
//    Human-Computer Interaction with Mobile Devices and Services (MobileHCI '18). Barcelona, Spain
//	  (September 3-6, 2018). New York: ACM Press.
//	  DOI: https://doi.org/10.1145/3229434.3229465
//
// This software is distributed under the "New BSD License" agreement:

// Copyright (c) 2018, Radu-Daniel Vatavu, Lisa Anthony, and 
// Jacob O. Wobbrock. All rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;
using Game.Resources;
using Godot;

namespace Game;

public partial class QPointCloudRecognizer : Node {
    const string gestureLibraryPath = "res://gesture_library";

    [Signal]
    public delegate void GestureClassifiedEventHandler(string gestureName);

    [Export]
    private bool earlyAbandoning = true;
    [Export]
    private bool lowerBounding = true;

    private List<Gesture> gestureSet = new();

    public void Init() {
        var dir = DirAccess.Open(gestureLibraryPath);

        if (dir != null) {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "") {
                string resourceName = gestureLibraryPath + fileName.ToString();
                Resource gestureResource = ResourceLoader.Load(resourceName);
                gestureSet.Add((Gesture)gestureResource);
                fileName = dir.GetNext();
            }
        }
    }

    public string Classify(Gesture candidate) {
        float minDistance = float.PositiveInfinity;
        string gestureClass = "";
        foreach (Gesture template in gestureSet) {
            float distance;
        }

        return gestureClass;
    }

    private float GreedyCloudMatch(Gesture gesture1, Gesture gesture2, float minSoFar) {
        int n = gesture1.PointsInt.Count();
        float eps = 0.5f;

        return minSoFar;
    }

    private float[] ComputeLowerBound(Gesture gesture1, Gesture gesture2, int[][] LUT, int step) {
        int n = gesture1.Points.Count();
        float[] LB = new float[n / step + 1];
        float[] SAT = new float[n];

        LB[0] = 0;
        for (int i = 0; i < n; i++) {
            int index = LUT[gesture1.Points[i].intY / Gesture.LUT_SCALE_FACTOR][gesture1.Points[i].intX / Gesture.LUT_SCALE_FACTOR];
            float dist = SqrEuclideanDistance(gesture1.Points[i], gesture2.Points[index]);
            SAT[i] = (i == 0) ? dist : SAT[i - 1] + dist;
            LB[0] += (n - i) * dist;
        }

        for (int i = step, indexLB = 1; i < n; i += step, indexLB++)
            LB[indexLB] = LB[0] + i * SAT[n - 1] - n * SAT[i - 1];
        return LB;
    }

    private float SqrEuclideanDistance(Point a, Point b) {
        return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
    }
}
