using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public GameObject playerItems;
    public GameObject studioItems;
    public GameObject activeWallPattern;
    public GameObject activeFloorPattern;
    public GameObject inventoryItemPrefab;
    public GameObject inventoryUI;
    public GameObject placeInventoryItemButton;
    public GameObject inspectInventoryItemButton;
    public GameObject archiveInventoryItemButton;
    public GameObject slotSelectedHighlight;
    public GameObject selectedItem = null;
    public bool inventoryOpen;
    public int slotSelected;
    public int pageActive;
    public bool organizeByType;
    public ItemType activeOrganizeType;
    public bool organizeByNew;
    public bool organizeNewestToOldest;
    public List<Item> itemsInInventory = new List<Item>();

    public static int slotsPerPage = 110;
    public static int inventoryCapacity = 550;
    public static Vector2 studioItemSpawnEvenPos = new Vector2(0, 0);
    public static Vector2 studioItemSpawnOddPos = new Vector2(0.25f, 0);

    private GameObject selectedPhysicalItem;
    private StudioItem physicalItemDetails;
    private SpriteRenderer physicalItemRenderer;
    private float scrollWheelInput;

    private ItemDatabase itemDatabase;
    private OrganizeItems itemOrganizer;
    private StudioEditorManager studioEditor;
    private GameManager gameManager;
    private InventoryItem inventoryItemDetails;

    void Awake()
    {
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        itemOrganizer = playerItems.GetComponent<OrganizeItems>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        // inventoryItemDetails = selectedItem.GetComponent<InventoryItem>();
        // selectedPhysicalItem = inventoryItemDetails.itemDetails.itemPrefab;
        // physicalItemDetails = selectedPhysicalItem.GetComponent<StudioItem>();
    }

    void Update()
    {
        if (inventoryOpen)
        {
            scrollWheelInput = Input.GetAxis("Mouse ScrollWheel"); // One scroll wheel click works the same as tapping a keyboard button once.

            if ((Input.GetKeyDown(KeyCode.DownArrow) || scrollWheelInput < 0) && pageActive < 5)
                ScrollDown();
            if ((Input.GetKeyDown(KeyCode.UpArrow) || scrollWheelInput > 0) && pageActive > 1)
                ScrollUp();

            if (gameManager.openMenu != this.gameObject)
                CloseInventory();
        }
    }

    public void InventoryButtonsFollow(GameObject item)
    {
        selectedItem = item;

        // Gets  values for the inventory item and its physical version:
        inventoryItemDetails = selectedItem.GetComponent<InventoryItem>();
        selectedPhysicalItem = inventoryItemDetails.itemDetails.itemPrefab;
        physicalItemDetails = selectedPhysicalItem.GetComponent<StudioItem>();
        physicalItemRenderer = selectedPhysicalItem.GetComponent<SpriteRenderer>();

        // Change first button depending on if it's clothing, decor, or an upgrade.

        Vector3 itemPos = item.transform.position;
        Vector3 placeButtonOffset = new Vector3(-0.89f, 0.89f, -1);
        Vector3 inspectButtonOffset = new Vector3(0, 0.89f, -1);
        Vector3 archiveButtonOffset = new Vector3(0.89f, 0.89f, -1);

        // Activates button UI:
        placeInventoryItemButton.SetActive(true);
        inspectInventoryItemButton.SetActive(true);
        archiveInventoryItemButton.SetActive(true);
        slotSelectedHighlight.SetActive(true);

        // Set buttons to item position:
        placeInventoryItemButton.transform.position = itemPos + placeButtonOffset;
        inspectInventoryItemButton.transform.position = itemPos + inspectButtonOffset;
        archiveInventoryItemButton.transform.position = itemPos + archiveButtonOffset;
        slotSelectedHighlight.transform.position = itemPos;
    }

    public void PlaceInventoryItem()
    {
        if (inventoryItemDetails.itemDetails.itemType != ItemType.WallPattern || inventoryItemDetails.itemDetails.itemType != ItemType.FloorPattern)
        {
            var physicalItemWidth = physicalItemRenderer.bounds.size.x * 2;
            studioEditor.studioEditorActive = true; // Activates Studio Editor.
            physicalItemDetails.sourceInventoryItem = selectedItem; // Sets the inventory item as the physical item's "source item".
            physicalItemDetails.wasPlacedFromInventory = true;
            if (physicalItemWidth % 2 == 0)
                Instantiate(selectedPhysicalItem, studioItemSpawnEvenPos, selectedPhysicalItem.transform.rotation, studioItems.transform); // Spawns physical item.
            else if (physicalItemWidth % 2 == 1)
                Instantiate(selectedPhysicalItem, studioItemSpawnOddPos, selectedPhysicalItem.transform.rotation, studioItems.transform);
        }
        else if (inventoryItemDetails.itemDetails.itemType == ItemType.WallPattern)
        {
            // Return active pattern to inventory:
            GameObject activePattern = activeWallPattern.transform.GetChild(0).gameObject;
            StudioItem activePatternDetails = activePattern.GetComponent<StudioItem>();
            itemDatabase.AddItem(activePatternDetails.itemID, false);
            Destroy(activePattern);

            // Spawn new pattern:
            physicalItemDetails.sourceInventoryItem = selectedItem;
            itemOrganizer.RemoveItemFromInventory(physicalItemDetails.sourceInventoryItem, inventoryItemDetails.itemSlot);
            Instantiate(selectedPhysicalItem, studioItemSpawnEvenPos, selectedPhysicalItem.transform.rotation, activeWallPattern.transform);
        }
        else if (inventoryItemDetails.itemDetails.itemType == ItemType.FloorPattern)
        {
            // Return active pattern to inventory:
            GameObject activePattern = activeFloorPattern.transform.GetChild(0).gameObject;
            StudioItem activePatternDetails = activePattern.GetComponent<StudioItem>();
            itemDatabase.AddItem(activePatternDetails.itemID, false);
            Destroy(activePattern);

            // Spawn new pattern:
            physicalItemDetails.sourceInventoryItem = selectedItem;
            Instantiate(selectedPhysicalItem, studioItemSpawnEvenPos, selectedPhysicalItem.transform.rotation, activeWallPattern.transform);
            itemOrganizer.RemoveItemFromInventory(selectedItem, inventoryItemDetails.itemSlot);
        }

        CloseInventory(); // Closes inventory UI.
    }

    public void OpenInventory(bool reopening)
    {
        if (reopening == true)
            pageActive = 1;
        inventoryOpen = true;
        inventoryUI.SetActive(true);
        playerItems.SetActive(true);
        itemOrganizer.OrganizePage();
        gameManager.openMenu = this.gameObject;
    }

    public void CloseInventory()
    {
        inventoryOpen = false; // This is where to fix the reopen inventory problem.
        organizeByType = false;
        organizeByNew = false;
        DeselectItem();
        inventoryUI.SetActive(false);
        playerItems.SetActive(false);
        if (gameManager.openMenu == this.gameObject)
            gameManager.openMenu = null;
    }

    public void DeselectItem()
    {
        selectedItem = null;
        placeInventoryItemButton.SetActive(false);
        inspectInventoryItemButton.SetActive(false);
        archiveInventoryItemButton.SetActive(false);
        slotSelectedHighlight.SetActive(false);
    }

    void ScrollDown()
    {
        pageActive++;
        itemOrganizer.OrganizePage();
    }

    void ScrollUp()
    {
        pageActive--;
        itemOrganizer.OrganizePage();
    }
}
