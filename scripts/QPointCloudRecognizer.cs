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
    const string GESTURE_LIBRARY_PATH = "res://resources/gesture_library/";

    [Signal]
    public delegate void GestureClassifiedEventHandler(string gestureName);

    [Export]
    private bool UseEarlyAbandoning = true;
    [Export]
    private bool UseLowerBounding = true;

    private List<Gesture> GestureSet = new();

    public void Init(Godot.Collections.Array<Gesture> gestureLibray) {
        // var dir = DirAccess.Open(GESTURE_LIBRARY_PATH);

        // if (dir != null) {
        //     dir.ListDirBegin();
        //     string fileName = dir.GetNext();
        //     while (fileName != "") {
        //         string resourceName = GESTURE_LIBRARY_PATH + fileName.ToString();
        //         Resource gestureResource = ResourceLoader.Load<Gesture>(resourceName);
        //         GestureSet.Add((Gesture)gestureResource);
        //         fileName = dir.GetNext();
        //     }
        // }

        // Cannot convert GD Array to list so have to loop and add to GestureSet
        // Have to reconstruct LUT as Godot can't serialize the field, maybe change to dictionary
        foreach (Gesture gestureResource in gestureLibray) {
            gestureResource.ConstructLUT();
            GestureSet.Add(gestureResource);
        }
    }

    public string Classify(Gesture candidate) {
        float minDistance = float.MaxValue;
        string gestureClass = "";
        foreach (Gesture template in GestureSet) {
            float dist = GreedyCloudMatch(candidate, template, minDistance);
            if (dist < minDistance) {
                minDistance = dist;
                gestureClass = template.Name;
            }
        }

        return gestureClass;
    }

    private float GreedyCloudMatch(Gesture gesture1, Gesture gesture2, float minSoFar) {
        int gesturePointLength = gesture1.Points.Length;
        float eps = 0.5f;
        int step = (int)Math.Floor(Math.Pow(gesturePointLength, 1.0f - eps));


        if (UseLowerBounding) {
            float[] LB1 = ComputeLowerBound(gesture1.Points, gesture2.Points, gesture2.LUT, step);
            float[] LB2 = ComputeLowerBound(gesture2.Points, gesture1.Points, gesture1.LUT, step);
            for (int i = 0, indexLB = 0; i < gesturePointLength; i += step, indexLB++) {
                if (LB1[indexLB] < minSoFar) minSoFar = Math.Min(minSoFar, CloudDistance(gesture1.Points, gesture2.Points, i, minSoFar));
                if (LB2[indexLB] < minSoFar) minSoFar = Math.Min(minSoFar, CloudDistance(gesture2.Points, gesture1.Points, i, minSoFar));
            }
        } else {
            for (int i = 0; i < gesturePointLength; i += step) {
                minSoFar = Math.Min(minSoFar, CloudDistance(gesture1.Points, gesture2.Points, i, minSoFar));
                minSoFar = Math.Min(minSoFar, CloudDistance(gesture2.Points, gesture1.Points, i, minSoFar));
            }
        }

        return minSoFar;
    }

    private float[] ComputeLowerBound(Point[] points1, Point[] points2, int[][] LUT, int step) {
        int pointsLength = points1.Length;
        float[] LB = new float[pointsLength / step + 1];
        float[] SAT = new float[pointsLength];

        LB[0] = 0;
        for (int i = 0; i < pointsLength; i++) {
            int index = LUT[points1[i].intY / Gesture.LUT_SCALE_FACTOR][points1[i].intX / Gesture.LUT_SCALE_FACTOR];
            float dist = SqrEuclideanDistance(points1[i], points2[index]);
            SAT[i] = (i == 0) ? dist : SAT[i - 1] + dist;
            LB[0] += (pointsLength - i) * dist;
        }

        for (int i = step, indexLB = 1; i < pointsLength; i += step, indexLB++)
            LB[indexLB] = LB[0] + i * SAT[pointsLength - 1] - pointsLength * SAT[i - 1];
        return LB;
    }

    private float CloudDistance(Point[] points1, Point[] points2, int startIndex, float minSoFar) {
        int pointsLength = points1.Length;
        int[] indexesNotMatched = new int[pointsLength];
        for (int j = 0; j < pointsLength; j++) {
            indexesNotMatched[j] = j;
        }

        float sum = 0;
        int i = startIndex;
        int weight = pointsLength;
        int indexNotMatched = 0;
        do {
            int index = -1;
            float minDistance = float.MaxValue;
            for (int j = indexNotMatched; j < pointsLength; j++) {
                float dist = SqrEuclideanDistance(points1[i], points2[indexesNotMatched[j]]);
                if (dist < minDistance) {
                    minDistance = dist;
                    index = j;
                }
            }
            indexesNotMatched[index] = indexesNotMatched[indexNotMatched];
            sum += (weight--) * minDistance;
            if (UseEarlyAbandoning) {
                if (sum >= minSoFar)
                    return sum;
            }

            i = (i + 1) % pointsLength;
            indexNotMatched++;
        } while (i != startIndex);

        return sum;
    }

    public static float SqrEuclideanDistance(Point a, Point b) {
        return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
    }
    
    public static float EuclideanDistance(Point a, Point b) {
        return (float)Math.Sqrt(SqrEuclideanDistance(a, b));
    }
}
