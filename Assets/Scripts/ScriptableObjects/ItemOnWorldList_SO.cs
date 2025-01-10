using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemOnWorld {
    public string sceneName;
    public Vector2 position;
    public Quaternion rotation;
    public uint id;

    public ItemOnWorld(string sceneName, Vector2 position, Quaternion rotation, uint id) {
        this.sceneName = sceneName;
        this.position = position;
        this.rotation = rotation;
        this.id = id;
    }
    public override bool Equals(object obj) {
        return obj is ItemOnWorld world &&
               sceneName == world.sceneName &&
               position.Equals(world.position) &&
               rotation.Equals(world.rotation) &&
               id == world.id;
    }
    public override int GetHashCode() {
        return HashCode.Combine(sceneName, position, rotation, id);
    }
}

[CreateAssetMenu(fileName = "ItemOnWorldList_SO", menuName = "MySO/ItemOnWorldList")]
public class ItemOnWorldList_SO : ScriptableObject {
    public List<ItemOnWorld> itemOnWorldList = new();
}