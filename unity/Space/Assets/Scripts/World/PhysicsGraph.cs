using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class PhysicsGraph
    {
        public List<PhysicsVertex> Vertices = new();
        public List<PhysicsEdge> Edges = new();
        public int NextVertexId;

        public PhysicsVertex AddVertex(Vector2 position)
        {
            var vertex = new PhysicsVertex
            {
                Id = NextVertexId++,
                Position = position,
                OldPosition = position,
            };

            Vertices.Add(vertex);
            return vertex;
        }

        public void RemoveVertex(PhysicsVertex vertex)
        {
            if (vertex == null) return;
            Edges.RemoveAll(e => e.A == vertex.Id || e.B == vertex.Id);
            Vertices.Remove(vertex);
        }

        public bool AddEdge(PhysicsVertex a, PhysicsVertex b)
        {
            if (a == null || b == null || a == b) return false;

            foreach (var edge in Edges)
            {
                if ((edge.A == a.Id && edge.B == b.Id) ||
                    (edge.A == b.Id && edge.B == a.Id))
                {
                    return false;
                }
            }

            // Rest length is captured from the shape as drawn/built.
            Edges.Add(new PhysicsEdge
            {
                A = a.Id,
                B = b.Id,
                Type = EdgeType.Hull,
                RestLength = Vector2.Distance(a.Position, b.Position),
            });

            return true;
        }

        public void RemoveEdge(PhysicsEdge edge)
        {
            Edges.Remove(edge);
        }

        public PhysicsVertex GetVertex(int id)
        {
            foreach (var v in Vertices)
            {
                if (v.Id == id) return v;
            }
            return null;
        }

        public Vector2 Centroid()
        {
            if (Vertices.Count == 0) return Vector2.zero;
            var sum = Vector2.zero;
            foreach (var v in Vertices) sum += v.Position;
            return sum / Vertices.Count;
        }

        public PhysicsVertex FindVertex(Vector2 pos, float maxDist)
        {
            PhysicsVertex nearest = null;
            float bestSqr = maxDist * maxDist;

            foreach (var v in Vertices)
            {
                float sqr = (v.Position - pos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    nearest = v;
                }
            }

            return nearest;
        }

        public PhysicsEdge FindEdge(Vector2 pos, float maxDist)
        {
            PhysicsEdge nearest = null;
            float bestDist = maxDist;

            foreach (var edge in Edges)
            {
                var a = GetVertex(edge.A);
                var b = GetVertex(edge.B);

                if (a == null || b == null) continue;

                float d = Geometry.DistanceToSegment(pos, a.Position, b.Position);
                if (d < bestDist)
                {
                    bestDist = d;
                    nearest = edge;
                }
            }

            return nearest;
        }
    }
}
