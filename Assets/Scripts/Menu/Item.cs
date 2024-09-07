using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//creates object of type Item 
[CreateAssetMenu(fileName = "item1", menuName = "AddItem/Item")]
public class Item : ScriptableObject
{
  
    public GameObject itemPrefab;
    public Sprite itemImage;
    public GameObject infoPanel;  // Reference to the corresponding Info Panel

}
