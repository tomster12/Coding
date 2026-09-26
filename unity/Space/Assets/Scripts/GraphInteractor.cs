using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class GraphInteractor : MonoBehaviour
    {
        public GraphVertex SelectedVertex => selectedVertex;

        [Header("References")]
        [SerializeField] Camera cam;
        [SerializeField] WorldObject target;

        [Header("Config")]
        [SerializeField] float vertexPickRadius = 0.5f;
        [SerializeField] float edgePickRadius = 0.2f;

        InputAction inputLmb;
        InputAction inputRmb;
        InputAction inputMmb;
        InputAction inputDelete;
        InputAction inputCancel;
        InputAction inputMousePos;

        private GraphVertex selectedVertex;
        private GraphVertex draggingVertex;
        GraphBody TargetBody => target.Body;
        Graph TargetGraph => TargetBody.Graph;
        private bool CanInteract =>
            cam != null &&
            target != null &&
            target.Body != null &&
            target.Body.Graph != null;

        private void Awake()
        {
            BuildInputs();
        }

        void BuildInputs()
        {
            var map = new InputActionMap("GraphEditor");

            inputMousePos = map.AddAction("MousePosition", InputActionType.Value, "<Mouse>/position");
            inputLmb = map.AddAction("LeftClick", InputActionType.Button, "<Mouse>/leftButton");
            inputRmb = map.AddAction("RightClick", InputActionType.Button, "<Mouse>/rightButton");
            inputMmb = map.AddAction("MiddleClick", InputActionType.Button, "<Mouse>/middleButton");

            inputDelete = map.AddAction("Delete", InputActionType.Button);
            inputDelete.AddBinding("<Keyboard>/delete");
            inputDelete.AddBinding("<Keyboard>/backspace");

            inputCancel = map.AddAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
        }

        void OnEnable()
        {
            inputLmb.Enable(); inputRmb.Enable();
            inputMmb.Enable(); inputDelete.Enable();
            inputCancel.Enable(); inputMousePos.Enable();

            inputLmb.started += OnLmbPress;
            inputLmb.canceled += OnLmbRelease;
            inputRmb.started += OnRmbPress;
            inputMmb.started += OnMmbPress;
            inputDelete.started += OnDeletePress;
            inputCancel.started += OnCancelPress;
        }

        void OnDisable()
        {
            inputLmb.started -= OnLmbPress;
            inputLmb.canceled -= OnLmbRelease;
            inputRmb.started -= OnRmbPress;
            inputMmb.started -= OnMmbPress;
            inputDelete.started -= OnDeletePress;
            inputCancel.started -= OnCancelPress;

            inputLmb.Disable(); inputRmb.Disable();
            inputMmb.Disable(); inputDelete.Disable();
            inputCancel.Disable(); inputMousePos.Disable();
        }

        void Update()
        {
            if (!CanInteract) return;

            if (draggingVertex != null)
            {
                draggingVertex.Position = GetMouseLocalToTarget();
            }
        }

        void OnLmbPress(InputAction.CallbackContext _)
        {
            if (!CanInteract) return;

            var mouseLocal = GetMouseLocalToTarget();
            draggingVertex = TargetGraph.FindVertex(mouseLocal, vertexPickRadius);
            if (draggingVertex == null) TargetGraph.AddVertex(mouseLocal);
        }

        void OnLmbRelease(InputAction.CallbackContext _)
        {
            draggingVertex = null;
        }

        void OnRmbPress(InputAction.CallbackContext _)
        {
            if (!CanInteract) return;

            var mouseLocal = GetMouseLocalToTarget();
            var clicked = TargetGraph.FindVertex(mouseLocal, vertexPickRadius);

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

            TargetGraph.AddEdge(selectedVertex, clicked);
            selectedVertex = null;
        }

        void OnMmbPress(InputAction.CallbackContext _)
        {
            if (!CanInteract) return;

            var mouseLocal = GetMouseLocalToTarget();
            var edge = TargetGraph.FindEdge(mouseLocal, edgePickRadius);
            if (edge == null) return;

            int count = System.Enum.GetValues(typeof(EdgeType)).Length;
            edge.Type = (EdgeType)(((int)edge.Type + 1) % count);
        }

        void OnDeletePress(InputAction.CallbackContext _)
        {
            if (!CanInteract) return;

            var mouseLocal = GetMouseLocalToTarget();
            var vertex = TargetGraph.FindVertex(mouseLocal, vertexPickRadius);

            if (vertex != null)
            {
                if (selectedVertex == vertex) selectedVertex = null;
                if (draggingVertex == vertex) draggingVertex = null;
                TargetGraph.RemoveVertex(vertex);
                return;
            }

            var edge = TargetGraph.FindEdge(mouseLocal, edgePickRadius);
            if (edge != null) TargetGraph.RemoveEdge(edge);
        }

        void OnCancelPress(InputAction.CallbackContext _)
        {
            selectedVertex = null;
        }

        Vector2 GetMouseLocalToTarget()
        {
            Vector2 screen = inputMousePos.ReadValue<Vector2>();
            Vector2 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
            return TargetBody.LocalPosition(world);
        }
    }
}
