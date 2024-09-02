using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SPawn : MonoBehaviour
{
    [SerializeField] ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> m_hit = new List<ARRaycastHit>();
    [SerializeField] GameObject obj;

    Camera arCam;
    GameObject spawnObj;

    void Start()
    {
        spawnObj = null;
        arCam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update() {

        if (Input.touchCount == 0)
            return;
        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);
        if (aRRaycastManager.Raycast(Input.GetTouch(0).position, m_hit))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnObj == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnObj = hit.collider.gameObject;
                    }
                    else
                    {
                        SpawnPrefab(m_hit[0].pose.position);
                    }
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnObj != null)
                {
                    spawnObj.transform.position = m_hit[0].pose.position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    spawnObj = null;
                }
            }
        }
    }

        private void SpawnPrefab(Vector3 spawnPos)
        {
            spawnObj = Instantiate(obj, spawnPos, Quaternion.identity);
        }
    }

