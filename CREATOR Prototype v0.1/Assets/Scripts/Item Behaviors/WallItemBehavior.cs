﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallItemBehavior : MonoBehaviour
{
    [SerializeField] protected WallItem wallItemDetails;

    protected bool itemEditorActive;

    private int selectedItemOrder;
    private float itemMoveSpeed = 0.13f;
    private Vector3 horizontalSnapToGrid = new Vector3(0.5f, 0, 0);
    private Vector3 verticalSnapToGrid = new Vector3(0, 0.5f, 0);
    private bool moveLeftActive;
    private bool moveRightActive;
    private bool moveUpActive;
    private bool moveDownActive;
    private bool onFrontWall;
    private bool isBeingDragged;

    protected GameManager gameManager;
    protected StudioEditorManager studioEditor;
    protected ItemDatabase itemDatabase;
    protected OrganizeItems itemOrganizer;
    protected InventoryItem sourceItemDetails;
    protected SpriteRenderer spriteRenderer;
    protected Collider2D thisCollider;
    protected StudioItem studioItemDetails;

    protected void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        itemOrganizer = GameObject.Find("Player Items").GetComponent<OrganizeItems>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        studioItemDetails = GetComponent<StudioItem>();

        sourceItemDetails = studioItemDetails.sourceInventoryItem.GetComponent<InventoryItem>();
    }

    protected void Start()
    {
        CalculateSortingOrder();

        if (studioItemDetails.wasPlacedFromInventory)
        {
            ActivateItemEditor();
        }
    }

    protected void Update()
    {
        if (itemEditorActive)
        {
            ItemEditorMode();
            CalculateSortingOrder();
        }

        if (thisCollider.OverlapPoint(gameManager.mousePos) && studioEditor.studioEditorActive && (studioEditor.itemEditorsInUse <= 0))
        {
            HighlightItem();

            if (Input.GetMouseButtonUp(0))
                ActivateItemEditor();
        }

        if (!thisCollider.OverlapPoint(gameManager.mousePos) & wallItemDetails.highlightSprite)
            UnhighlightItem();
    }

    protected void LateUpdate()
    {
        if (itemEditorActive)
            OutOfBounds();
    }

    void ActivateItemEditor()
    {
        itemEditorActive = true;
        studioEditor.cancelButton.SetActive(true);
        studioEditor.placeButton.SetActive(true);
        studioEditor.itemEditorsInUse++;
        studioEditor.activeItem = this.gameObject;
        studioEditor.activeItemDetails = GetComponent<StudioItem>();
        selectedItemOrder++;
        UnhighlightItem();
    }

    void ItemEditorMode()
    {
        GridMovement();
        studioEditor.ItemEditorButtonFollow(this.gameObject, spriteRenderer.bounds.size.y, "Wall Item"); // Activates Item Arrow.

        if (studioEditor.studioEditorActive == false && !studioEditor.cannotPlace)
            studioEditor.PlaceItem(); // Automatically places selected item if you deactivate Studio Editor mode.

        if (studioEditor.cannotPlace)
        {
            // When selected item is in area where you can't place it, item becomes semi-transparent and the no-symbol activates.
            studioEditor.NoSymbolFollow(this.gameObject);
            studioEditor.noSymbol.SetActive(true);
            spriteRenderer.color = StudioEditorManager.semitransparent;

            if (studioEditor.studioEditorActive == false)
                ItemCannotBePlaced();
        }
        if (!studioEditor.cannotPlace && studioEditor.noSymbol)
        {
            studioEditor.noSymbol.SetActive(false); // Deactivates no-symbol when item is placeable.

            if (!onFrontWall)
                spriteRenderer.color = Color.white; // Returns item to full opacity if not it's not on the Front Wall.
        }

        if (studioEditor.itemEditorsInUse <= 0)
        {
            ItemPlaced();
        }
    }

    void ItemPlaced()
    {
        itemEditorActive = false;
        selectedItemOrder--;
        if (studioItemDetails.wasPlacedFromInventory) // Remove source inventory item from inventory if the physical item is placed.
        {
            studioItemDetails.wasPlacedFromInventory = false;
            itemOrganizer.RemoveItemFromInventory(studioItemDetails.sourceInventoryItem, sourceItemDetails.itemSlot);
            // studioItemDetails.sourceInventoryItem = null;
        }
    }

    void ItemCannotBePlaced()
    {
        // Automatically removes item and no-symbol if Studio Editor is deactivated and item can't be placed.
        studioEditor.PlaceItem();
        studioEditor.noSymbol.SetActive(false);
        studioEditor.cannotPlace = false;
        if (!studioItemDetails.wasPlacedFromInventory)
            itemDatabase.AddItem(studioItemDetails.itemID, false);
        Destroy(this.gameObject);
    }

    void GridMovement()
    {
        // Move item LEFT:
        if (Input.GetButton("Left"))
            moveLeftActive = true;
        if (Input.GetButtonDown("Left"))
            StartCoroutine(GridMovementRoutine("Left"));
        if (Input.GetButtonUp("Left"))
            moveLeftActive = false;

        // Move item RIGHT:
        if (Input.GetButton("Right"))
            moveRightActive = true;
        if (Input.GetButtonDown("Right"))
            StartCoroutine(GridMovementRoutine("Right"));
        if (Input.GetButtonUp("Right"))
            moveRightActive = false;

        // Move item UP:
        if (Input.GetButton("Up"))
            moveUpActive = true;
        if (Input.GetButtonDown("Up"))
            StartCoroutine(GridMovementRoutine("Up"));
        if (Input.GetButtonUp("Up"))
            moveUpActive = false;

        // Move item DOWN:
        if (Input.GetButton("Down"))
            moveDownActive = true;
        if (Input.GetButtonDown("Down"))
            StartCoroutine(GridMovementRoutine("Down"));
        if (Input.GetButtonUp("Down"))
            moveDownActive = false;

        // Drag item:
        if (Input.GetMouseButtonDown(0) && thisCollider.OverlapPoint(gameManager.mousePos))
            isBeingDragged = true;
        if (isBeingDragged)
            transform.position = SnapToGrid();
        if (Input.GetMouseButtonUp(0))
            isBeingDragged = false;

        // Place item:
        if ((Input.GetButton("Submit") || Input.GetKeyDown(KeyCode.E)) && !studioEditor.cannotPlace && !Input.GetButton("Movement") && !Input.GetMouseButton(0))
        {
            studioEditor.PlaceItem();
        }
    }

    IEnumerator GridMovementRoutine(string direction)
    {
        // Move it LEFT:
        while (moveLeftActive && direction == "Left")
        {
            if (!moveRightActive)
            {
                transform.position += -horizontalSnapToGrid; // Straight left.
                if (moveUpActive && !moveDownActive)
                    transform.position += verticalSnapToGrid; // Diagonal left-up.
                else if (!moveUpActive && moveDownActive)
                    transform.position += -verticalSnapToGrid; // Diagonal left-down.
            }
            yield return new WaitForSeconds(itemMoveSpeed);
        }

        // Move item RIGHT:
        while (moveRightActive && direction == "Right")
        {
            if (!moveLeftActive)
            {
                transform.position += horizontalSnapToGrid; // Straight right.
                if (moveUpActive && !moveDownActive)
                    transform.position += verticalSnapToGrid; // Diagonal right-up.
                else if (!moveUpActive && moveDownActive)
                    transform.position += -verticalSnapToGrid; // Diagonal right-down.
            }
            yield return new WaitForSeconds(itemMoveSpeed);
        }

        // Move item UP:
        while (moveUpActive && direction == "Up")
        {
            if ((!moveDownActive && !moveLeftActive && !moveRightActive) || (!moveDownActive && moveLeftActive && moveRightActive))
                transform.position += verticalSnapToGrid; // Straight up.
            yield return new WaitForSeconds(itemMoveSpeed);
        }

        // Move item DOWN:
        while (moveDownActive && direction == "Down")
        {
            if ((!moveUpActive && !moveLeftActive && !moveRightActive) || (!moveUpActive && moveLeftActive && moveRightActive))
                transform.position += -verticalSnapToGrid; // Straight down.
            yield return new WaitForSeconds(itemMoveSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Front Wall Collider")
        {
            onFrontWall = true;

            if (!studioEditor.cannotPlace)
            {
                // If you can place the item and it's on the front wall, it flips and becomes semi-transparent.
                spriteRenderer.flipX = true;
                spriteRenderer.color = StudioEditorManager.semitransparent;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall Collider") || other.gameObject.CompareTag("Wall Item"))
            studioEditor.cannotPlace = true; // On collision with floor, door, or another wall item, you cannot place item.
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall Collider") || other.gameObject.CompareTag("Wall Item"))
        {
            studioEditor.cannotPlace = false;

            if (onFrontWall)
            { 
                // If selected item leaves area where it can't be placed and is still on front wall, it flips and becomes semi-transparent.
                spriteRenderer.flipX = true;
                spriteRenderer.color = StudioEditorManager.semitransparent;
            }
        }

        if (other.gameObject.name == "Front Wall Collider")
        {
            // When selected item leaves front wall area, it flips again and returns to full opacity.
            onFrontWall = false;
            spriteRenderer.flipX = false;
            spriteRenderer.color = Color.white;
        }
    }

    Vector2 SnapToGrid()
    {
        float currentItemHeight = spriteRenderer.bounds.size.y * 2;
        float currentItemWidth = spriteRenderer.bounds.size.x * 2;

        Vector2 roundedMousePos = new Vector2(Mathf.Round(gameManager.mousePos.x * 2) / 2, Mathf.Round(gameManager.mousePos.y * 2) / 2); // If item height and width are BOTH EVEN.

        if ((currentItemHeight % 2) == 0 && (currentItemWidth % 2) == 1)
                roundedMousePos = new Vector2(Mathf.Round(gameManager.mousePos.x * 2) / 2 + 0.25f, Mathf.Round(gameManager.mousePos.y * 2) / 2); // If item height is EVEN and width is ODD.

        else if ((currentItemHeight % 2) == 1)
        {
            roundedMousePos = new Vector2(Mathf.Round(gameManager.mousePos.x * 2) / 2 + 0.25f, Mathf.Round(gameManager.mousePos.y * 2) / 2 + 0.25f); // If item height and width are BOTH ODD.

            if ((currentItemWidth % 2) == 0)
                roundedMousePos = new Vector2(Mathf.Round(gameManager.mousePos.x * 2) / 2, Mathf.Round(gameManager.mousePos.y * 2) / 2 + 0.25f); // if item height is ODD and item width is EVEN.
        }

        return roundedMousePos;
    }

    void HighlightItem()
    {
        wallItemDetails.highlightSprite.SetActive(true);
        wallItemDetails.highlightSprite.transform.position = transform.position;
    }

    void UnhighlightItem()
    {
        wallItemDetails.highlightSprite.SetActive(false);
    }

    void CalculateSortingOrder()
    {
        float bottomOfItem = transform.position.y - (spriteRenderer.bounds.size.y / 2);
        int adjustedItemOrder = -(Mathf.RoundToInt(bottomOfItem * 100) / 5);

        float distanceFromCamera = ((bottomOfItem * 100) / 500);

        if (adjustedItemOrder >= 90) // If wall item is on front wall:
        {
            adjustedItemOrder = 500;
            distanceFromCamera = -1.5f;
        }
        else if (adjustedItemOrder >= -10 && adjustedItemOrder <= 30) // If wall item is on loft wall:
        {
            adjustedItemOrder = 30;
            distanceFromCamera = -0.3f;
        }    
        else if (adjustedItemOrder <= -80) // If wall item is on back wall:
        {
            adjustedItemOrder = -500;
            distanceFromCamera = 1.5f;
        }

        spriteRenderer.sortingOrder = adjustedItemOrder + selectedItemOrder;
        transform.position = new Vector3(transform.position.x, transform.position.y, distanceFromCamera);
    }

    void OutOfBounds()
    {
        float itemWidth = spriteRenderer.bounds.size.x / 2; // Distance from item's center to its RIGHT edge.
        float itemHeight = spriteRenderer.bounds.size.y / 2; // Distance from item's center to its TOP edge.

        float itemLeftSide = transform.position.x - itemWidth;
        float itemRightSide = transform.position.x + itemWidth;
        float itemTopSide = transform.position.y + itemHeight;
        float itemBottomSide = transform.position.y - itemHeight;

        float itemHorizontalBounds = StudioEditorManager.studioBounds - itemWidth;
        float itemVerticalBounds = StudioEditorManager.studioBounds - itemHeight;

        if (itemLeftSide < -StudioEditorManager.studioBounds) // Left bounds.
            transform.position = new Vector3(-itemHorizontalBounds, transform.position.y, transform.position.z); 
        else if (itemRightSide > StudioEditorManager.studioBounds) // Right bounds.
            transform.position = new Vector3(itemHorizontalBounds, transform.position.y, transform.position.z);
        if (itemTopSide > StudioEditorManager.studioBounds) // Upper bounds.
            transform.position = new Vector3(transform.position.x, itemVerticalBounds, transform.position.z);
        else if (itemBottomSide < -StudioEditorManager.studioBounds) // Lower bounds.
            transform.position = new Vector3(transform.position.x, -itemVerticalBounds, transform.position.z);

        // Refreshing Item Arrow and no-symbol positions so they don't follow mouse positino out of bounds.
        if (itemLeftSide < -StudioEditorManager.studioBounds || itemRightSide > StudioEditorManager.studioBounds || itemTopSide > StudioEditorManager.studioBounds || itemBottomSide < -StudioEditorManager.studioBounds)
            studioEditor.ItemEditorButtonFollow(this.gameObject, spriteRenderer.bounds.size.y, "Wall Item");
        if (studioEditor.cannotPlace)
            studioEditor.NoSymbolFollow(this.gameObject);
    }
}
