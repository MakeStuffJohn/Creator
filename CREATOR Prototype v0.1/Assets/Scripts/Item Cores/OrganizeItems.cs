using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OrganizeItems : MonoBehaviour
{
    private PlayerInventory inventory;

    void Awake()
    {
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
    }

    public void RemoveItemFromInventory(GameObject removedItem, int emptySlot)
    {
        InventoryItem itemSlot;

        Destroy(removedItem); // Removes item from inventory.
        inventory.itemsInInventory.RemoveAt(emptySlot); // Removes item stats from inventory list.

        foreach (Transform child in transform) // Goes through all inventory items.
        {
            itemSlot = child.gameObject.GetComponent<InventoryItem>(); 

            if (itemSlot.itemSlot > emptySlot)
                itemSlot.itemSlot--; // Moves all the items after the removed item back a spot to fill in the gap.

            child.transform.SetSiblingIndex(itemSlot.itemSlot); // Rearanges inventory item in the hierarchy based on its slot position.
        }

        OrganizePage(); // Reorganizes inventory based on new item slot values.
    }

    public void OrganizePageNewestToOldest()
    {
        // Observe: a beautiful plate of spaghetti.
        int t = 0; // Determines the final item's slot number.
        int n = 0;
        
        InventoryItem itemSlot;
        GameObject[] allInventoryItems = new GameObject[inventory.itemsInInventory.Count];

        if (!inventory.organizeNewestToOldest)
        {
            foreach (Transform child in transform)
            {
                allInventoryItems[n] = child.gameObject;
                n++;
            }

            for (int i = 0; i < inventory.itemsInInventory.Count; i++)
            {
                itemSlot = allInventoryItems[i].GetComponent<InventoryItem>();
                itemSlot.itemSlot = t; // Sets first item's slot number to the last item's slot number...
                t++;
                allInventoryItems[i].transform.SetSiblingIndex(itemSlot.itemSlot);
                itemSlot.SetInventoryPosition();
            }

            OrganizePage();
        }

        t = inventory.itemsInInventory.Count - 1;
        n = 0;

        foreach (Transform child in transform)
        {
            allInventoryItems[n] = child.gameObject;
            n++;
        }

        for (int i = 0; i < inventory.itemsInInventory.Count; i++)
        {
            itemSlot = allInventoryItems[i].GetComponent<InventoryItem>();
            itemSlot.itemSlot = t; // Sets first item's slot number to the last item's slot number...
            t--; // ...then subtracts from the iterator to give the next item the next-to-last slot number, and so on.
            allInventoryItems[i].transform.SetSiblingIndex(itemSlot.itemSlot);
            itemSlot.SetInventoryPosition();
        } 

        OrganizePage();
    }

    public void OrganizePage()
    {
        // Organizes page based on which sorting mode is active:
        if (!inventory.organizeByType && !inventory.organizeByNew)
            OrganizePageByDefault();
        else if (inventory.organizeByType)
            OrganizePageByType();
        else if (inventory.organizeByNew)
            OrganizePageByNew();
    }

    void OrganizePageByDefault()
    {
        InventoryItem itemSlot;
        int firstPageLength = PlayerInventory.slotsPerPage; // 110
        int secondPageLength = PlayerInventory.slotsPerPage * 2; // 220
        int thirdPageLength = PlayerInventory.slotsPerPage * 3; // 330
        int fourthPageLength = PlayerInventory.slotsPerPage * 4; // 440
        int fifthPageLength = PlayerInventory.slotsPerPage * 5; // 550

        switch (inventory.pageActive) // Sets certain inventory items to "active" depending on which inventory page is open.
        {
            case 1:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    itemSlot.SetInventoryPosition(); // Makes sure inventory item is in its correct slot.
                    // Activates or deactivates item depending on its page.
                    if (itemSlot.itemSlot < firstPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.itemSlot >= firstPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 2:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    itemSlot.SetInventoryPosition();
                    if (itemSlot.itemSlot >= firstPageLength && itemSlot.itemSlot < secondPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.itemSlot < firstPageLength || itemSlot.itemSlot >= secondPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 3:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    itemSlot.SetInventoryPosition();
                    if (itemSlot.itemSlot >= secondPageLength && itemSlot.itemSlot < thirdPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.itemSlot < secondPageLength || itemSlot.itemSlot >= thirdPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 4:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    itemSlot.SetInventoryPosition();
                    if (itemSlot.itemSlot >= thirdPageLength && itemSlot.itemSlot < fourthPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.itemSlot < thirdPageLength || itemSlot.itemSlot >= fourthPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 5:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    itemSlot.SetInventoryPosition();
                    if (itemSlot.itemSlot >= fourthPageLength && itemSlot.itemSlot < fifthPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.itemSlot < fourthPageLength || itemSlot.itemSlot >= fifthPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    void OrganizePageByType()
    {
        int i = 0;
        ItemType type = inventory.activeOrganizeType;
        InventoryItem itemSlot;
        int dontShowItem = PlayerInventory.inventoryCapacity + 1;
        int firstPageLength = PlayerInventory.slotsPerPage; // 110
        int secondPageLength = PlayerInventory.slotsPerPage * 2; // 220
        int thirdPageLength = PlayerInventory.slotsPerPage * 3; // 330
        int fourthPageLength = PlayerInventory.slotsPerPage * 4; // 440
        int fifthPageLength = PlayerInventory.slotsPerPage * 5; // 550

        switch (inventory.pageActive)
        {
            case 1:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    // First, we set the item's type slot #:
                    if (itemSlot.itemDetails.itemType == type)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (itemSlot.itemDetails.itemType != type)
                    {
                        itemSlot.typeSlot = dontShowItem; // Sets item's slot number so high that it's never seen on any of the pages.
                    }

                    // Then we set it as active or inactive depending on if it's on the current page:
                    if (itemSlot.typeSlot < firstPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot >= firstPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 2:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    if (itemSlot.itemDetails.itemType == type)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (itemSlot.itemDetails.itemType != type)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= firstPageLength && itemSlot.typeSlot < secondPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < firstPageLength || itemSlot.typeSlot >= secondPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 3:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    if (itemSlot.itemDetails.itemType == type)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (itemSlot.itemDetails.itemType != type)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= secondPageLength && itemSlot.typeSlot < thirdPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < secondPageLength || itemSlot.typeSlot >= thirdPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 4:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    if (itemSlot.itemDetails.itemType == type)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (itemSlot.itemDetails.itemType != type)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= thirdPageLength && itemSlot.typeSlot < fourthPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < thirdPageLength || itemSlot.typeSlot >= fourthPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 5:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    if (itemSlot.itemDetails.itemType == type)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (itemSlot.itemDetails.itemType != type)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= fourthPageLength && itemSlot.typeSlot < fifthPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < fourthPageLength || itemSlot.typeSlot >= fifthPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    void OrganizePageByNew()
    {
        int i = 0;
        InventoryItem itemSlot;
        int dontShowItem = PlayerInventory.inventoryCapacity + 1;
        int firstPageLength = PlayerInventory.slotsPerPage; // 110
        int secondPageLength = PlayerInventory.slotsPerPage * 2; // 220
        int thirdPageLength = PlayerInventory.slotsPerPage * 3; // 330
        int fourthPageLength = PlayerInventory.slotsPerPage * 4; // 440
        int fifthPageLength = PlayerInventory.slotsPerPage * 5; // 550

        switch (inventory.pageActive)
        {
            case 1:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    // First, we set the item's type slot #:
                    if (itemSlot.isNew)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (!itemSlot.isNew)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    // Then we set it as active or inactive depending on if it's on the current page:
                    if (itemSlot.typeSlot < firstPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot >= firstPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 2:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();
                    if (itemSlot.isNew)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (!itemSlot.isNew)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= firstPageLength && itemSlot.typeSlot < secondPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < firstPageLength || itemSlot.typeSlot >= secondPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 3:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    if (itemSlot.isNew)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (!itemSlot.isNew)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= secondPageLength && itemSlot.typeSlot < thirdPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < secondPageLength || itemSlot.typeSlot >= thirdPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 4:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    if (itemSlot.isNew)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (!itemSlot.isNew)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= thirdPageLength && itemSlot.typeSlot < fourthPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < thirdPageLength || itemSlot.typeSlot >= fourthPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            case 5:
                foreach (Transform child in transform)
                {
                    itemSlot = child.GetComponent<InventoryItem>();

                    if (itemSlot.isNew)
                    {
                        itemSlot.typeSlot = i;
                        itemSlot.SetInventoryPosition();
                        i++;
                    }
                    else if (!itemSlot.isNew)
                    {
                        itemSlot.typeSlot = dontShowItem;
                    }

                    if (itemSlot.typeSlot >= fourthPageLength && itemSlot.typeSlot < fifthPageLength)
                        child.gameObject.SetActive(true);
                    else if (itemSlot.typeSlot < fourthPageLength || itemSlot.typeSlot >= fifthPageLength)
                        child.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
}
