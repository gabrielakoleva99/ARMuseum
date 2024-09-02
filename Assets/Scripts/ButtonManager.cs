using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    private Button button;
   // public GameObject model;
    private int itemId;
    private Sprite buttonImage;
    [SerializeField] private RawImage rawImage;

    public int ItemId
    {
        set
        {
            itemId = value;
        }
    }

    public Sprite ButtonImage
    {
        set
        {
            buttonImage = value;
            rawImage.texture = buttonImage.texture;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectObject);
    }

    // Update is called once per frame
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
    
    public void SelectObject()
    {
        DataHandler.Instance.SetModel(itemId);
    }
}
