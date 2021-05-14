using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Desk,
    Chair,
    Table,
    MusicPlayer,
    Storage,
    ClockOut,
    Notes,
    DROPList,
    ProjectBinder,
    ArtPrint,
    Calendar,
    Checklist,
    Shelf,
    Window,
    Scene,
    Mug,
    Sticker,
    WallDecor,
    FloorDecor,
    WallPattern,
    FloorPattern,
    StudioExterior,
    Hat,
    Accessory,
    Backpack,
    Top,
    Bottom,
    Shoe,
    EnamelPin,
    Music,
    Currency,
    TaskSFX,
    TaskAnimation,
    SuccessAnimation,
    WorkTimerUpgrade,
    Font,
    UIAesthetic,
    KeyItem
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public ItemType itemType;
    public Sprite defaultSprite;
    public int itemValue;
    public ItemRarity itemRarity;
    public bool isAnUpgrade;
    [TextArea(3, 3)] public string flavorText;
    public GameObject itemPrefab;
}
