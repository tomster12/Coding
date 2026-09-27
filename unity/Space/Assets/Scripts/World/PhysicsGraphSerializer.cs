using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.World
{
    [Serializable]
    public class PhysicsGraphData
    {
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
        public float Mass;
        public bool Pinned;
    }

    [Serializable]
    public class EdgeData
    {
        public int A;
        public int B;
        public EdgeType Type;
        public float RestLength;
        public float Stiffness;
        public float YieldStrain;
        public float BreakStrain;
        public float PlasticRate;
    }

    public static class PhysicsGraphSerializer
    {
        static string ResolvePath(string objectName) =>
            Path.Combine(Application.persistentDataPath, "Graphs", objectName + ".json");

        public static void Save(PhysicsGraph graph, string objectName)
        {
            if (graph == null || string.IsNullOrWhiteSpace(objectName))
            {
                Debug.LogWarning($"PhysicsGraphSerializer: skipping save, missing graph or object name '{objectName}'.");
                return;
            }

            var data = new PhysicsGraphData { NextVertexId = graph.NextVertexId };

            foreach (var v in graph.Vertices)
            {
                data.Vertices.Add(new VertexData
                {
                    Id = v.Id,
                    X = v.Position.x,
                    Y = v.Position.y,
                    Mass = v.Mass,
                    Pinned = v.Pinned,
                });
            }

            foreach (var e in graph.Edges)
            {
                data.Edges.Add(new EdgeData
                {
                    A = e.A,
                    B = e.B,
                    Type = e.Type,
                    RestLength = e.RestLength,
                    Stiffness = e.Stiffness,
                    YieldStrain = e.YieldStrain,
                    BreakStrain = e.BreakStrain,
                    PlasticRate = e.PlasticRate,
                });
            }

            string path = ResolvePath(objectName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, JsonUtility.ToJson(data));
            }
            catch (Exception ex)
            {
                Debug.LogError($"PhysicsGraphSerializer: failed to save '{objectName}' to '{path}': {ex}");
            }
        }

        public static PhysicsGraph Load(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName)) return new PhysicsGraph();

            string path = ResolvePath(objectName);
            if (!File.Exists(path)) return new PhysicsGraph();

            PhysicsGraphData data;

            try
            {
                string text = File.ReadAllText(path);
                data = JsonUtility.FromJson<PhysicsGraphData>(text);
            }
            catch (Exception ex)
            {
                Debug.LogError($"PhysicsGraphSerializer: failed to load '{objectName}' from '{path}', starting empty: {ex}");
                return new PhysicsGraph();
            }

            if (data == null) return new PhysicsGraph();

            var graph = new PhysicsGraph { NextVertexId = data.NextVertexId };

            foreach (var vd in data.Vertices)
            {
                graph.Vertices.Add(new PhysicsVertex
                {
                    Id = vd.Id,
                    Position = new Vector2(vd.X, vd.Y),
                    OldPosition = new Vector2(vd.X, vd.Y),
                    Mass = vd.Mass,
                    Pinned = vd.Pinned,
                });
            }

            foreach (var ed in data.Edges)
            {
                graph.Edges.Add(new PhysicsEdge
                {
                    A = ed.A,
                    B = ed.B,
                    Type = ed.Type,
                    RestLength = ed.RestLength,
                    Stiffness = ed.Stiffness,
                    YieldStrain = ed.YieldStrain,
                    BreakStrain = ed.BreakStrain,
                    PlasticRate = ed.PlasticRate,
                });
            }

            return graph;
        }
    }
}
