using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;  // Make sure this is included

public class SpawnObject : MonoBehaviour
{
    public GameObject objPrefab;
    private GameObject obj;
    private bool isSpawned;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private ARInputAction inputAction; // Reference to the generated Input Actions class

    void Awake()
    {
        inputAction = new ARInputAction(); // Initialize the Input Actions
    }

    void OnEnable()
    {
        inputAction.Enable(); // Enable the Input Actions
    }

    void OnDisable()
    {
        inputAction.Disable(); // Disable the Input Actions
    }

    // Start is called before the first frame update
    void Start()
    {
        isSpawned = false;
        raycastManager = GetComponent<ARRaycastManager>();

        // Subscribe to the touch event
        inputAction.TouchControl.PrimaryTouch.performed += ctx => OnTouch(ctx);
    }

    void OnTouch(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = context.ReadValue<Vector2>(); // Get the touch position

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitpose = hits[0].pose;
            if (!isSpawned)
            {
                obj = Instantiate(objPrefab, hitpose.position, hitpose.rotation);
                isSpawned = true;
            }
            else
            {
                obj.transform.position = hitpose.position;
            }
        }
    }
}
