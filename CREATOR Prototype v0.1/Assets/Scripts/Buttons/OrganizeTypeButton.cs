using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizeTypeButton : MonoBehaviour
{
    public ItemType itemToOrganize;
    public GameObject playerItems;

    private GameManager gameManager;
    private PlayerInventory inventory;
    private OrganizeItems itemOrganizer;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        itemOrganizer = playerItems.GetComponent<OrganizeItems>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint))
        {
            if (!inventory.organizeByType || (inventory.organizeByType && inventory.activeOrganizeType != itemToOrganize))
            {
                inventory.organizeByNew = false;
                inventory.organizeByType = true;
                inventory.activeOrganizeType = itemToOrganize;
                inventory.DeselectItem();
                inventory.pageActive = 1;
                itemOrganizer.OrganizePage();
                // Close item type buttons pop-up.
            }
            else if (inventory.organizeByType && inventory.activeOrganizeType == itemToOrganize)
            {
                inventory.organizeByType = false;
                inventory.DeselectItem();
                inventory.pageActive = 1;
                itemOrganizer.OrganizePage();
                // Close item type buttons pop-up.
            }
        }
    }
}
