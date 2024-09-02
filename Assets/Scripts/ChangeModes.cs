using UnityEngine;
using UnityEngine.UI;
using CW.Common;
using PaintCore;
using PaintIn3D;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class ChangeModes : MonoBehaviour
{
    [SerializeField] private GameObject SeatedAmenhotep;
    [SerializeField] private Button paintModeButton;
    [SerializeField] private Button interactionModeButton;

    private ARSelectionInteractable selectionInteractable;
    private ARRotationInteractable rotationInteractable;
    private CwPaintableMesh paintableMesh;
    private CwPaintableMeshTexture paintableTexture;

    private void Start()
    {
        Debug.Log("Start method called");

        // Find and assign the components in the hierarchy
        GameObject untitled2 = SeatedAmenhotep.transform.Find("untitled2").gameObject;
        GameObject model = SeatedAmenhotep.transform.Find("untitled2/default").gameObject;

        // Assign components
        selectionInteractable = untitled2.GetComponent<ARSelectionInteractable>();
        rotationInteractable = untitled2.GetComponent<ARRotationInteractable>();
        paintableMesh = model.GetComponent<CwPaintableMesh>();
        paintableTexture = model.GetComponent<CwPaintableMeshTexture>();

        // Log to confirm assignment
        Debug.Log($"selectionInteractable: {selectionInteractable}");
        Debug.Log($"rotationInteractable: {rotationInteractable}");
        Debug.Log($"paintableMesh: {paintableMesh}");
        Debug.Log($"paintableTexture: {paintableTexture}");

        // Ensure the model starts in interaction mode
        EnableInteractionMode();

        // Assign button click events
        paintModeButton.onClick.AddListener(EnablePaintMode);
        interactionModeButton.onClick.AddListener(EnableInteractionMode);
    }

    private void EnablePaintMode()
    {
        Debug.Log("Paint mode enabled");

        // Enable the Paintable components
        if (paintableMesh != null)
        {
            paintableMesh.enabled = true;
            Debug.Log("paintableMesh enabled");
        }

        if (paintableTexture != null)
        {
            paintableTexture.enabled = true;
            Debug.Log("paintableTexture enabled");
        }

        // Disable selection and rotation interactables
        if (selectionInteractable != null)
        {
            selectionInteractable.enabled = false;
            Debug.Log("selectionInteractable disabled");
        }

        if (rotationInteractable != null)
        {
            rotationInteractable.enabled = false;
            Debug.Log("rotationInteractable disabled");
        }
    }

    private void EnableInteractionMode()
    {
        Debug.Log("Interaction mode enabled");

        // Disable the Paintable components
        if (paintableMesh != null)
        {
            paintableMesh.enabled = false;
            Debug.Log("paintableMesh disabled");
        }

        if (paintableTexture != null)
        {
            paintableTexture.enabled = false;
            Debug.Log("paintableTexture disabled");
        }

        // Enable selection and rotation interactables
        if (selectionInteractable != null)
        {
            selectionInteractable.enabled = true;
            Debug.Log("selectionInteractable enabled");
        }

        if (rotationInteractable != null)
        {
            rotationInteractable.enabled = true;
            Debug.Log("rotationInteractable enabled");
        }
    }
}
