using Assets.Scripts.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Client
{
    public enum InteractMode { Edit, Physics }

    public class WorldInteractor : MonoBehaviour
    {
        public PhysicsVertex SelectedVertex => selectedVertex;

        [Header("References")]
        [SerializeField] Camera cam;
        [SerializeField] PhysicsWorld world;
        [SerializeField] PhysicsSimulator simulator;

        [Header("Config")]
        [SerializeField] InteractMode mode = InteractMode.Edit;
        [SerializeField] float vertexPickRadius = 0.5f;
        [SerializeField] float edgePickRadius = 0.2f;

        InputAction inputLmb;
        InputAction inputRmb;
        InputAction inputMmb;
        InputAction inputDelete;
        InputAction inputCancel;
        InputAction inputToggleMode;
        InputAction inputMousePos;

        PhysicsGraph selectedGraph;
        PhysicsVertex selectedVertex;
        PhysicsVertex draggingVertex;

        bool CanEdit => cam != null && world != null;

        void Awake()
        {
            BuildInputs();
        }

        void BuildInputs()
        {
            var map = new InputActionMap("WorldInteractor");

            inputMousePos = map.AddAction("MousePosition", InputActionType.Value, "<Mouse>/position");
            inputLmb = map.AddAction("LeftClick", InputActionType.Button, "<Mouse>/leftButton");
            inputRmb = map.AddAction("RightClick", InputActionType.Button, "<Mouse>/rightButton");
            inputMmb = map.AddAction("MiddleClick", InputActionType.Button, "<Mouse>/middleButton");

            inputDelete = map.AddAction("Delete", InputActionType.Button);
            inputDelete.AddBinding("<Keyboard>/delete");
            inputDelete.AddBinding("<Keyboard>/backspace");

            inputCancel = map.AddAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
            inputToggleMode = map.AddAction("ToggleMode", InputActionType.Button, "<Keyboard>/tab");
        }

        void OnEnable()
        {
            inputLmb.Enable();
            inputRmb.Enable();
            inputMmb.Enable();
            inputDelete.Enable();
            inputCancel.Enable();
            inputMousePos.Enable();
            inputToggleMode.Enable();

            inputLmb.started += OnLmbPress;
            inputLmb.canceled += OnLmbRelease;
            inputRmb.started += OnRmbPress;
            inputMmb.started += OnMmbPress;
            inputDelete.started += OnDeletePress;
            inputCancel.started += OnCancelPress;
            inputToggleMode.started += OnToggleModePress;

            ApplyMode();
        }

        void OnDisable()
        {
            inputLmb.started -= OnLmbPress;
            inputLmb.canceled -= OnLmbRelease;
            inputRmb.started -= OnRmbPress;
            inputMmb.started -= OnMmbPress;
            inputDelete.started -= OnDeletePress;
            inputCancel.started -= OnCancelPress;
            inputToggleMode.started -= OnToggleModePress;

            inputLmb.Disable();
            inputRmb.Disable();
            inputMmb.Disable();
            inputDelete.Disable();
            inputCancel.Disable();
            inputMousePos.Disable();
            inputToggleMode.Disable();
        }

        void Update()
        {
            if (draggingVertex == null) return;

            Vector2 mouseWorld = GetMouseWorld();

            if (mode == InteractMode.Edit)
            {
                // Teleport cleanly - no velocity is carried into the simulation.
                draggingVertex.Position = mouseWorld;
                draggingVertex.OldPosition = mouseWorld;
            }
            else
            {
                // Leave OldPosition stale: on release, the vertex keeps the
                // velocity implied by the last frame's drag (a "throw").
                draggingVertex.Position = mouseWorld;
            }
        }

        void OnToggleModePress(InputAction.CallbackContext _)
        {
            mode = mode == InteractMode.Edit ? InteractMode.Physics : InteractMode.Edit;
            ApplyMode();
        }

        void ApplyMode()
        {
            selectedGraph = null;
            selectedVertex = null;
            draggingVertex = null;
            if (simulator != null) simulator.enabled = mode == InteractMode.Physics;
        }

        void OnLmbPress(InputAction.CallbackContext _)
        {
            if (!CanEdit) return;

            Vector2 mouseWorld = GetMouseWorld();

            if (mode == InteractMode.Edit)
            {
                // Clicking a vertex in any graph both selects its owner and starts dragging it.
                // Clicking empty space only creates a new vertex if a graph is already selected.
                var clicked = world.FindNearestVertex(mouseWorld, vertexPickRadius, out var clickedGraph);

                if (clicked != null)
                {
                    selectedGraph = clickedGraph;
                    draggingVertex = clicked;
                }
                else if (selectedGraph != null)
                {
                    draggingVertex = selectedGraph.AddVertex(mouseWorld);
                }
            }
            else
            {
                draggingVertex = world.FindNearestVertex(mouseWorld, vertexPickRadius, out _);
            }
        }

        void OnLmbRelease(InputAction.CallbackContext _)
        {
            draggingVertex = null;
        }

        void OnRmbPress(InputAction.CallbackContext _)
        {
            if (mode != InteractMode.Edit || !CanEdit || selectedGraph == null) return;

            Vector2 mouseWorld = GetMouseWorld();
            var clicked = selectedGraph.FindVertex(mouseWorld, vertexPickRadius);

            if (selectedVertex == null)
            {
                selectedVertex = clicked;
                return;
            }

            if (clicked == null || clicked == selectedVertex)
            {
                selectedVertex = null;
                return;
            }

            selectedGraph.AddEdge(selectedVertex, clicked);
            selectedVertex = null;
        }

        void OnMmbPress(InputAction.CallbackContext _)
        {
            if (mode != InteractMode.Edit || !CanEdit || selectedGraph == null) return;

            Vector2 mouseWorld = GetMouseWorld();
            var edge = selectedGraph.FindEdge(mouseWorld, edgePickRadius);
            if (edge == null) return;

            int count = System.Enum.GetValues(typeof(EdgeType)).Length;
            edge.Type = (EdgeType)(((int)edge.Type + 1) % count);
        }

        void OnDeletePress(InputAction.CallbackContext _)
        {
            if (mode != InteractMode.Edit || !CanEdit || selectedGraph == null) return;

            Vector2 mouseWorld = GetMouseWorld();
            var vertex = selectedGraph.FindVertex(mouseWorld, vertexPickRadius);

            if (vertex != null)
            {
                if (selectedVertex == vertex) selectedVertex = null;
                if (draggingVertex == vertex) draggingVertex = null;
                selectedGraph.RemoveVertex(vertex);
                return;
            }

            var edge = selectedGraph.FindEdge(mouseWorld, edgePickRadius);
            if (edge != null) selectedGraph.RemoveEdge(edge);
        }

        void OnCancelPress(InputAction.CallbackContext _)
        {
            selectedVertex = null;
        }

        Vector2 GetMouseWorld()
        {
            Vector2 screen = inputMousePos.ReadValue<Vector2>();
            return cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
        }
    }
}
