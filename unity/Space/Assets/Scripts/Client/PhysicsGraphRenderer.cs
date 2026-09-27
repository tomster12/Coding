using Assets.Scripts.World;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Client
{
    [ExecuteAlways]
    public class PhysicsGraphRenderer : MonoBehaviour
    {
        static readonly Color ColourHull = new(0.20f, 0.90f, 0.30f);
        static readonly Color ColourInternal = new(0.95f, 0.85f, 0.10f);
        static readonly Color ColourBrace = new(0.20f, 0.80f, 0.95f);
        static readonly Color ColourVertex = Color.white;
        static readonly Color ColourSelected = new(1f, 0.55f, 0f);

        [Header("References")]
        [SerializeField] WorldInteractor interactor;
        [SerializeField] WorldObject target;

        [Header("Config")]
        [SerializeField] float vertexRadius = 0.15f;
        [SerializeField] bool showVertexIds = true;
        [SerializeField] bool showEdgeTypes = false;

        PhysicsGraph TargetGraph => target.Graph;
        private bool CanRender =>
            interactor != null &&
            target != null &&
            target.Graph != null;

        void OnDrawGizmos()
        {
            if (!CanRender) return;

            foreach (var edge in TargetGraph.Edges)
            {
                var a = TargetGraph.GetVertex(edge.A);
                var b = TargetGraph.GetVertex(edge.B);
                if (a == null || b == null) continue;

                Gizmos.color = EdgeColour(edge.Type);
                Gizmos.DrawLine(a.Position, b.Position);

                if (showEdgeTypes)
                {
                    Vector2 mid = (a.Position + b.Position) * 0.5f;
                    Handles.Label(mid + Vector2.up * 0.15f, edge.Type.ToString());
                }
            }

            foreach (var vertex in TargetGraph.Vertices)
            {
                Vector2 wp = vertex.Position;

                Handles.color = vertex == interactor.SelectedVertex ? ColourSelected : ColourVertex;
                Handles.DrawSolidDisc(wp, Vector3.forward, vertexRadius);

                if (showVertexIds)
                {
                    Handles.Label(wp + Vector2.one * (vertexRadius + 0.05f), vertex.Id.ToString());
                }
            }
        }

        static Color EdgeColour(EdgeType type) => type switch
        {
            EdgeType.Hull => ColourHull,
            EdgeType.Internal => ColourInternal,
            EdgeType.Brace => ColourBrace,
            _ => Color.white
        };
    }
}
