using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    [HideInInspector] public Path path;

    [Header("Bezier Settings")]
    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = 0.1f;
    public float controlDiameter = 0.075f;
    public bool displayControlPoints = true;
    public bool displayBezier = true;

    [Space(25)]
    [Header("Spaced Points Settings")]
    public Color evenlySpacedpointsColor = Color.white;
    public float spacing = 0.1f;
    public float resolution = 1f;
    public float evenlySpacedPointsDiameter = 0.04f;
    public bool displayEvenlySpacedPoints = false;


    public void CreatePath()
    {
        path = new Path(transform.position);
    }

    private void Reset()
    {
        CreatePath();
    }
}
