using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    /// <summary>
    /// The prefab of the object that will be placed in the AR scene.
    /// </summary>
    [SerializeField] private GameObject prefab;

    /// <summary>
    /// Reference to the ARRaycastManager component, used for performing raycasts against AR planes.
    /// </summary>
    private ARRaycastManager aRRaycastManager;

    /// <summary>
    /// Reference to the ARPlaneManager component, used for managing AR planes in the scene.
    /// </summary>
    private ARPlaneManager aRPlaneManager;

    /// <summary>
    /// A list to store the results of the raycast hits.
    /// </summary>
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    /// <summary>
    /// The currently selected or placed object in the AR scene.
    /// </summary>
    private GameObject selectedObject = null;

    /// <summary>
    /// A boolean flag to track if an object is currently selected or placed.
    /// </summary>
    private bool isObjectSelected = false;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Initializes references to ARRaycastManager and ARPlaneManager components.
    /// </summary>
    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    /// <summary>
    /// Called when the script is enabled.
    /// Enables touch input simulation and subscribes to touch events.
    /// </summary>
    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        // EnhancedTouch.Touch.onFingerMove += FingerMove;  // Uncomment if object movement is required
    }

    /// <summary>
    /// Called when the script is disabled.
    /// Disables touch input simulation and unsubscribes from touch events.
    /// </summary>
    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        // EnhancedTouch.Touch.onFingerMove -= FingerMove;  // Uncomment if object movement is required
    }

 
    /// Called when a finger touches the screen.
    /// Attempts to place the prefab in the AR scene at the location of the touch if no object is already selected.
    /// <param name="finger">The finger that initiated the touch event.</param>
    private void FingerDown(EnhancedTouch.Finger finger)
    {
        // Only process the first finger (index 0)
        if (finger.index != 0) return;

        // Check if the touch is over an already selected object
        if (TouchIsOverObject(finger.currentTouch.screenPosition))
        {
            isObjectSelected = true;
            return; // Exit if the object is selected
        }

        // Perform a raycast to detect AR planes
        if (!isObjectSelected && aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                Pose pose = hit.pose;
                // Instantiate the prefab at the hit pose position and rotation
                selectedObject = Instantiate(prefab, pose.position, pose.rotation);
                // Ensure the object is scaled correctly
                selectedObject.transform.localScale = Vector3.one;
                isObjectSelected = true;
                break; // Only place one object
            }
        }
    }

    /*
    /// <summary>
    /// Called when a finger moves on the screen.
    /// Allows rotation and scaling of the selected object.
    /// </summary>
    /// <param name="finger">The finger that is moving.</param>
    private void FingerMove(EnhancedTouch.Finger finger)
    {
        if (isObjectSelected && selectedObject != null)
        {
            // Rotate the selected object based on the finger movement
            Vector2 delta = finger.currentTouch.delta;
            selectedObject.transform.Rotate(0, -delta.x * 0.5f, 0, Space.World);

            // Handle pinch-to-zoom for scaling the object
            if (EnhancedTouch.Touch.activeFingers.Count == 2)
            {
                var touches = EnhancedTouch.Touch.activeTouches;
                var currentDistance = Vector2.Distance(touches[0].screenPosition, touches[1].screenPosition);
                var previousDistance = Vector2.Distance(touches[0].startScreenPosition, touches[1].startScreenPosition);
                float zoomFactor = (currentDistance - previousDistance) * 0.01f;

                selectedObject.transform.localScale += Vector3.one * zoomFactor;
            }
        }
    }
    */

    /// <summary>
    /// Checks if the touch is over an existing object in the scene.
    /// </summary>
    /// <param name="touchPosition">The position of the touch on the screen.</param>
    /// <returns>Returns true if the touch is over the selected object, false otherwise.</returns>
    private bool TouchIsOverObject(Vector2 touchPosition)
    {
        // Cast a ray from the touch position into the scene
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // Check if the ray hits an object
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is the currently selected object
            if (hit.collider != null && hit.collider.gameObject == selectedObject)
            {
                return true;
            }
        }
        return false;
    }
}

