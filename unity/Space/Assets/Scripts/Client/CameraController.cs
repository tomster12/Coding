using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Client
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Camera cam;

        [Header("Config")]
        [SerializeField] float zoomSpeed = 1f;
        [SerializeField] float minZoom = 1f;
        [SerializeField] float maxZoom = 50f;

        private bool isDragging;
        private Vector2 dragGrabWorldPoint;

        private void Awake()
        {
            BuildInputs();
        }

        private void OnEnable()
        {
            RegisterInputs();
        }

        private void OnDisable()
        {
            UnregisterInputs();
        }

        private void Update()
        {
            if (!isDragging || cam == null) return;

            // Keep the world point grabbed at drag start glued under the cursor
            Vector2 mouseWorld = GetMouseWorld();
            cam.transform.position += (Vector3)(dragGrabWorldPoint - mouseWorld);
        }

        #region Input

        private InputAction inputMmb;
        private InputAction inputScroll;
        private InputAction inputMousePos;

        private void BuildInputs()
        {
            var map = new InputActionMap("CameraController");

            inputMousePos = map.AddAction("MousePosition", InputActionType.Value, "<Mouse>/position");
            inputMmb = map.AddAction("MiddleClick", InputActionType.Button, "<Mouse>/middleButton");
            inputScroll = map.AddAction("Scroll", InputActionType.Value, "<Mouse>/scroll");
        }

        private void RegisterInputs()
        {
            inputMousePos.Enable();
            inputMmb.Enable();
            inputScroll.Enable();

            inputMmb.started += OnMmbPress;
            inputMmb.canceled += OnMmbRelease;
            inputScroll.performed += OnScroll;
        }

        private void UnregisterInputs()
        {
            inputMmb.started -= OnMmbPress;
            inputMmb.canceled -= OnMmbRelease;
            inputScroll.performed -= OnScroll;

            inputMousePos.Disable();
            inputMmb.Disable();
            inputScroll.Disable();
        }

        private Vector2 GetMouseWorld()
        {
            Vector2 screen = inputMousePos.ReadValue<Vector2>();
            return cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
        }

        private void OnMmbPress(InputAction.CallbackContext ctx)
        {
            if (cam == null) return;

            isDragging = true;
            dragGrabWorldPoint = GetMouseWorld();
        }

        private void OnMmbRelease(InputAction.CallbackContext ctx)
        {
            isDragging = false;
        }

        private void OnScroll(InputAction.CallbackContext ctx)
        {
            if (cam == null) return;

            float scroll = ctx.ReadValue<Vector2>().y;
            if (Mathf.Approximately(scroll, 0f)) return;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * 0.01f, minZoom, maxZoom);
        }

        #endregion
    }
}
