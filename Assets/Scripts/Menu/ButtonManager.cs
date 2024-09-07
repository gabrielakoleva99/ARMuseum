using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

// manages the behavior and properties of a UI button that represents a 3D model in an AR application.
public class ButtonManager : MonoBehaviour
{
    //button for 3d model
    private Button button;

    //the id of the model
    private int itemId;

    //the image coresponding to the model
    private Sprite buttonImage;

    //the component for displaying the image
    [SerializeField] private RawImage rawImage;

    //sets the item id
    public int ItemId
    {
        set
        {
            itemId = value;
        }
    }

    //sets the image as Sprite object
    public Sprite ButtonImage
    {
        set
        {
            buttonImage = value;
            rawImage.texture = buttonImage.texture;
        }
    }
    // assigns the selectObject method to the button component
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectObject);
    }

    /// Checks if the mouse is hovering over the button and scales the button accordingly.
    void Update()
    {
        if (ModelSwiper.Instance.OnEntered(gameObject))
        {
            transform.localScale = Vector3.one*2;

        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    //sets the model in the dataHandler using the model id
    public void SelectObject()
    {
        DataHandler.Instance.SetModel(itemId);
    }
}
