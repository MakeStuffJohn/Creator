using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganizeOldestInventoryButton : MonoBehaviour
{
    public Sprite newestSprite;
    public Sprite oldestSprite;
    public GameObject playerItems;

    private GameManager gameManager;
    private PlayerInventory inventory;
    private OrganizeItems itemOrganizer;
    private Collider2D thisCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        itemOrganizer = playerItems.GetComponent<OrganizeItems>();
        thisCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint))
        {
            if (!inventory.organizeNewestToOldest)
            {
                inventory.organizeNewestToOldest = true;
                inventory.DeselectItem();
                inventory.pageActive = 1;
                itemOrganizer.OrganizePageNewestToOldest();
                spriteRenderer.sprite = oldestSprite;
            }
            else if (inventory.organizeNewestToOldest)
            {
                inventory.organizeNewestToOldest = false;
                inventory.DeselectItem();
                inventory.pageActive = 1;
                itemOrganizer.OrganizePageNewestToOldest();
                spriteRenderer.sprite = newestSprite;
            }
        }
    }
}
