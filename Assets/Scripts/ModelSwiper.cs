using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ModelSwiper : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    public Transform selectionPoint;

    // Singleton instance
    private static ModelSwiper _instance;

    public static ModelSwiper Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ModelSwiper>();
                // Optionally, create a new GameObject if no instance is found
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

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = selectionPoint.position;
    }

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
