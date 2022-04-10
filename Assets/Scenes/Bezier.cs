using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Vector2.Lerp(a, b, t), Vector2.Lerp(b, c, t), t);
    }

    public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(EvaluateQuadratic(a, b, c, t), EvaluateQuadratic(b, c, d, t), t);
    }
}
 