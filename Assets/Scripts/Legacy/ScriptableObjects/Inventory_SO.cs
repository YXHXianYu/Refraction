using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory_SO", menuName = "MySO/Inventory")]
public class Inventory_SO : ScriptableObject {
    public List<ItemInventory> itemList;
}