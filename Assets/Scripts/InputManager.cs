using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.ARSubsystems;

public class InputManager : ARBaseGestureInteractable
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] GameObject crosshair;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose pose;
    private GameObject placedObject;

    private float initialDistance;
    private Vector3 initialScale;
    private float initialAngle;
    private Quaternion initialRotation;

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        return gesture.targetObject == null;
    }


protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.isCanceled || gesture.targetObject != null || IsPointerOverUi(gesture))
        {
            return;
        }

        if (raycastManager.Raycast(gesture.startPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            if (hits.Count > 0) // Ensure that there is at least one valid hit
            {
                pose = hits[0].pose;

                // Get the plane associated with the raycast hit
                ARPlane hitPlane = planeManager.GetPlane(hits[0].trackableId);

                if (hitPlane != null)
                {
                    // Attach an anchor to the plane at the pose position
                    ARAnchor anchor = anchorManager.AttachAnchor(hitPlane, pose);

                    if (anchor == null)
                    {
                        Debug.LogError("Error creating anchor for the plane.");
                        return;
                    }

                    // Instantiate your model at the detected pose position and rotation
                    GameObject placedObject = Instantiate(DataHandler.Instance.GetModels(), pose.position, pose.rotation);

                    // Parent the object to the anchor
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


    void Update()
    {
        if (placedObject == null)
            return;

        if (UnityEngine.InputSystem.Touchscreen.current.touches.Count == 2)
        {
            var touch1 = UnityEngine.InputSystem.Touchscreen.current.touches[0];
            var touch2 = UnityEngine.InputSystem.Touchscreen.current.touches[1];

            // Zoom
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

                float factor = currentDistance / initialDistance;
                placedObject.transform.localScale = initialScale * factor;
            }

            // Rotation
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

    void FixedUpdate()
    {
        CrosshairCalculate();
    }

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

    void CrosshairCalculate()
    {
        Vector3 origin = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        if (raycastManager.Raycast(origin, hits, TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }
}
