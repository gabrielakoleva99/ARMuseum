using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

//This script was created while following a tutorial from Youtube user samyam (Tap to Place Objects in Unity's AR Foundation - Tutorial 2023)
//https://www.youtube.com/watch?v=lYDfV-GaKQA&t=748s
// The script had to be modified to fulfill the requirements for this application

//Script for placing 3D object on the AR plane
[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    
    // The prefab of the object that will be placed in the AR scene
    [SerializeField] private GameObject prefab;

   
   // Reference to the ARRaycastManager component, used for performing raycasts against AR planes
    private ARRaycastManager aRRaycastManager;

    
    // Reference to the ARPlaneManager component, for managing AR planes in the scene
    private ARPlaneManager aRPlaneManager;

   
    // A list consisting of raycast hits
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();


    // The currently selected or placed object in the AR scene
    private GameObject selectedObject = null;

    /// A boolean flag to check if an object is currently selected 
    private bool isObjectSelected = false;


    // Initializing an ARRaycastManager and an ARPlaneManager component
    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    // Enables touch input simulation and subscribes to touch events
    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    //Disables touch input simulation and unsubscribes to touch events
    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

 
    // Called by finger touch. Attempts to place the prefab in the AR scene at the location of the touch if no object is already selected.
    // <param name="finger">The finger that initiated the touch event.</param>
    private void FingerDown(EnhancedTouch.Finger finger)
    {
        // Only process the first finger (index 0)
        if (finger.index != 0) return;

        // Check if touch is over any selected object
        if (TouchIsOverObject(finger.currentTouch.screenPosition))
        {
            isObjectSelected = true;
            return; 
        }

        // Perform a raycast to detect AR planes
        if (!isObjectSelected && aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                Pose pose = hit.pose;
                selectedObject = Instantiate(prefab, pose.position, pose.rotation);
                selectedObject.transform.localScale = Vector3.one;
                isObjectSelected = true;
                break; 
            }
        }
    }

 

    // Checks if touching any object
    // <param name="touchPosition">The position of the touch on the screen.</param>
    // <returns>Returns true if the touch is over the selected object, false otherwise.</returns>
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

