using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDatabase : MonoBehaviour
{
    public GameObject inventoryItem;
    public GameObject playerItems;
    public List<Item> itemDatabase = new List<Item>();

    private PlayerInventory inventory;
    private InventoryItem itemDetails;

    void Awake()
    {
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        itemDetails = inventoryItem.GetComponent<InventoryItem>();
    }

    public void AddItem(int ID, bool isNew)
    {
        foreach (var item in itemDatabase)
        {
            // Finds item in database based on its ID number:
            if (item.itemID == ID)
            {
                // Adds item to inventory (if inventory isn't full).
                if (inventory.itemsInInventory.Count < PlayerInventory.inventoryCapacity)
                {
                    // Gives inventory and inventory item the stats of the database's item:
                    inventory.itemsInInventory.Add(item);
                    itemDetails.itemDetails = item; 

                    // Sets item's slot position to the first available slot:
                    itemDetails.itemSlot = inventory.itemsInInventory.Count - 1;

                    // Adds or removes "NEW" badge depending on if the item is new or returning to the inventory, respectively.
                    if (isNew == true)
                        itemDetails.isNew = true;
                    else if (isNew == false)
                        itemDetails.isNew = false;

                    // Finally, it spawns the item within the inventory:
                    Instantiate(inventoryItem, playerItems.transform);
                }

                // if (inventory is full), then add it to archive. Tell the player it went to the archive.

                break;
            }
        }
    }
}
