using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{
    [SerializeField] private ButtonManager buttonManager;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private List<Item> items;
    [SerializeField] private string label;
    [SerializeField] private Button infoButton;  // Reference to the InfoButton

    private GameObject model;     // Currently selected 3D model
    private int currentId = 0;
    private static DataHandler instance;

    public static DataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>();
            }
            return instance;
        }
    }

    private async void Start()
    {
        items = new List<Item>();
        await Get(label);
        CreateButton();

        // Ensure the InfoButton is linked and add a listener
        if (infoButton != null)
        {
            infoButton.onClick.AddListener(OnInfoButtonClicked);
        }
    }

    // Method that gets called when the InfoButton is clicked
    private void OnInfoButtonClicked()
    {
        // Check if a model is currently selected
        if (model != null)
        {
            // Find the corresponding item for the selected model and show its info panel
            foreach (Item item in items)
            {
                if (item.itemPrefab == model)
                {
                    ShowModelInfo(item);
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("No model selected. Please select a model first.");
        }
    }

    public GameObject Model
    {
        get { return model; }
        set { model = value; }
    }

    // Create buttons for each item dynamically
    void CreateButton()
    {
        foreach (Item i in items)
        {
            ButtonManager b = Instantiate(buttonManager, buttonContainer.transform);
            b.ItemId = currentId;
            b.ButtonImage = i.itemImage;

            // Add a listener to show the info panel for the corresponding model when the button is clicked
            b.GetComponent<Button>().onClick.AddListener(() => SetModelAndShowInfo(i));

            currentId++;
        }
    }

    // Set the selected model and show its info panel
    private void SetModelAndShowInfo(Item selectedItem)
    {
        model = selectedItem.itemPrefab;
        ShowModelInfo(selectedItem);
    }

    // Show the selected model's corresponding info panel and hide all others
    private void ShowModelInfo(Item selectedItem)
    {
        // Hide all other panels
        foreach (Item item in items)
        {
            if (item.infoPanel != null)
            {
                item.infoPanel.SetActive(false);
            }
        }

        // Show the selected item's info panel
        if (selectedItem.infoPanel != null)
        {
            selectedItem.infoPanel.SetActive(true);
        }
    }

    // returns the currently selected 3D model
    public GameObject GetModels()
    {
        return model;
    }

    // sets the current model based on the item ID
    public void SetModel(int id)
    {
        model = items[id].itemPrefab;
    }
    public async Task Get(string label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach (var location in locations)
        {
            var obj = await Addressables.LoadAssetAsync<Item>(location).Task;
            items.Add(obj);
        }
    }
}
