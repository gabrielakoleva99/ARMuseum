using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceSittingGuy : MonoBehaviour
{
    public GameObject spawnPrefab;
    GameObject spawned;
    bool isSpawned;
    ARRaycastManager arraycastManager;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        isSpawned = false;
        arraycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            if(arraycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitpose = hits[0].pose;
                if (!isSpawned)
                {
                    spawned = Instantiate(spawnPrefab, hitpose.position, hitpose.rotation);
                    isSpawned = true;
                }
                else
                {
                    spawned.transform.position = hitpose.position;
                }
            }
        }
    }
}
