using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SlotInventory :
    MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, // 拖拽
    IPointerEnterHandler, IPointerExitHandler // 鼠标悬停
{
    public int slotIndex;

    public SlotType slotType;

    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    // do not modify this field in SlotInventory, use UIManager instead
    [SerializeField] private GameObject highlight;

    private ItemInventory previousItemInventory;

    private void Awake() {
        highlight.SetActive(false);
    }

    private void Start() {
        InitData();
        UpdateData();
    }

    private void Update() {
        UpdateData();
    }

    private void UpdateData() {
        if (slotType == SlotType.PlayerBackpack) UpdatePlayerBackpack();
    }

    private void InitData() {
        if (slotType == SlotType.PlayerBackpack) InitPlayerBackpack();
    }

    private void InitPlayerBackpack() {
    }

    private void UpdatePlayerBackpack() {
        var item = GetPlayerBackpackItemBySlotIndex();
        if (item == null) return;

        if (previousItemInventory != null && item.Equals(previousItemInventory)) {
            // do nothing
        }
        else if (item.id == 0) {
            previousItemInventory = item.Clone();

            image.sprite = null;
            image.color = new Color(1, 1, 1, 0);
            text.text = "";
            text.enabled = false;
            button.interactable = false;

            if (highlight.activeSelf) UIManager.Instance.SetSelectedHighlight(highlight);
        }
        else {
            var itemDetails = InventoryManager.Instance.GetItemDetailsById(item.id);

            image.sprite = itemDetails.icon;
            image.color = new Color(1, 1, 1, 1);
            text.text = item.amount.ToString();
            text.enabled = true;
            button.interactable = true;

            // Debug.Log("UpdatePlayerBackpack: " + item.id + " ; " + item.amount);

            previousItemInventory = item.Clone();
        }
    }

    private ItemInventory GetPlayerBackpackItemBySlotIndex() {
        if (slotIndex < 0) return null;

        if (InventoryManager.Instance == null)
            // Debug.LogWarning("InventoryManager.Instance is null");
            return null;

        return InventoryManager.Instance.GetPlayerBackpackItemByIndex(slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (button.interactable == false) return;

        UIManager.Instance.SetSelectedHighlight(highlight);
        if (slotType.Equals(SlotType.PlayerBackpack)) {
            var item = GetPlayerBackpackItemBySlotIndex();
            if (item != null)
                DefaultEventEmitter.Instance.Emit(
                    "Select Player Backpack Slot",
                    InventoryManager.Instance.GetItemDetailsById(item.id),
                    highlight.activeSelf
                );
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        var item = InventoryManager.Instance.GetPlayerBackpackItemByIndex(slotIndex);
        if (item == null || item.amount == 0) return;

        UIManager.Instance.dragImage.gameObject.SetActive(true);
        UIManager.Instance.dragImage.sprite = image.sprite;
        UIManager.Instance.dragImage.SetNativeSize();

        if (!highlight.activeSelf) UIManager.Instance.SetSelectedHighlight(highlight);
    }

    public void OnDrag(PointerEventData eventData) {
        UIManager.Instance.dragImage.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        UIManager.Instance.dragImage.gameObject.SetActive(false);

        var item = InventoryManager.Instance.GetPlayerBackpackItemByIndex(slotIndex);
        if (
            eventData.pointerCurrentRaycast.gameObject != null
            && eventData.pointerCurrentRaycast.gameObject.TryGetComponent<SlotInventory>(out var slotInventory)
        ) {
            // remove
            InventoryManager.Instance.SetPlayerBackpackItemByIndex(slotIndex, new ItemInventory());
            // add
            InventoryManager.Instance.SetPlayerBackpackItemByIndex(slotInventory.slotIndex, item);
        }
        else {
            // Map
            var pos_in_screen = new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                -Camera.main.transform.position.z);
            var pos = Camera.main.ScreenToWorldPoint(pos_in_screen);

            // Debug.Log("Drop item: " + item.id + " at " + pos);

            var itemDetails = InventoryManager.Instance.GetItemDetailsById(item.id);
            if (itemDetails.canDropped == false) {
                Debug.Log("Cannot drop item: " + itemDetails.name);
            }
            else {
                // remove
                InventoryManager.Instance.SetPlayerBackpackItemByIndex(slotIndex, new ItemInventory());
                // add
                for (var i = 0; i < item.amount; i++) {
                    var pos_d = new Vector2(pos.x + Random.Range(-0.5f, 0.5f), pos.y + Random.Range(-0.5f, 0.5f));
                    ItemOnWorld itemOnWorld = new(
                        ItemManager.Instance.GetCurrectMapSceneName(),
                        new Vector2(pos_d.x, pos_d.y),
                        Quaternion.identity,
                        item.id
                    );
                    ItemManager.Instance.AddItemToListAndScene(itemOnWorld);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        // if (eventData.delta == Vector2.zero) return; // 避免反复触发（之前这个bug是由于鼠标准心指在tooltip上，已修复）
        if (button.interactable == false) return;

        var item = InventoryManager.Instance.GetPlayerBackpackItemByIndex(slotIndex);
        if (item == null || item.amount == 0) return;

        var itemDetails = InventoryManager.Instance.GetItemDetailsById(item.id);

        // Debug.Log("In : " + slotIndex);
        var pos = GetComponent<RectTransform>().position;
        var size = GetComponent<RectTransform>().sizeDelta;
        UIManager.Instance.ShowTooltip(itemDetails, new Vector2(pos.x + size.x / 2, pos.y + size.y));
    }

    public void OnPointerExit(PointerEventData eventData) {
        // if (eventData.delta == Vector2.zero) return; // 避免反复触发（之前这个bug是由于鼠标准心指在tooltip上，已修复）
        // Debug.Log("Out: " + slotIndex + "; eventData: " + eventData);
        UIManager.Instance.HideTooltip();
    }
}