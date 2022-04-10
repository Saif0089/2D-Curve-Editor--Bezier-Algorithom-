using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [HideInInspector]
    public Path path;

    public int curveIndex = 0;

    public float moveSpeed = 2f;

    private bool coroutineAllowed = true;

    private Vector3 newPosition;

    private void OnEnable()
    {
        path = FindObjectOfType<PathCreator>().path;
    }

    private void Update()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(MoveWithCurve(curveIndex));
        }
    }

    float t = 0f;

    private IEnumerator MoveWithCurve(int i)
    {
        coroutineAllowed = false;
        Vector2[] points = path.GetPointsInSegment(i);
        Vector2 p0 = points[0];
        Vector2 p1 = points[1];
        Vector2 p2 = points[2];
        Vector2 p3 = points[3];
        
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            newPosition = CubicCurve(p0, p1, p2, p3, t);

            transform.position = newPosition;
            yield return new WaitForEndOfFrame();
        }

        t = 0f;
        curveIndex++;
        if (curveIndex > 0)
        {
            curveIndex = 0;
        }
        coroutineAllowed = true;
    }

    public static Vector2 QuadraticCurve(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Vector2.Lerp(a, b, t), Vector2.Lerp(b, c, t), t);
    }

    public static Vector2 CubicCurve(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(QuadraticCurve(a, b, c, t), QuadraticCurve(b, c, d, t), t);
    }
}
