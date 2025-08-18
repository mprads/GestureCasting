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
using Game.Resources;
using Godot;

namespace Game;

public partial class QPointCloudRecognizer : Node {
    const string gestureLibraryPath = "res://resources/gesture_library";

    [Signal]
    public delegate void GestureClassifiedEventHandler(string gestureName);

    [Export]
    private bool UseEarlyAbandoning = true;
    [Export]
    private bool UseLowerBounding = true;

    private List<Gesture> GestureSet = new();

    public void Init() {
        var dir = DirAccess.Open(gestureLibraryPath);

        if (dir != null) {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "") {
                string resourceName = gestureLibraryPath + fileName.ToString();
                Resource gestureResource = ResourceLoader.Load(resourceName);
                GestureSet.Add((Gesture)gestureResource);
                fileName = dir.GetNext();
            }
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
        int n = gesture1.Points.Length;
        float eps = 0.5f;
        int step = (int)Math.Floor(Math.Pow(n, 1.0f - eps));


        if (UseLowerBounding) {
            float[] LB1 = ComputeLowerBound(gesture1.Points, gesture2.Points, gesture2.LUT, step);
            float[] LB2 = ComputeLowerBound(gesture2.Points, gesture1.Points, gesture1.LUT, step);
            for (int i = 0, indexLB = 0; i < n; i += step, indexLB++) {
                if (LB1[indexLB] < minSoFar) minSoFar = Math.Min(minSoFar, CloudDistance(gesture1.Points, gesture2.Points, i, minSoFar));
                if (LB2[indexLB] < minSoFar) minSoFar = Math.Min(minSoFar, CloudDistance(gesture2.Points, gesture1.Points, i, minSoFar));
            }
        } else {
            for (int i = 0; i < n; i += step) {
                minSoFar = Math.Min(minSoFar, CloudDistance(gesture1.Points, gesture2.Points, i, minSoFar));
                minSoFar = Math.Min(minSoFar, CloudDistance(gesture2.Points, gesture1.Points, i, minSoFar));
            }   
        }

        return minSoFar;
    }

    private float[] ComputeLowerBound(Point[] points1, Point[] points2, int[][] LUT, int step) {
        int n = points1.Length;
        float[] LB = new float[n / step + 1];
        float[] SAT = new float[n];

        LB[0] = 0;
        for (int i = 0; i < n; i++) {
            int index = LUT[points1[i].intY / Gesture.LUT_SCALE_FACTOR][points1[i].intX / Gesture.LUT_SCALE_FACTOR];
            float dist = SqrEuclideanDistance(points1[i], points2[index]);
            SAT[i] = (i == 0) ? dist : SAT[i - 1] + dist;
            LB[0] += (n - i) * dist;
        }

        for (int i = step, indexLB = 1; i < n; i += step, indexLB++)
            LB[indexLB] = LB[0] + i * SAT[n - 1] - n * SAT[i - 1];
        return LB;
    }

     private float CloudDistance(Point[] points1, Point[] points2, int startIndex, float minSoFar) {
        int n = points1.Length;                
        int[] indexesNotMatched = new int[n];
        for (int j = 0; j < n; j++) {
             indexesNotMatched[j] = j;
        }

        float sum = 0;                
        int i = startIndex;           
        int weight = n;              
        int indexNotMatched = 0;      
        do {
            int index = -1;
            float minDistance = float.MaxValue;
            for (int j = indexNotMatched; j < n; j++) {
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

            i = (i + 1) % n;
            indexNotMatched++;
        } while (i != startIndex);

        return sum;
        }

    private float SqrEuclideanDistance(Point a, Point b) {
        return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
    }
}
