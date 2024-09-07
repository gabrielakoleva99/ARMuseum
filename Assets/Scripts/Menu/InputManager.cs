
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.ARSubsystems;

//  handles user input for placing and manipulating 3D objects in the AR space
public class InputManager : ARBaseGestureInteractable
{
    // serialized fields for AR camera, AR Raycast Manager, AR Anchor Manager, AR Plane Manager, and crosshair object.
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] GameObject crosshair;

    // list to store raycast hits 
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose pose;

    // object that is placed in the AR space.
    private GameObject placedObject;

    // variables to store initial values for scaling and rotation during gesture manipulation.
    private float initialDistance;
    private Vector3 initialScale;
    private float initialAngle;
    private Quaternion initialRotation;

    // determines whether a tap gesture can start the manipulation.
    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        return gesture.targetObject == null;
    }

    // handles the end of a tap gesture, used for placing an object in AR space.
    protected override void OnEndManipulation(TapGesture gesture)
    {
        // If the gesture is canceled, or if the user is interacting with a UI element, exit.
        if (gesture.isCanceled || gesture.targetObject != null || IsPointerOverUi(gesture))
        {
            return;
        }

        // perform a raycast to detect AR planes within the polygon defined by the gesture's start position.
        if (raycastManager.Raycast(gesture.startPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            if (hits.Count > 0)
            {
                pose = hits[0].pose; // Get the position and rotation of the first hit.

                // Retrieve the AR plane associated with the raycast hit.
                ARPlane hitPlane = planeManager.GetPlane(hits[0].trackableId);

                if (hitPlane != null)
                {
                    // Attach an anchor to the detected plane at the pose position.
                    ARAnchor anchor = anchorManager.AttachAnchor(hitPlane, pose);

                    if (anchor == null)
                    {
                        Debug.LogError("Error creating anchor for the plane.");
                        return;
                    }

                    // Instantiate the model at the detected position and rotation.
                    GameObject placedObject = Instantiate(DataHandler.Instance.GetModels(), pose.position, pose.rotation);

                    // Set the object as a child of the anchor to maintain its position relative to the AR plane.
                    placedObject.transform.SetParent(anchor.transform);

                    Debug.Log("Plane anchored successfully at " + anchor.transform.position);
                }
                else
                {
                    Debug.LogWarning("No plane found to attach anchor.");
                }
            }
            else
            {
                Debug.LogWarning("No valid hit detected by raycast.");
            }
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any surfaces.");
        }
    }

    //checks for multi-touch input for scaling and rotating the placed object.
    void Update()
    {
        if (placedObject == null)
            return;

        // Check if there are two active touches on the screen.
        if (UnityEngine.InputSystem.Touchscreen.current.touches.Count == 2)
        {
            var touch1 = UnityEngine.InputSystem.Touchscreen.current.touches[0];
            var touch2 = UnityEngine.InputSystem.Touchscreen.current.touches[1];

            // Handle zoom (scaling) functionality.
            if (touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began || touch2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touch1.position.ReadValue(), touch2.position.ReadValue());
                initialScale = placedObject.transform.localScale;
            }
            else if (touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved || touch2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch1.position.ReadValue(), touch2.position.ReadValue());
                if (Mathf.Approximately(initialDistance, 0))
                    return;

                // Scale the object proportionally to the change in distance between the touches.
                float factor = currentDistance / initialDistance;
                placedObject.transform.localScale = initialScale * factor;
            }

            // Handle rotation functionality.
            Vector2 prevDir = (touch1.position.ReadValue() - touch1.delta.ReadValue()) - (touch2.position.ReadValue() - touch2.delta.ReadValue());
            Vector2 currDir = touch1.position.ReadValue() - touch2.position.ReadValue();

            if (touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began || touch2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
            {
                initialAngle = Vector2.SignedAngle(prevDir, currDir);
                initialRotation = placedObject.transform.rotation;
            }
            else if (touch1.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved || touch2.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float angle = Vector2.SignedAngle(prevDir, currDir) - initialAngle;
                placedObject.transform.rotation = initialRotation * Quaternion.Euler(0, -angle, 0);
            }
        }
    }

    //  updates the crosshair position, indicating where the user can place the object.
    void FixedUpdate()
    {
        CrosshairCalculate();
    }

    // checks if the gesture is interacting with the UI instead of AR content.
    bool IsPointerOverUi(TapGesture gesture)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(gesture.startPosition.x, gesture.startPosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    // Updates the crosshair position and orientation based on the raycast hit on the detected AR plane.
    void CrosshairCalculate()
    {
        // Convert the center of the screen to world space and perform a raycast.
        Vector3 origin = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        if (raycastManager.Raycast(origin, hits, TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose; // Get the position of the first raycast hit.
            crosshair.transform.position = pose.position; // Update crosshair position.
            crosshair.transform.eulerAngles = new Vector3(90, 0, 0); // Set crosshair rotation to face upward.
        }
    }
}

