using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

public class RandomSculpturePlacement : MonoBehaviour
{
    public GameObject[] models;
    public XROrigin arSessionOrigin;
    public ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Update()
    {
        // Check if there is a touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Perform a raycast from the touch position into the AR world
                bool collision = arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon);

                if (collision && hits.Count > 0)
                {
                    // Instantiate a random model at the hit position
                    GameObject obj = Instantiate(models[Random.Range(0, models.Length)]);
                    obj.transform.position = hits[0].pose.position;

                    // Optionally disable the plane manager and deactivate all planes
                    foreach (var plane in arPlaneManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }

                    arPlaneManager.enabled = false;
                }
            }
        }
    }
}
