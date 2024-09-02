using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class DataHandler : MonoBehaviour
{
    [SerializeField] private ButtonManager buttonManager;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private List<Item> items;
    [SerializeField] private string label;
    private GameObject model;
    private int currentId = 0;


    private static DataHandler instance;
    public static DataHandler Instance {
    get
        {
            if(instance == null)
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
        Debug.Log("Items count after loading: " + items.Count); // Debug log to check item count
        CreateButton();
    }

    // Property to get and set the model
    public GameObject Model
    {
        get { return model; }
        set { model = value; }
    }


    void CreateButton()
    {
        foreach (Item i in items)
        {
            ButtonManager b = Instantiate(buttonManager, buttonContainer.transform);
            b.ItemId = currentId;
            b.ButtonImage = i.itemImage;
            currentId++;
        }
    }

    public void SetModel(int id)
    {
        model = items[id].itemPrefab;
    }
    // Method to return the current model
    public GameObject GetModels()
    {
        return model;
    }

    public async Task Get(string label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        foreach(var location in locations)
        {
            var obj = await Addressables.LoadAssetAsync<Item>(location).Task;
            items.Add(obj);

        }
    }
}
