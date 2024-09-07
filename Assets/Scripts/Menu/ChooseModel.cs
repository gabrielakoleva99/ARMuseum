using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// handles the selection of a 3D model in the AR environment 
public class ChooseModel : MonoBehaviour
{
    //button displaying the 3d model
    private Button btn;

    //the actual model
    public GameObject model;

    /// This method initializes the button component and assigns the SelectObject method as a listener to the button's onClick event.
    void Start()
    {
        // Get the Button component from the GameObject
        btn = GetComponent<Button>();

        // Add the SelectObject method as a listener to the onClick event
        btn.onClick.AddListener(SelectObject);
    }

    // This method is called when the button (image) is clicked
    void SelectObject()
    {
        if (DataHandler.Instance.GetModels() != null && model != null)
        {
            //initializing of the model
            DataHandler.Instance.Model = model;
            Debug.Log("Model selected: " + model.name);
        }
        else
        {
            Debug.LogError("DataHandler.Instance.GetModels() or model is null!");
        }
    }
}

