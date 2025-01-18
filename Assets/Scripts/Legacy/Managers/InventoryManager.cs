using UnityEngine;
using UnityEngine.Assertions;

public class InventoryManager : Singleton<InventoryManager> {
    [Header("Item Data")] public ItemDataList_SO itemDataList_SO;

    [Header("Player Backpack Data")] public Inventory_SO playerBackpack_SO;


    /// <summary>
    /// 通过ID获取物品详细信息
    /// </summary>
    public ItemDetails GetItemDetailsById(uint itemId) {
        return itemDataList_SO.itemDetailsList.Find(item => item.id == itemId);
    }

    /// <summary>
    /// 将id为itemId的物品添加到背包、或从背包中去除（不负责销毁等操作）
    /// </summary>
    /// <returns>是否成功</returns>
    public bool ModifyPlayerBackpackItem(uint itemId, int amount_delta) {
        if (amount_delta == 0) {
            Debug.LogWarning("ChangeItemAmount: amount_delta is 0");
            return false;
        }
        else if (amount_delta > 0) {
            var index = GetPlayerBackpackItemIndexByItemId(itemId);
            if (index != -1) {
                playerBackpack_SO.itemList[index].amount =
                    (uint)(amount_delta + (int)playerBackpack_SO.itemList[index].amount);
                return true;
            }

            index = GetPlayerBackpackFirstEmptyIndex();
            if (index != -1) {
                playerBackpack_SO.itemList[index].id = itemId;
                playerBackpack_SO.itemList[index].amount = (uint)amount_delta;
                return true;
            }

            // backpack is full
            Debug.LogWarning("ChangeItemAmount: backpack is full");
            return false;
        }
        else {
            Assert.IsTrue(amount_delta < 0);

            var index = GetPlayerBackpackItemIndexByItemId(itemId);
            if (index == -1) {
                Debug.LogWarning("ChangeItemAmount: item not found");
                return false;
            }

            var sum = (int)playerBackpack_SO.itemList[index].amount + amount_delta;
            if (sum < 0) {
                Debug.LogWarning("ChangeItemAmount: amount is negative");
                return false;
            }
            else if (sum == 0) {
                playerBackpack_SO.itemList[index].id = 0;
                playerBackpack_SO.itemList[index].amount = 0;
                return true;
            }
            else {
                Assert.IsTrue(sum > 0);
                playerBackpack_SO.itemList[index].amount = (uint)sum;
                return true;
            }
        }
    }

    public bool IsPlayerBackpackEmpty() {
        foreach (var item in playerBackpack_SO.itemList)
            if (item.id != 0)
                return false;

        return true;
    }

    public int GetPlayerBackpackSize() {
        return playerBackpack_SO.itemList.Count;
    }

    /// <summary>
    /// return -1 if backpack is full
    /// </summary>
    public int GetPlayerBackpackFirstEmptyIndex() {
        for (var i = 0; i < playerBackpack_SO.itemList.Count; i++)
            if (playerBackpack_SO.itemList[i].id == 0)
                return i;

        return -1;
    }

    /// <summary>
    /// return -1 if not found
    /// </summary>
    public int GetPlayerBackpackItemIndexByItemId(uint itemId) {
        for (var i = 0; i < playerBackpack_SO.itemList.Count; i++)
            if (playerBackpack_SO.itemList[i].id == itemId)
                return i;

        return -1;
    }

    public ItemInventory GetPlayerBackpackItemByIndex(int index) {
        Assert.IsTrue(index >= 0 && index < playerBackpack_SO.itemList.Count &&
                      playerBackpack_SO.itemList[index] != null);
        return playerBackpack_SO.itemList[index];
    }

    public void SetPlayerBackpackItemByIndex(int index, ItemInventory item) {
        Assert.IsTrue(index >= 0 && index < playerBackpack_SO.itemList.Count);
        playerBackpack_SO.itemList[index] = item;
    }

    protected override void Awake() {
        base.Awake();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
    }
}