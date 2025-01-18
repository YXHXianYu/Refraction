using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "MySO/ItemDataList")]
public class ItemDataList_SO : ScriptableObject {
    public List<ItemDetails> itemDetailsList = new();
}