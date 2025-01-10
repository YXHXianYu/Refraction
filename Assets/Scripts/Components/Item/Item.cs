using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public uint itemId;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private ItemDetails itemDetails;

    public ItemDetails ItemDetails {
        get {
            return itemDetails;
        }
    }

    private void Awake() {
        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start() {
        if (itemId != 0) {
            Init(itemId);
        }
    }

    public void Init(uint itemId) {
        this.itemId = itemId;
        
        itemDetails = InventoryManager.Instance.GetItemDetailsById(itemId);

        if (itemDetails != null) {
            // sprite
            if (itemDetails.iconOnWorld != null) {
                spriteRenderer.sprite = itemDetails.iconOnWorld;
            } else {
                spriteRenderer.sprite = itemDetails.icon;
                Debug.LogWarning("No iconOnWorld found for item: " + itemId + " - " + itemDetails.name + ". Using icon instead.");
            }
            // collider
            var newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            boxCollider2D.size = newSize;
            boxCollider2D.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
        }
    }


    
}
