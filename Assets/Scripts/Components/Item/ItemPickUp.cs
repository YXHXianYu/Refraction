using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickUp : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other) {
        Item item = other.GetComponent<Item>();
        OnTriggerEnter2DItem(item);
    }

    void OnTriggerEnter2DItem(Item item) {
        if (item == null) return;
        if (item.ItemDetails == null) return;

        if (item.ItemDetails.canPickedUp) {
            if(!InventoryManager.Instance.ModifyPlayerBackpackItem(item.ItemDetails.id, 1)) {
                Debug.LogWarning("Cannot pick up item: " + item.ItemDetails.name);
            }

            ItemManager.Instance.RemoveItemFromListAndScene(
                new ItemOnWorld(item.gameObject.scene.name, item.transform.position, item.transform.rotation, item.itemId),
                item.gameObject
            );
        }
    }
}
