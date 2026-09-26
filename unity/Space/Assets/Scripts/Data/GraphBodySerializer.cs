using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class GraphBodyData
    {
        public float PosX;
        public float PosY;
        public float Angle;
        public float VelX;
        public float VelY;
        public float AngularVel;
        public float Mass;
        public float Inertia;
        public int NextVertexId;
        public List<VertexData> Vertices = new();
        public List<EdgeData> Edges = new();
    }

    [Serializable]
    public class VertexData
    {
        public int Id;
        public float X;
        public float Y;
    }

    [Serializable]
    public class EdgeData
    {
        public int A;
        public int B;
        public EdgeType Type;
    }

    public static class GraphBodySerializer
    {
        public static void Save(GraphBody body, string path)
        {
            var data = new GraphBodyData
            {
                PosX = body.Position.x,
                PosY = body.Position.y,
                Angle = body.Angle,
                VelX = body.Velocity.x,
                VelY = body.Velocity.y,
                AngularVel = body.AngularVel,
                Mass = body.Mass,
                Inertia = body.Inertia,
                NextVertexId = body.Graph.NextVertexId,
            };

            foreach (var v in body.Graph.Vertices)
            {
                data.Vertices.Add(new VertexData { Id = v.Id, X = v.Position.x, Y = v.Position.y });
            }

            foreach (var e in body.Graph.Edges)
            {
                data.Edges.Add(new EdgeData { A = e.A, B = e.B, Type = e.Type });
            }

            var text = JsonUtility.ToJson(data);

            Debug.Log(text);

            File.WriteAllText(path, text);
        }

        public static GraphBody Load(string path)
        {
            string text;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (FileNotFoundException ex)
            {
                Debug.LogWarning(ex);
                return new();
            }

            Debug.Log(text);

            var data = JsonUtility.FromJson<GraphBodyData>(text);

            var body = new GraphBody
            {
                Position = new Vector2(data.PosX, data.PosY),
                Angle = data.Angle,
                Velocity = new Vector2(data.VelX, data.VelY),
                AngularVel = data.AngularVel,
                Mass = data.Mass,
                Inertia = data.Inertia,
            };

            body.Graph.NextVertexId = data.NextVertexId;

            foreach (var vd in data.Vertices)
            {
                body.Graph.Vertices.Add(new GraphVertex { Id = vd.Id, Position = new Vector2(vd.X, vd.Y) });
            }

            foreach (var ed in data.Edges)
            {
                body.Graph.Edges.Add(new GraphEdge { A = ed.A, B = ed.B, Type = ed.Type });
            }

            return body;
        }
    }
}
