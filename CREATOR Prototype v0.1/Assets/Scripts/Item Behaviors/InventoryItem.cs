using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem : MonoBehaviour
{
    public Item itemDetails;
    public int itemSlot;
    public int typeSlot;
    public bool isNew;
    public GameObject newBadge;
    public GameObject wallPatternSmiley;
    public Sprite[] itemIcons;

    [SerializeField] private bool itemSelected;

    private SpriteRenderer spriteRenderer;
    private Collider2D thisCollider;
    private PlayerInventory inventory;
    private GameManager gameManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        SetItemIcon();
        SetInventoryPosition();
        if (!isNew)
            Destroy(newBadge);
    }

    void Update()
    {
        if (itemSelected && (inventory.selectedItem != this.gameObject))
        {
            itemSelected = false;
        }
    }

    void LateUpdate()
    {
        if (this.thisCollider == gameManager.overlapPoint)
        {
            if (isNew && inventory.selectedItem == null)
            {
                isNew = false;
                Destroy(newBadge);
            }

            if (Input.GetMouseButtonDown(0))
                ItemSelected();
        }

        if (Input.GetMouseButtonDown(0) && (gameManager.overlapPoint == null))
        {
            inventory.DeselectItem();
        }
    }

    void ItemSelected()
    {
        itemSelected = true;
        inventory.InventoryButtonsFollow(this.gameObject);
    }

    public void SetInventoryPosition()
    {
        int currentItemSlot = itemSlot % PlayerInventory.slotsPerPage;
        float slotWidth = 1;
        float slotHeight = 1;

        if (inventory.organizeByType || inventory.organizeByNew)
            currentItemSlot = typeSlot % PlayerInventory.slotsPerPage;

        Vector2 firstSlotPos = new Vector2(-5, 4);
        int inventoryColumn = currentItemSlot % 11;
        int inventoryRow = -((currentItemSlot - inventoryColumn) / 11); // The value is negative because it's moving down the screen.
        Vector2 addSlots = new Vector2(slotWidth * inventoryColumn, slotHeight * inventoryRow);
        Vector2 slotPos = firstSlotPos + addSlots;

        transform.position = slotPos;

        // NOTE: If the inventory row length changes, just change the number you % and / by to whatever the row length is.
        // ALSO NOTE: If the space between slots changes, just make the x of the addSlots the horizontal distance between the centers of two slots, and do the same for the y but vertical distance.
    }

    void SetItemIcon()
    {
        switch (itemDetails.itemType)
        {
            case ItemType.Desk:
                spriteRenderer.sprite = itemIcons[01];
                break;
            case ItemType.Chair:
                spriteRenderer.sprite = itemIcons[02];
                break;
            case ItemType.Table:
                spriteRenderer.sprite = itemIcons[03];
                break;
            case ItemType.MusicPlayer:
                spriteRenderer.sprite = itemIcons[04];
                break;
            case ItemType.Storage:
                spriteRenderer.sprite = itemIcons[05];
                break;
            case ItemType.ClockOut:
                spriteRenderer.sprite = itemIcons[06];
                break;
            case ItemType.Notes:
                spriteRenderer.sprite = itemIcons[07];
                break;
            case ItemType.DROPList:
                spriteRenderer.sprite = itemIcons[08];
                break;
            case ItemType.ProjectBinder:
                spriteRenderer.sprite = itemIcons[09];
                break;
            case ItemType.ArtPrint:
                spriteRenderer.sprite = itemIcons[10];
                break;
            case ItemType.Calendar:
                spriteRenderer.sprite = itemIcons[11];
                break;
            case ItemType.Checklist:
                spriteRenderer.sprite = itemIcons[12];
                break;
            case ItemType.Shelf:
                spriteRenderer.sprite = itemIcons[13];
                break;
            case ItemType.Window:
                spriteRenderer.sprite = itemIcons[14];
                break;
            case ItemType.Scene:
                spriteRenderer.sprite = itemIcons[15];
                break;
            case ItemType.Mug:
                spriteRenderer.sprite = itemIcons[16];
                break;
            case ItemType.Sticker:
                spriteRenderer.sprite = itemIcons[17];
                break;
            case ItemType.WallDecor:
                spriteRenderer.sprite = itemIcons[18];
                break;
            case ItemType.FloorDecor:
                spriteRenderer.sprite = itemIcons[19];
                break;
            case ItemType.WallPattern:
                spriteRenderer.sprite = itemIcons[20];
                int smileyValue = Random.Range(1, 50);
                if (smileyValue == 7)
                    wallPatternSmiley.SetActive(true);
                break;
            case ItemType.FloorPattern:
                spriteRenderer.sprite = itemIcons[21];
                break;
            case ItemType.StudioExterior:
                spriteRenderer.sprite = itemIcons[22];
                break;
            case ItemType.Hat:
                spriteRenderer.sprite = itemIcons[23];
                break;
            case ItemType.Accessory:
                spriteRenderer.sprite = itemIcons[24];
                break;
            case ItemType.Backpack:
                spriteRenderer.sprite = itemIcons[25];
                break;
            case ItemType.Top:
                spriteRenderer.sprite = itemIcons[26];
                break;
            case ItemType.Bottom:
                spriteRenderer.sprite = itemIcons[27];
                break;
            case ItemType.Shoe:
                spriteRenderer.sprite = itemIcons[28];
                break;
            case ItemType.EnamelPin:
                spriteRenderer.sprite = itemIcons[29];
                break;
            case ItemType.Music:
                spriteRenderer.sprite = itemIcons[30];
                break;
            case ItemType.Currency:
                spriteRenderer.sprite = itemIcons[31];
                break;
            case ItemType.TaskSFX:
                spriteRenderer.sprite = itemIcons[32];
                break;
            case ItemType.TaskAnimation:
                spriteRenderer.sprite = itemIcons[33];
                break;
            case ItemType.SuccessAnimation:
                spriteRenderer.sprite = itemIcons[34];
                break;
            case ItemType.WorkTimerUpgrade:
                spriteRenderer.sprite = itemIcons[35];
                break;
            case ItemType.Font:
                spriteRenderer.sprite = itemIcons[36];
                break;
            case ItemType.UIAesthetic:
                spriteRenderer.sprite = itemIcons[37];
                break;
            case ItemType.KeyItem:
                spriteRenderer.sprite = itemIcons[38];
                break;
            default:
                break;
        }
    }
}
