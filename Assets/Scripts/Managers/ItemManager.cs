using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemManager : Singleton<ItemManager> {
    public GameObject itemPrefab;
    public ItemOnWorldList_SO itemOnWorldList_SO;

    private Scene targetScene;
    private Transform itemParent;

    public void AddItemToListAndScene(ItemOnWorld item) {
        // list
        itemOnWorldList_SO.itemOnWorldList.Add(item);
        // scene
        GenerateItemInScene(item);
    }

    public void RemoveItemFromListAndScene(ItemOnWorld item, GameObject go) {
        // list
        itemOnWorldList_SO.itemOnWorldList.Remove(item);
        // scene
        Destroy(go);
    }
    
    public string GetCurrectMapSceneName() {
        return targetScene.name;
    }

    private void OnEnable() {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    private void Start() {
        // Debug.Log("Currect scene name: " + targetScene.name);
        LoadItemsToScene();
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // already has a map scene
        if (targetScene.name != null) return;
        // its not a map scene
        if (!int.TryParse(scene.name.Split('.')[0], out var _)) return;
        // setup
        targetScene = scene;
    }
    
    private void OnSceneUnloaded(Scene scene) {
        Debug.Log("Scene unloaded: " + scene.name);
        // its not the current target scene
        if (scene != targetScene) return;
        // reset
        targetScene = default;
        // remove item parent and items
        RemoveItemsFromScene();
    }

    private void LoadItemsToScene() {
        // generate item parent
        SceneManager.SetActiveScene(targetScene);
        itemParent = new GameObject("ItemParent").transform;
        itemParent.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // generate items
        foreach (var item in itemOnWorldList_SO.itemOnWorldList) {
            GenerateItemInScene(item);;
        }
    }

    private void RemoveItemsFromScene() {
        // TODO: 是否需要在场景unload时，手动删除itemParent？
    }

    private void GenerateItemInScene(ItemOnWorld item) {
        if (item.sceneName != targetScene.name) return;

        var itemObj = Instantiate(itemPrefab, item.position, item.rotation, itemParent);
        itemObj.GetComponent<Item>().itemId = item.id;
    }
}
