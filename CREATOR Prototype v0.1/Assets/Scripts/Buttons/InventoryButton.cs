using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButton : MonoBehaviour
{
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    private GameManager gameManager;
    private PlayerInventory inventory;
    private SpriteRenderer spriteRenderer;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        if ((Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint)) || Input.GetKeyDown(KeyCode.I))
        {
            if (!inventory.inventoryOpen)
            {
                inventory.OpenInventory(false);
                spriteRenderer.sprite = activeSprite;
            }
            else if (inventory.inventoryOpen)
            {
                inventory.CloseInventory();
                spriteRenderer.sprite = inactiveSprite;
            }
        }
    }
}
