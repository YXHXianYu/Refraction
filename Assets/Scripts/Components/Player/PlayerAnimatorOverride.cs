using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAnimatorOverride : MonoBehaviour {
    private Animator[] animators;

    public SpriteRenderer holdItem;

    [Header("Carry动画列表")] public List<AnimatorChangingInformation> animatorChangingInformationList;

    private Dictionary<string, Animator> animatorNameDict = new();

    private EventEmitter.EventListener eventListener = null;

    private void Awake() {
        animators = GetComponentsInChildren<Animator>();

        foreach (var animator in animators) animatorNameDict.Add(animator.name, animator);
    }

    private void OnEnable() {
        eventListener = (args) => { EventSelectPlayerBackpackSlot((ItemDetails)args[0], (bool)args[1]); };
        DefaultEventEmitter.Instance.On("Select Player Backpack Slot", eventListener);
    }

    private void OnDisable() {
        DefaultEventEmitter.Instance?.Off("Select Player Backpack Slot", eventListener);
    }

    private void EventSelectPlayerBackpackSlot(ItemDetails itemDetails, bool isSelected) {
        // WORKFLOW: 不同工具的动画在这里修改 tag: Animator Switching

        AnimatorChangingItemType animatorChangingItemType;
        if (isSelected) {
            var tmp = itemDetails.type switch {
                ItemType.Seed => AnimatorChangingItemType.Carry,
                // ItemType.Commodity => AnimatorChangingItemType.Carry,
                _ => AnimatorChangingItemType.None
            };
            animatorChangingItemType = tmp;
        }
        else {
            animatorChangingItemType = AnimatorChangingItemType.None;
        }

        // hold item
        if (animatorChangingItemType == AnimatorChangingItemType.Carry) {
            holdItem.enabled = true;
            holdItem.sprite = itemDetails.iconOnWorld;
        }
        else if (animatorChangingItemType == AnimatorChangingItemType.None) {
            holdItem.enabled = false;
        }

        SwitchAnimator(animatorChangingItemType);
    }

    private void SwitchAnimator(AnimatorChangingItemType animatorChangingItemType) {
        foreach (var record in animatorChangingInformationList)
            if (record.carryItemType == animatorChangingItemType)
                animatorNameDict[record.carryBodyType.ToString()].runtimeAnimatorController =
                    record.animatorOverrideController;
    }
}