using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class PhysicsWorld : MonoBehaviour
    {
        readonly List<WorldObject> objects = new();
        public IReadOnlyList<WorldObject> Objects => objects;

        public void Register(WorldObject obj)
        {
            if (!objects.Contains(obj)) objects.Add(obj);
        }

        public void Unregister(WorldObject obj)
        {
            objects.Remove(obj);
        }

        public PhysicsVertex FindNearestVertex(Vector2 pos, float maxDist, out PhysicsGraph graph)
        {
            PhysicsVertex nearest = null;
            graph = null;
            float bestSqr = maxDist * maxDist;

            foreach (var obj in objects)
            {
                if (obj.Graph == null) continue;

                foreach (var v in obj.Graph.Vertices)
                {
                    float sqr = (v.Position - pos).sqrMagnitude;
                    if (sqr < bestSqr)
                    {
                        bestSqr = sqr;
                        nearest = v;
                        graph = obj.Graph;
                    }
                }
            }

            return nearest;
        }

        public PhysicsEdge FindNearestEdge(Vector2 pos, float maxDist, out PhysicsGraph graph)
        {
            PhysicsEdge nearest = null;
            graph = null;
            float bestDist = maxDist;

            foreach (var obj in objects)
            {
                if (obj.Graph == null) continue;

                foreach (var edge in obj.Graph.Edges)
                {
                    var a = obj.Graph.GetVertex(edge.A);
                    var b = obj.Graph.GetVertex(edge.B);
                    if (a == null || b == null) continue;

                    float d = Geometry.DistanceToSegment(pos, a.Position, b.Position);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        nearest = edge;
                        graph = obj.Graph;
                    }
                }
            }

            return nearest;
        }
    }
}
