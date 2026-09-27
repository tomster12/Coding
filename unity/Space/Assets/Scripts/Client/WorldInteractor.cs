using Assets.Scripts.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Client
{
    public enum InteractMode { Edit, Physics }

    public class WorldInteractor : MonoBehaviour
    {
        public InteractMode InteractMode => interactMode;
        public PhysicsGraph SelectedGraph => selectedGraph;
        public PhysicsVertex SelectedVertex => selectedVertex;

        [Header("References")]
        [SerializeField] Camera cam;
        [SerializeField] PhysicsWorld world;
        [SerializeField] PhysicsSimulator simulator;

        [Header("Config")]
        [SerializeField] private InteractMode interactMode = InteractMode.Edit;
        [SerializeField] private float vertexPickRadius = 0.5f;
        [SerializeField] private float edgePickRadius = 0.2f;
        [SerializeField] private float dragStartThreshold = 0.05f;

        private PhysicsGraph selectedGraph;
        private PhysicsVertex selectedVertex;
        private PhysicsVertex draggingVertex;
        private Vector2 dragStartMouseWorld;
        private Vector2 dragStartOffset;
        private bool isDragActive;

        private bool CanEdit => cam != null && world != null;

        private void Awake()
        {
            BuildInputs();
        }

        private void OnEnable()
        {
            RegisterInputs();
            ApplyInteractionMode();
        }

        private void OnDisable()
        {
            UnregisterInputs();
        }

        private void Update()
        {
            if (draggingVertex == null) return;

            Vector2 mouseWorld = GetMouseWorld();

            if (!isDragActive)
            {
                if ((mouseWorld - dragStartMouseWorld).sqrMagnitude < dragStartThreshold * dragStartThreshold) return;
                isDragActive = true;
            }

            Vector2 targetPosition = mouseWorld + dragStartOffset;

            if (interactMode == InteractMode.Edit)
            {
                // Move without velocity
                draggingVertex.Position = targetPosition;
                draggingVertex.OldPosition = targetPosition;
            }
            else
            {
                // Move with velocity (leave OldPosition stale)
                draggingVertex.Position = targetPosition;
            }

            UpdateConnectedEdgeRestLengths(draggingVertex, selectedGraph);
        }

        private static void UpdateConnectedEdgeRestLengths(PhysicsVertex vertex, PhysicsGraph graph)
        {
            if (vertex == null || graph == null) return;

            foreach (var edge in graph.Edges)
            {
                if (edge.A != vertex.Id && edge.B != vertex.Id) continue;

                var a = graph.GetVertex(edge.A);
                var b = graph.GetVertex(edge.B);
                if (a == null || b == null) continue;

                edge.RestLength = Vector2.Distance(a.Position, b.Position);
            }
        }

        private void ApplyInteractionMode()
        {
            selectedGraph = null;
            selectedVertex = null;
            draggingVertex = null;

            if (simulator != null)
            {
                simulator.enabled = interactMode == InteractMode.Physics;
            }
        }

        #region Input

        private InputAction inputLmb;
        private InputAction inputRmb;
        private InputAction inputMmb;
        private InputAction inputDelete;
        private InputAction inputCancel;
        private InputAction inputToggleMode;
        InputAction inputMousePos;

        private void BuildInputs()
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

        private void RegisterInputs()
        {

            inputLmb.Enable();
            inputRmb.Enable();
            inputMmb.Enable();
            inputDelete.Enable();
            inputCancel.Enable();
            inputMousePos.Enable();
            inputToggleMode.Enable();

            inputLmb.started += OnInteract1Pressed;
            inputLmb.canceled += OnInteract1Released;
            inputRmb.started += OnInteract2Pressed;
            inputMmb.started += OnInteract3Pressed;
            inputDelete.started += OnDeletePress;
            inputCancel.started += OnCancelPress;
            inputToggleMode.started += OnToggleModePress;
        }

        private void UnregisterInputs()
        {
            inputLmb.started -= OnInteract1Pressed;
            inputLmb.canceled -= OnInteract1Released;
            inputRmb.started -= OnInteract2Pressed;
            inputMmb.started -= OnInteract3Pressed;
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

        private Vector2 GetMouseWorld()
        {
            Vector2 screen = inputMousePos.ReadValue<Vector2>();
            return cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
        }

        private void OnToggleModePress(InputAction.CallbackContext ctx)
        {
            interactMode = interactMode == InteractMode.Edit
                ? InteractMode.Physics
                : InteractMode.Edit;

            ApplyInteractionMode();
        }

        private void OnInteract1Pressed(InputAction.CallbackContext ctx)
        {
            if (!CanEdit) return;

            Vector2 mouseWorld = GetMouseWorld();

            // EDIT: Select and start dragging, otherwise deselect the graph
            if (interactMode == InteractMode.Edit)
            {
                var clickedVertex = world.FindNearestVertex(mouseWorld, vertexPickRadius, out var vertexGraph);
                if (clickedVertex != null)
                {
                    selectedGraph = vertexGraph;
                    StartDrag(clickedVertex, mouseWorld);
                }
                else
                {
                    selectedGraph = null;
                    selectedVertex = null;
                }
            }

            // PHYSICS: Select and start dragging
            else
            {
                var clickedVertex = world.FindNearestVertex(mouseWorld, vertexPickRadius, out var vertexGraph);
                selectedGraph = vertexGraph;
                StartDrag(clickedVertex, mouseWorld);
            }
        }

        private void StartDrag(PhysicsVertex vertex, Vector2 mouseWorld)
        {
            draggingVertex = vertex;
            isDragActive = false;

            if (vertex == null) return;
            dragStartMouseWorld = mouseWorld;
            dragStartOffset = vertex.Position - mouseWorld;
        }

        private void OnInteract1Released(InputAction.CallbackContext ctx)
        {
            // Always stop dragging
            draggingVertex = null;
            isDragActive = false;
        }

        private void OnInteract2Pressed(InputAction.CallbackContext ctx)
        {
            if (interactMode != InteractMode.Edit || !CanEdit) return;

            Vector2 mouseWorld = GetMouseWorld();

            // Look within the selected graph if we have one, otherwise search the whole world
            PhysicsVertex clickedVertex;
            if (selectedGraph != null)
            {
                clickedVertex = selectedGraph.FindVertex(mouseWorld, vertexPickRadius);
            }
            else
            {
                clickedVertex = world.FindNearestVertex(mouseWorld, vertexPickRadius, out var vertexGraph);
                selectedGraph = vertexGraph;
            }

            // Clicked empty space: create a new node (needs a graph to add it to), connecting it to the selected vertex if any
            if (clickedVertex == null)
            {
                if (selectedGraph == null) return;

                var newVertex = selectedGraph.AddVertex(mouseWorld);
                if (selectedVertex != null) selectedGraph.AddEdge(selectedVertex, newVertex);
                selectedVertex = newVertex;
                return;
            }

            // Select new vertex
            if (selectedVertex == null)
            {
                selectedVertex = clickedVertex;
                return;
            }

            // Deselect vertex
            if (clickedVertex == selectedVertex)
            {
                selectedVertex = null;
                return;
            }

            // Connection to new selected vertex
            selectedGraph.AddEdge(selectedVertex, clickedVertex);
            selectedVertex = clickedVertex;
        }

        private void OnInteract3Pressed(InputAction.CallbackContext ctx)
        {
            if (interactMode != InteractMode.Edit || !CanEdit) return;

            // Cycle through EdgeType enum
            // Search within the selected graph if we have one, otherwise the whole world
            Vector2 mouseWorld = GetMouseWorld();

            PhysicsEdge selectedEdge;
            if (selectedGraph != null)
            {
                selectedEdge = selectedGraph.FindEdge(mouseWorld, edgePickRadius);
            }
            else
            {
                selectedEdge = world.FindNearestEdge(mouseWorld, edgePickRadius, out var edgeGraph);
                selectedGraph = edgeGraph;
            }

            if (selectedEdge == null) return;

            int maxEnum = System.Enum.GetValues(typeof(PhysicsEdgeType)).Length;
            selectedEdge.Type = (PhysicsEdgeType)(((int)selectedEdge.Type + 1) % maxEnum);
        }

        private void OnDeletePress(InputAction.CallbackContext ctx)
        {
            if (interactMode != InteractMode.Edit || !CanEdit || selectedGraph == null) return;

            Vector2 mouseWorld = GetMouseWorld();
            var vertex = selectedGraph.FindVertex(mouseWorld, vertexPickRadius);

            // Delete vertex
            if (vertex != null)
            {
                if (selectedVertex == vertex) selectedVertex = null;
                if (draggingVertex == vertex) draggingVertex = null;
                selectedGraph.RemoveVertex(vertex);
                return;
            }

            // Delete edge
            var edge = selectedGraph.FindEdge(mouseWorld, edgePickRadius);
            if (edge != null) selectedGraph.RemoveEdge(edge);
        }

        private void OnCancelPress(InputAction.CallbackContext ctx)
        {
            selectedVertex = null;
        }

        #endregion
    }
}
