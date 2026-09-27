using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Client
{
    [ExecuteAlways]
    public class PhysicsGraphRenderer : MonoBehaviour
    {
        static readonly Color ColourHull = new(0.20f, 0.90f, 0.30f);
        static readonly Color ColourInternal = new(0.95f, 0.85f, 0.10f);
        static readonly Color ColourBrace = new(0.20f, 0.80f, 0.95f);
        static readonly Color ColourVertex = new(0.8f, 0.8f, 0.8f);
        static readonly Color ColourSelected = new(1f, 0.55f, 0f);

        [Header("References")]
        [SerializeField] WorldInteractor interactor;
        [SerializeField] PhysicsWorld world;

        [Header("Config")]
        [SerializeField] float vertexRadius = 0.15f;

        void OnDrawGizmos()
        {
            if (world == null) return;

            foreach (var obj in world.Objects)
            {
                if (obj.Graph != null) DrawGraph(obj.Graph);
            }
        }

        void DrawGraph(PhysicsGraph graph)
        {
            foreach (var edge in graph.Edges)
            {
                var a = graph.GetVertex(edge.A);
                var b = graph.GetVertex(edge.B);
                if (a == null || b == null) continue;

                Gizmos.color = EdgeColour(edge.Type);
                Gizmos.DrawLine(a.Position, b.Position);
            }

            foreach (var vertex in graph.Vertices)
            {
                Gizmos.color = interactor != null && vertex == interactor.SelectedVertex
                    ? ColourSelected
                    : ColourVertex;

                Gizmos.DrawSphere(vertex.Position, vertexRadius);
            }
        }

        static Color EdgeColour(PhysicsEdgeType type) => type switch
        {
            PhysicsEdgeType.Hull => ColourHull,
            PhysicsEdgeType.Internal => ColourInternal,
            PhysicsEdgeType.Brace => ColourBrace,
            _ => Color.white
        };
    }
}
