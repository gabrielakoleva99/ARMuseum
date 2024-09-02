using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "item1", menuName = "AddItem/Item")]
public class Item : ScriptableObject
{
  
    public GameObject itemPrefab;
    public Sprite itemImage;

}
