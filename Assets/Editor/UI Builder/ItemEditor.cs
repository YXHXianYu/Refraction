using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    private ItemDataList_SO dataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;

    //默认预览图片
    private Sprite defaultIcon;

    private VisualElement iconPreview;
    //获得VisualElement
    private ListView itemListView;

    [MenuItem("Window/UI Toolkit/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // VisualElement label = new Label("Hello World! From C#");
        // root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //拿到模版数据
        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");

        //拿默认Icon图片
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        //变量赋值
        itemListView = root.Q<VisualElement>("LeftContainer").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");


        //获得按键
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;

        //加载数据
        LoadDataBase();

        //生成ListView
        GenerateListView();
    }

    #region 按键事件
    private void OnDeleteClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }

    private void OnAddItemClicked()
    {
        ItemDetails newItem = new ItemDetails();
        newItem.name = "NEW ITEM";
        newItem.id = (uint)(1001 + itemList.Count);
        itemList.Add(newItem);
        itemListView.Rebuild();
    }
    #endregion

    private void LoadDataBase()
    {
        var dataArray = AssetDatabase.FindAssets("t:ItemDataList_SO");  //不同版本写法不一样=
        if (dataArray.Length >= 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            dataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
        }

        itemList = dataBase.itemDetailsList;
        //如果不标记则无法保存数据
        EditorUtility.SetDirty(dataBase);
        // Debug.Log(itemList[0].itemID);
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].icon != null) {
                    e.Q<VisualElement>("Icon").style.backgroundImage = GetTexture2DFromAtlas(itemList[i].icon);
                }
                e.Q<Label>("Name").text = itemList[i] == null ? "NO ITEM" : itemList[i].name;
            }
        };

        itemListView.fixedItemHeight = 50;  //根据需要高度调整数值
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;

        itemListView.selectionChanged += OnListSelectionChange;
        // itemListView.onSelectionChange += OnListSelectionChange;

        //右侧信息面板不可见
        itemDetailsSection.visible = false;
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem = (ItemDetails)selectedItem.FirstOrDefault();
        GetItemDetails();
        itemDetailsSection.visible = true;
    }

    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();

        itemDetailsSection.Q<IntegerField>("ItemID").value = (int)activeItem.id;
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.id = (uint)evt.newValue;
        });

        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.name;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.name = evt.newValue;
            itemListView.Rebuild();
        });

        iconPreview.style.backgroundImage = activeItem.icon == null ? defaultIcon.texture : activeItem.icon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.icon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.icon = newIcon;

            if (newIcon == null) {
                iconPreview.style.backgroundImage = defaultIcon.texture;
            } else {
                iconPreview.style.backgroundImage = GetTexture2DFromAtlas(newIcon);
            }

            itemListView.Rebuild();
        });

        //其他所有变量的绑定
        itemDetailsSection.Q<ObjectField>("ItemIconOnWorld").value = activeItem.iconOnWorld;
        itemDetailsSection.Q<ObjectField>("ItemIconOnWorld").RegisterValueChangedCallback(evt =>
        {
            activeItem.iconOnWorld = (Sprite)evt.newValue;
        });

        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.type);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.type;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.type = (ItemType)evt.newValue;
        });

        itemDetailsSection.Q<TextField>("Description").value = activeItem.description;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.description = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = (int)activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = (uint)evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanPickedUp").value = activeItem.canPickedUp;
        itemDetailsSection.Q<Toggle>("CanPickedUp").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedUp = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("Price").value = (int)activeItem.price;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.price = (uint)evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }

    private Texture2D GetTexture2DFromAtlas(Sprite sprite) {

        var texture = SpriteUtility.GetSpriteTexture(sprite, false);

        return texture;

        // if (sprite.rect.width != sprite.texture.width)
        // {
        //     // 图集， 要求图集设置 Read/Write Enabled （尽量不要用 Texture，全部使用 Sprite，否则打开 Read/Write 后双倍内存），并且精灵设置为 Full Rect 而不能是Tight
        //     if ((int)sprite.rect.width != (int)sprite.textureRect.width || (int)sprite.rect.height != (int)sprite.textureRect.height)
        //     {
        //         Debug.LogErrorFormat("CreateTextureBySprite: sprite [{0}] must be FULL RECT，请在精灵属性设置，不能是Tight，否则宽度高度不对! \r\n sprite.rect: width={1}, height={2}; textureRect: x={3}, y={4}, width={5}, height={6}",
        //                 sprite.name, (int)sprite.rect.width, (int)sprite.rect.height,
        //                 (int)sprite.textureRect.x,
        //                 (int)sprite.textureRect.y,
        //                 (int)sprite.textureRect.width,
        //                 (int)sprite.textureRect.height);
        //     }
        //     Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        //     Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
        //                                                     (int)sprite.textureRect.y,
        //                                                     (int)sprite.textureRect.width,
        //                                                     (int)sprite.textureRect.height);

        //     newText.SetPixels(newColors);
        //     newText.Apply();
        //     return newText;
        // }
        // else
        // {
        //     return sprite.texture;
        // }
    }
}
