using System.Collections.Generic;
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
    }
}
