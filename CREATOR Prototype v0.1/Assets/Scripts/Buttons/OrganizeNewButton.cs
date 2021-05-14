using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizeNewButton : MonoBehaviour
{
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
            inventory.DeselectItem();

            if (!inventory.organizeByNew)
            {
                inventory.organizeByType = false;
                inventory.organizeByNew = true;
                inventory.pageActive = 1;
                itemOrganizer.OrganizePage();
                inventory.DeselectItem();
                // Set UI highlight to this button.
            }
            else if (inventory.organizeByNew)
            {
                inventory.organizeByNew = false;
                inventory.pageActive = 1;
                itemOrganizer.OrganizePage();
                inventory.DeselectItem();
                // Set UI highlight to default inventory button.
            }
        }
    }
}
