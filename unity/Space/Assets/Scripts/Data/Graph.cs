using System.Collections.Generic;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public enum EdgeType { Hull, Internal, Brace }

    public class GraphVertex
    {
        public int Id;
        public Vector2 Position;
    }

    public class GraphEdge
    {
        public int A;
        public int B;
        public EdgeType Type;
    }

    public class Graph
    {
        public List<GraphVertex> Vertices = new();
        public List<GraphEdge> Edges = new();
        public int NextVertexId;

        public GraphVertex AddVertex(Vector2 pos)
        {
            var v = new GraphVertex
            {
                Id = NextVertexId++,
                Position = pos
            };

            Vertices.Add(v);
            return v;
        }

        public void RemoveVertex(GraphVertex vertex)
        {
            if (vertex == null) return;
            Edges.RemoveAll(e => e.A == vertex.Id || e.B == vertex.Id);
            Vertices.Remove(vertex);
        }

        public bool AddEdge(GraphVertex a, GraphVertex b)
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

            Edges.Add(new GraphEdge
            {
                A = a.Id,
                B = b.Id,
                Type = EdgeType.Hull
            });

            return true;
        }

        public void RemoveEdge(GraphEdge edge)
        {
            Edges.Remove(edge);
        }

        public GraphVertex GetVertex(int id)
        {
            foreach (var v in Vertices)
            {
                if (v.Id == id) return v;
            }
            return null;
        }

        public Vector2 LocalCentroid()
        {
            if (Vertices.Count == 0) return Vector2.zero;
            var sum = Vector2.zero;
            foreach (var v in Vertices) sum += v.Position;
            return sum / Vertices.Count;
        }

        public GraphVertex FindVertex(Vector2 pos, float maxDist)
        {
            GraphVertex nearest = null;
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

        public GraphEdge FindEdge(Vector2 pos, float maxDist)
        {
            GraphEdge nearest = null;
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
