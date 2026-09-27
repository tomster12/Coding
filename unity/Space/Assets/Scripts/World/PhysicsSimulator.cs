using UnityEngine;

namespace Assets.Scripts.World
{
    public class PhysicsSimulator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] PhysicsWorld world;

        [Header("Config")]
        [SerializeField] Vector2 gravity = Vector2.zero;
        [SerializeField] float damping = 0.01f;
        [SerializeField] int constraintIterations = 8;

        void FixedUpdate()
        {
            if (world == null) return;

            float dt = Time.fixedDeltaTime;
            var objects = world.Objects;

            foreach (var obj in objects)
            {
                if (obj.Graph != null) Integrate(obj.Graph, dt);
            }

            for (int i = 0; i < constraintIterations; i++)
            {
                foreach (var obj in objects)
                {
                    if (obj.Graph != null) SolveConstraints(obj.Graph);
                }
            }

            foreach (var obj in objects)
            {
                if (obj.Graph != null) ApplyPlasticity(obj.Graph);
            }
        }

        void Integrate(PhysicsGraph graph, float dt)
        {
            foreach (var v in graph.Vertices)
            {
                if (v.Pinned || v.InverseMass <= 0f) continue;

                Vector2 velocity = (v.Position - v.OldPosition) * (1f - damping);
                Vector2 newPosition = v.Position + velocity + gravity * (dt * dt);

                v.OldPosition = v.Position;
                v.Position = newPosition;
            }
        }

        void SolveConstraints(PhysicsGraph graph)
        {
            foreach (var edge in graph.Edges)
            {
                var a = graph.GetVertex(edge.A);
                var b = graph.GetVertex(edge.B);
                if (a == null || b == null) continue;

                float invMassSum = a.InverseMass + b.InverseMass;
                if (invMassSum <= 0f) continue;

                Vector2 delta = b.Position - a.Position;
                float distance = delta.magnitude;
                if (distance < Mathf.Epsilon) continue;

                float diff = (distance - edge.RestLength) / distance;
                Vector2 correction = delta * (diff * edge.Stiffness / invMassSum);

                a.Position += correction * a.InverseMass;
                b.Position -= correction * b.InverseMass;
            }
        }

        void ApplyPlasticity(PhysicsGraph graph)
        {
            graph.Edges.RemoveAll(edge =>
            {
                var a = graph.GetVertex(edge.A);
                var b = graph.GetVertex(edge.B);
                if (a == null || b == null) return true;

                float distance = Vector2.Distance(a.Position, b.Position);
                float strain = (distance - edge.RestLength) / edge.RestLength;

                if (Mathf.Abs(strain) >= edge.BreakStrain)
                    return true; // Structural failure: the edge snaps.

                if (Mathf.Abs(strain) >= edge.YieldStrain)
                {
                    // Permanently bend: rest length creeps toward the current
                    // length instead of springing all the way back.
                    edge.RestLength = Mathf.Lerp(edge.RestLength, distance, edge.PlasticRate);
                }

                return false;
            });
        }
    }
}
