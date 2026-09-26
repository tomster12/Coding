using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class Geometry
    {
        public static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        public static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float lenSqr = ab.sqrMagnitude;

            if (lenSqr < Mathf.Epsilon)
                return Vector2.Distance(p, a);

            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / lenSqr);
            return Vector2.Distance(p, a + ab * t);
        }
    }
}