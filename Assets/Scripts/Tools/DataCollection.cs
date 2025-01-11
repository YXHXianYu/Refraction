using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDetails {
    public uint id;
    public string name;
    public ItemType type;

    public Sprite icon;
    public Sprite iconOnWorld;

    public string description;
    public uint itemUseRadius;

    public bool canPickedUp;
    public bool canDropped;
    public bool canCarried;

    public uint price;
    [Range(0, 1)] public float sellPercentage;

    public ItemDetails() {
        id = 0;
        name = "New Item";
        type = ItemType.Seed;
        icon = null;
        iconOnWorld = null;
        description = "Description";
        itemUseRadius = 0;
        canPickedUp = true;
        canDropped = true;
        canCarried = true;
        price = 0;
        sellPercentage = 0.5f;
    }

    public ItemDetails(uint id, string name, ItemType type, Sprite icon, Sprite iconOnWorld, string description,
        uint itemUseRadius, bool canPickedUp, bool canDropped, bool canCarried, uint price, float sellPercentage) {
        this.id = id;
        this.name = name;
        this.type = type;
        this.icon = icon;
        this.iconOnWorld = iconOnWorld;
        this.description = description;
        this.itemUseRadius = itemUseRadius;
        this.canPickedUp = canPickedUp;
        this.canDropped = canDropped;
        this.canCarried = canCarried;
        this.price = price;
        this.sellPercentage = sellPercentage;
    }

    public ItemDetails Clone() {
        return new ItemDetails(id, name, type, icon, iconOnWorld, description, itemUseRadius, canPickedUp, canDropped,
            canCarried, price, sellPercentage);
    }
}

[Serializable]
public class ItemInventory {
    public uint id;
    public uint amount;

    public ItemInventory() {
        id = 0;
        amount = 0;
    }

    public ItemInventory(uint id, uint amount) {
        this.id = id;
        this.amount = amount;
    }

    public ItemInventory Clone() {
        return new ItemInventory(id, amount);
    }

    public bool Equals(ItemInventory itemInventory) {
        return id == itemInventory.id && amount == itemInventory.amount;
    }
}

[Serializable]
public class AnimatorChangingInformation {
    public AnimatorChangingItemType carryItemType;
    public AnimatorChangingBodyType carryBodyType;
    public AnimatorOverrideController animatorOverrideController;
}