using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {
    [SerializeField] private GameObject playerBackpack;
    [SerializeField] private GameObject actionBar;
    [SerializeField] private GameObject tooltip;

    public Image dragImage;

    private GameObject selectedHighlight;

    protected override void Awake() {
        base.Awake();

        playerBackpack.SetActive(false);
        actionBar.SetActive(true);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) TogglePlayerBackpack();
    }

    public void TogglePlayerBackpack() {
        playerBackpack.SetActive(!playerBackpack.activeSelf);
        if (!playerBackpack.activeSelf)
            // Debug.Log("Player backpack closed.");
            HideTooltip();
    }

    public void ToggleActionBar() {
        actionBar.SetActive(!actionBar.activeSelf);
    }

    public void SetSelectedHighlight(GameObject highlight) {
        if (selectedHighlight == highlight) {
            selectedHighlight.SetActive(false);
            selectedHighlight = null;
        }
        else if (selectedHighlight != null) {
            selectedHighlight.SetActive(false);
            selectedHighlight = highlight;
            selectedHighlight.SetActive(true);
        }
        else {
            Assert.IsNull(selectedHighlight);
            selectedHighlight = highlight;
            selectedHighlight.SetActive(true);
        }
    }

    public void ShowTooltip(ItemDetails itemDetails, Vector2 position) {
        tooltip.transform.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
        tooltip.transform.position = new Vector3(position.x, position.y, 0);
        tooltip.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemDetails.name;
        tooltip.transform.Find("Wrapper").Find("Description").GetComponent<TextMeshProUGUI>().text
            = "Description:\n  " + (itemDetails.description == "" || itemDetails.description == null
                ? "<null>"
                : itemDetails.description);
        tooltip.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = itemDetails.price.ToString() + "$";
        tooltip.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip.transform.GetComponent<RectTransform>());
    }

    public void HideTooltip() {
        tooltip.SetActive(false);
    }
}