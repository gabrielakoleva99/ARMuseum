using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//handles detection of UI elements, when they are hover by or selected
public class ModelSwiper : MonoBehaviour
{
    //component for detecting UI objects
    private GraphicRaycaster raycaster; 

    //data for raycasting
    private PointerEventData pointerEventData; 

    //handles UI events
    private EventSystem eventSystem; 

    //original point for raycasting
    public Transform selectionPoint; // The point from which raycasting originates, typically the user's pointer or a designated area on the screen.

    // singleton instance of class
    private static ModelSwiper _instance;


    // gets the singleton istance or creates a new one if not found
    public static ModelSwiper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ModelSwiper>();
                // create a new GameObject if no instance is found
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ModelSwiper>();
                    singletonObject.name = typeof(ModelSwiper).ToString() + " (Singleton)";
                }
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

  
    // sets singleton instance of the class
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


   // initializes the necessary components for raycasting and sets up the pointer event data.
    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = selectionPoint.position;
    }

   
    // chechks if button is under pointer's position through raycast
    // <param name="button">button GameObject to check against the pointer's position.</param>
    // <returns>true if the button is under the pointer, otherwise false.</returns>
    public bool OnEntered(GameObject button)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == button)
            {
                return true;
            }
        }
        return false;
    }
}
