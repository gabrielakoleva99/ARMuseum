using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePlane : MonoBehaviour
{
    [SerializeField] private GameObject planePrefab;  // Assign your plane prefab here
    [SerializeField] private Button generateButton;   // Assign your UI Button here
    [SerializeField] private Camera arCamera;         // Assign your AR Camera here

    void Start()
    {
        // Ensure the button is assigned and add a listener to the button click event
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(GeneratePlane);
        }
        else
        {
            Debug.LogError("Generate Button not assigned in the Inspector.");
        }
    }

    void GeneratePlane()
    {
        // Calculate the position and rotation for the new plane relative to the camera
        Vector3 planePosition = arCamera.transform.position + arCamera.transform.forward * 1.0f;  // 1 meter in front of the camera
        Quaternion planeRotation = Quaternion.identity;

        // Instantiate the plane at the calculated position and rotation
        if (planePrefab != null)
        {
            GameObject spawnedPlane = Instantiate(planePrefab, planePosition, planeRotation);
            Debug.Log("Plane generated at: " + planePosition);

            // Instantiate the model using its prefab-defined position, rotation, and scale
            if (DataHandler.Instance.Model != null)
            {
                // Instantiate the model as-is, using the prefab's settings
                GameObject spawnedModel = Instantiate(DataHandler.Instance.Model);
                Debug.Log("Model spawned at prefab-defined position: " + spawnedModel.transform.position);

                // Optional: Parent the model to the plane, if needed
                // spawnedModel.transform.SetParent(spawnedPlane.transform, false);
            }
        }
        else
        {
            Debug.LogError("Plane prefab not assigned in the Inspector.");
        }
    }
}
