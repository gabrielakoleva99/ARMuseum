using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseModel : MonoBehaviour
{
    private Button btn;
    public GameObject model;

    // Start is called before the first frame update
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
            DataHandler.Instance.Model = model;
            Debug.Log("Model selected: " + model.name);
        }
        else
        {
            Debug.LogError("DataHandler.Instance.GetModels() or model is null!");
        }
    }
}

