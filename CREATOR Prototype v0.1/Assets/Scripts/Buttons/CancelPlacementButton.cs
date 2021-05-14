using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelPlacementButton : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerInventory inventory;
    private StudioEditorManager studioEditor;
    private ItemDatabase itemDatabase;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        // if (left click while this collider is the overlap point) OR (a menu is opened while an item is being placed), then send item back to inventory:
        if ((Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint)) || (gameManager.menuIsOpen && (studioEditor.itemEditorsInUse > 0)))
        {
            studioEditor.PlaceItem();
            studioEditor.noSymbol.SetActive(false);
            studioEditor.cannotPlace = false;

            if (studioEditor.activeItemDetails.wasPlacedFromInventory)
                inventory.OpenInventory(true);
            else if (!studioEditor.activeItemDetails.wasPlacedFromInventory)
                itemDatabase.AddItem(studioEditor.activeItemDetails.itemID, false);

            Destroy(studioEditor.activeItem);
        }
    }
}
