using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 1f; // The speed at which the camera zooms
    [SerializeField] private float panSpeed = 10f; // The speed at which the camera pans
    [SerializeField] private float dragSpeed = 2f; // The speed at which the camera drags    
    [SerializeField] private float minZoom = 1f; // The minimum zoom level
    [SerializeField] private float maxZoom = 10f; // The maximum zoom level
    [SerializeField] private float minX = -10f; // The minimum X position of the camera
    [SerializeField] private float maxX = 10f; // The maximum X position of the camera
    [SerializeField] private float minY = -10f; // The minimum Y position of the camera
    [SerializeField] private float maxY = 10f; // The maximum Y position of the camera
    [SerializeField] private float moveSpeed = 10f; // The maximum Y position of the camera

    public bool preventDrag = false;

    private Camera cam; // The camera component
    private Mouse mouse;
    private Tilemap tilemap;

    private float zoomLevel = 5f; // The current zoom level
    private Vector3 dragOrigin; // The starting point of a drag gesture
    private bool isDragging = false;
    private Vector3 lookToPosition = Vector3.zero;

    private DiceManager diceManager;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        mouse = GameObject.Find("Mouse").GetComponent<Mouse>();
        diceManager = GameObject.Find("DiceManager").GetComponent<DiceManager>();
        tilemap = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
    }

    private void Update()
    {
        if (diceManager.IsDicing())
            return;

        // Zoom in and out with the scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomLevel -= scroll * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

        // Pan with the arrow keys or WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 pan = new Vector3(horizontal, vertical, 0f) * panSpeed * Time.deltaTime;
        transform.Translate(pan);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        isDragging = false;
        // Drag with the mouse
        if (!preventDrag)
        {
            if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
            {
                isDragging = true;
                dragOrigin = Input.mousePosition;
                Cursor.SetCursor(mouse.drag, new Vector2(mouse.drag.width/2, mouse.drag.height/2), CursorMode.ForceSoftware);
            }
            else if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
            {
                isDragging = true;
                Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                Vector3 move = new Vector3(difference.x * dragSpeed, difference.y * dragSpeed, 0);
                transform.Translate(move, Space.World);
                dragOrigin = Input.mousePosition;
                Cursor.SetCursor(mouse.drag, new Vector2(mouse.drag.width / 2, mouse.drag.height / 2), CursorMode.ForceSoftware);
            }
        }
        

        // Clamp the camera position to the bounds
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        // Set the camera's orthographic size based on the zoom level
        cam.orthographicSize = zoomLevel;

        if (isDragging)
        {
            lookToPosition = Vector3.one * int.MinValue;
        }

        if (lookToPosition != Vector3.one * int.MinValue && lookToPosition != transform.position)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, lookToPosition, Time.deltaTime * moveSpeed);

            // Move the camera to the new position and rotation
            transform.position = newPosition;
        }
        
    }

    public bool IsDragging()
    {
        return isDragging;
    }

    public void LookToCard(CardInPlay card)
    {
        Vector3Int v3 = new Vector3Int(card.hex.x, card.hex.y, 0);
        Vector3 v3World = tilemap.CellToWorld(v3);
        v3World = new Vector3(v3World.x, v3World.y, transform.position.z);
        LookTo(v3World);
    }
    public void LookTo(Vector3 position)
    {
        // Calculate the new position and rotation of the camera
        lookToPosition = position; 
    }
}