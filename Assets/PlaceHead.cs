using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceHead : MonoBehaviour
{

    public GameObject spawnPrefab;
    private GameObject spawned;
    private bool isSpawned;
    private ARRaycastManager arraycastManager;
    private ARAnchorManager anchorManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        isSpawned = false;
        arraycastManager = GetComponent<ARRaycastManager>();
        anchorManager = GetComponent<ARAnchorManager>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began)
                return;

            if (isSpawned && TouchIsOverObject(touch))
            {
                SelectObject();
                return;
            }

            if (arraycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitpose = hits[0].pose;

                if (!isSpawned)
                {
                    spawned = Instantiate(spawnPrefab, hitpose.position, hitpose.rotation);
                    Debug.Log("Object spawned at: " + hitpose.position);

                    // Add anchor to the spawned object using AddComponent
                    ARAnchor anchor = spawned.AddComponent<ARAnchor>();
                    if (anchor == null)
                    {
                        Debug.LogError("Error creating anchor for the placed object.");
                    }

                    isSpawned = true;
                }
                else
                {
                    spawned.transform.position = hitpose.position;
                    Debug.Log("Object moved to: " + hitpose.position);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any planes.");
            }
        }
    }

    private bool TouchIsOverObject(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == spawned)
            {
                return true;
            }
        }
        return false;
    }

    private void SelectObject()
    {
        Debug.Log("Object selected: " + spawned.name);
        // Additional logic for selecting the object can go here
    }
}