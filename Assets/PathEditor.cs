using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    public PathCreator creator;
    Path path
    {
        get
        {
            return creator.path;
        }
    }

    const float segmentSelectDistanceThreshold = 0.1f;
    int selectedSegmentIndex = -1;

    private void OnEnable()
    {
        creator = (PathCreator)target;
        if (creator.path == null)
        {
            creator.CreatePath();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Create New"))
        {
            Undo.RecordObject(creator, "Create New");
            creator.CreatePath();
        }

        bool isClosed = GUILayout.Toggle(path.IsClosed, "Closed");
        if (isClosed != path.IsClosed)
        {
            Undo.RecordObject(creator, "Toggle Closed");
            path.IsClosed = isClosed;
        }

        bool autoSetControlPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
        if (autoSetControlPoints != path.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle Auto Set Control Points");
            path.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI()
    {
        Draw();
        Input();
    }

    private void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            if (selectedSegmentIndex != -1)
            {
                Undo.RecordObject(creator, "Split Segmentt");
                path.SplitSegment(mousePos, selectedSegmentIndex);
            }
            else if (!path.IsClosed)
            {
                Undo.RecordObject(creator, "Add Segmentt");
                path.AddSegment(mousePos);
            }
        }
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
        {
            float minDistanceToAnchor = creator.anchorDiameter * 0.5f;
            int closestAnchorIndex = -1;
            for (int i = 0; i < path.NumPoints; i+=3)
            {
                float distance = Vector2.Distance(mousePos, path[i]);
                if (distance < minDistanceToAnchor)
                {
                    minDistanceToAnchor = distance;
                    closestAnchorIndex = i;
                }
            }

            if (closestAnchorIndex != -1)
            {
                Undo.RecordObject(creator, "Delete Segment");
                path.DeleteSegment(closestAnchorIndex);
            }
        }

        if (guiEvent.type == EventType.MouseMove)
        {
            float minDistanceToSegment = segmentSelectDistanceThreshold;
            int newSelectedSegmentIndex = -1;

            for (int i = 0; i < path.NumSegments; i++)
            {
                Vector2[] points = path.GetPointsInSegment(i);
                float distance = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);
                if (distance < minDistanceToSegment)
                {
                    minDistanceToSegment = distance;
                    newSelectedSegmentIndex = i;
                }
            }

            if (newSelectedSegmentIndex != selectedSegmentIndex)
            {
                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }

        HandleUtility.AddDefaultControl(0);
    }

    public void Draw()
    {

        if (creator.displayBezier)
        {
            for (int i = 0; i < path.NumSegments; i++)
            {
                Vector2[] points = path.GetPointsInSegment(i);
                Handles.color = Color.black;
                if (creator.displayControlPoints)
                {
                    Handles.DrawLine(points[1], points[0]);
                    Handles.DrawLine(points[2], points[3]);
                }
                Color segmentColor = i == selectedSegmentIndex && Event.current.shift ? creator.selectedSegmentCol : creator.segmentCol;
                Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentColor, null, 2);
            }
        }

        if (creator.displayControlPoints)
        {
            for (int i = 0; i < path.NumPoints; i++)
            {
                if (i % 3 == 0 || creator.displayControlPoints)
                {
                    Handles.color = i % 3 == 0 ? creator.anchorCol : creator.controlCol;
                    float handleSize = i % 3 == 0 ? creator.anchorDiameter : creator.controlDiameter;
                    Vector2 newPosition = Handles.FreeMoveHandle(path[i], Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
                    if (path[i] != newPosition)
                    {
                        Undo.RecordObject(creator, "Move Point");
                        path.MovePoint(i, newPosition);
                    }
                }
            }
        }
        if (creator.displayEvenlySpacedPoints)
        {
            Vector2[] points1 = path.CalculateEvenlySpacedPoints(creator.spacing, creator.resolution);
            Handles.color = creator.evenlySpacedpointsColor;
            foreach (Vector2 p in points1)
            {
                Handles.FreeMoveHandle(p, Quaternion.identity, creator.evenlySpacedPointsDiameter, Vector2.zero, Handles.CylinderHandleCap);
            }
        }
    }
}
