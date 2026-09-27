using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public class WorldHud : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Camera cam;
        [SerializeField] WorldInteractor interactor;
        [SerializeField] PhysicsWorld world;

        [Header("Config")]
        [SerializeField] float nameLabelPadding = 1f;
        [SerializeField] bool showVertexIds = true;
        [SerializeField] bool showEdgeTypes = true;

        GUIStyle textStyle;
        GUIStyle nameStyle;

        void OnGUI()
        {
            textStyle ??= new GUIStyle(GUI.skin.label) { normal = { textColor = Color.white } };
            nameStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };

            DrawInteractionModePanel();
            DrawObjectNames();
            DrawSelectedGraphDetails();
        }

        void DrawInteractionModePanel()
        {
            if (interactor == null) return;

            string controls = interactor.InteractMode == InteractMode.Edit
                ? "LMB: select vertex and drag to move it, or click empty space to deselect graph\nRMB: select/create vertex, click empty space to add a node (and edge if one is selected)\nMMB: cycle edge type\nDelete: remove vertex/edge"
                : "LMB: grab and throw the nearest body";

            GUILayout.BeginArea(new Rect(10, 10, 500, 100));
            GUILayout.Label($"Mode: {interactor.InteractMode}  (Tab to switch)", textStyle);
            GUILayout.Label(controls, textStyle);
            GUILayout.EndArea();
        }

        void DrawObjectNames()
        {
            if (world == null || cam == null) return;

            foreach (var obj in world.Objects)
            {
                if (obj.Graph == null || obj.Graph.Vertices.Count == 0) continue;

                float maxY = float.NegativeInfinity;
                float sumX = 0f;

                foreach (var v in obj.Graph.Vertices)
                {
                    if (v.Position.y > maxY) maxY = v.Position.y;
                    sumX += v.Position.x;
                }

                Vector2 labelPos = new(sumX / obj.Graph.Vertices.Count, maxY + nameLabelPadding);
                DrawWorldLabel(labelPos, obj.ObjectName, nameStyle, 140, 20);
            }
        }

        void DrawSelectedGraphDetails()
        {
            if (cam == null || interactor == null || interactor.SelectedGraph == null) return;

            var graph = interactor.SelectedGraph;

            if (showVertexIds)
            {
                foreach (var v in graph.Vertices)
                {
                    DrawWorldLabel(v.Position, v.Id.ToString(), textStyle, 40, 20);
                }
            }

            if (showEdgeTypes)
            {
                foreach (var edge in graph.Edges)
                {
                    var a = graph.GetVertex(edge.A);
                    var b = graph.GetVertex(edge.B);
                    if (a == null || b == null) continue;

                    Vector2 mid = (a.Position + b.Position) * 0.5f;
                    float length = Vector2.Distance(a.Position, b.Position);
                    DrawWorldLabel(mid, $"{edge.Type} ({length:F2})", textStyle, 100, 20);
                }
            }
        }

        void DrawWorldLabel(Vector2 worldPos, string text, GUIStyle style, float width, float height)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            if (screenPos.z < 0f) return;

            float guiY = Screen.height - screenPos.y;
            GUI.Label(new Rect(screenPos.x - width / 2f, guiY - height / 2f, width, height), text, style);
        }
    }
}
