using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorItemBehavior : MonoBehaviour
{
    [SerializeField] private FloorItem floorItemDetails; 
    public ItemDirection itemRotation;
    public bool itemEditorActive;


    private int selectedItemOrder;
    private float itemMoveSpeed = 0.13f;
    private Vector3 horizontalSnapToGrid = new Vector3(0.5f, 0, 0);
    private Vector3 verticalSnapToGrid = new Vector3(0, 0.5f, 0);
    private bool moveLeftActive;
    private bool moveRightActive;
    private bool moveUpActive;
    private bool moveDownActive;
    private bool isBeingDragged;

    protected StudioEditorManager studioEditor;
    protected GameManager gameManager;
    protected ItemDatabase itemDatabase;
    protected OrganizeItems itemOrganizer;
    protected InventoryItem sourceItemDetails;
    protected SpriteRenderer spriteRenderer;
    protected StudioItem studioItemDetails;
    protected Collider2D frontShapeCollider;
    protected Collider2D leftShapeCollider;
    protected Collider2D backShapeCollider;
    protected Collider2D rightShapeCollider;
    protected Collider2D activeShapeCollider;

    void Awake()
    {
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>(); // (Variable stored in item core script.)
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        itemOrganizer = GameObject.Find("Player Items").GetComponent<OrganizeItems>();
        studioItemDetails = GetComponent<StudioItem>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        sourceItemDetails = studioItemDetails.sourceInventoryItem.GetComponent<InventoryItem>();

        // Colliders for each direction the item can face:
        frontShapeCollider = floorItemDetails.frontShape.GetComponent<Collider2D>();
        leftShapeCollider = floorItemDetails.leftShape.GetComponent<Collider2D>();
        backShapeCollider = floorItemDetails.backShape.GetComponent<Collider2D>();
        rightShapeCollider = floorItemDetails.rightShape.GetComponent<Collider2D>();
        activeShapeCollider = frontShapeCollider;
    }

    void Start()
    {
        if (studioItemDetails.wasPlacedFromInventory)
        {
            ActivateItemEditor(); // Launches item editor mode if placed from inventory.
            itemRotation = ItemDirection.Front; // This avoids the item taking on the rotation of the previously placed floor item.
        }
        CalculateSortingOrder();
        CheckItemRotation();
    }

    void Update()
    {
        if (itemEditorActive)
        {
            ItemEditorMode();
            CalculateSortingOrder();
        }
    }

    void LateUpdate()
    {
        if ((activeShapeCollider == gameManager.overlapPoint) && studioEditor.studioEditorActive && (studioEditor.itemEditorsInUse <= 0))
        {
            // If hovering mouse over item outside of item editor mode:
            HighlightItem();

            if (Input.GetMouseButtonUp(0))
                ActivateItemEditor();
        }

        // If mouse isn't hovering over item, turns off any item highlights:
        if ((activeShapeCollider != gameManager.overlapPoint) & (floorItemDetails.frontHighlight || floorItemDetails.leftHighlight || floorItemDetails.backHighlight || floorItemDetails.rightHighlight))
            UnhighlightItem();

        if (itemEditorActive)
            OutOfBounds(); // Out of bounds checked in Late Update to avoid visual stuttering.
    }

    void ActivateItemEditor()
    {
        // CheckStartingItemRotation();
        itemEditorActive = true;

        // Activates item editor UI:
        studioEditor.cancelButton.SetActive(true);
        studioEditor.placeButton.SetActive(true);
        studioEditor.rotateButton.SetActive(true);

        studioEditor.itemEditorsInUse++; // Prevents other item editors from activating.
        studioEditor.activeItem = this.gameObject; // Gives the Studio Editor a public reference for this item.
        studioEditor.activeItemDetails = GetComponent<StudioItem>(); // Give Studio Editor access to this item's "StudioItem" variables.
        selectedItemOrder++; // Renders item over top of items on the same layer while in item editor.
        UnhighlightItem();
        RotateButton.onItemRotate += RotateItem; 
    }

    void ItemEditorMode()
    {
        GridMovement();
        CheckItemRotation();
        studioEditor.ItemEditorButtonFollow(this.gameObject, spriteRenderer.bounds.size.y, "Floor Item"); // Keeps item editor UI on selected item's position.

        if (studioEditor.studioEditorActive == false && !studioEditor.cannotPlace)
            studioEditor.PlaceItem(); // Automatically places selected item if you deactivate Studio Editor mode.

        if (studioEditor.cannotPlace)
        {
            // When selected item is in area where you can't place it, item becomes semi-transparent and the no-symbol activates.
            studioEditor.NoSymbolFollow(this.gameObject);
            studioEditor.noSymbol.SetActive(true);
            spriteRenderer.color = StudioEditorManager.semitransparent;

            if (studioEditor.studioEditorActive == false) //Sends item back to inventory if Studio Editor is deactivated while item cannot be placed.
                ItemCannotBePlaced();
        }

        if (!studioEditor.cannotPlace && studioEditor.noSymbol)
        {
            studioEditor.noSymbol.SetActive(false); // Deactivates no-symbol when item is placeable.
            spriteRenderer.color = Color.white;
        }

        if (studioEditor.itemEditorsInUse <= 0) // When item is placed/removed, deactivates item editor and lowers item sorting order again.
            ItemPlaced();
    }

    void ItemPlaced()
    {
        itemEditorActive = false;
        selectedItemOrder--;
        RotateButton.onItemRotate -= RotateItem;
        if (studioItemDetails.wasPlacedFromInventory) // Remove source inventory item from inventory if the physical item is placed.
        {
            studioItemDetails.wasPlacedFromInventory = false;
            itemOrganizer.RemoveItemFromInventory(studioItemDetails.sourceInventoryItem, sourceItemDetails.itemSlot); // Removes inventory version of item.
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
        if (Input.GetMouseButtonDown(0) && activeShapeCollider == gameManager.overlapPoint)
            isBeingDragged = true;
        if (isBeingDragged)
            transform.position = SnapToGrid();
        if (Input.GetMouseButtonUp(0))
            isBeingDragged = false;

        // Rotate item:
        if (Input.GetKeyDown(KeyCode.Q))
            RotateItem();

        // Place item:
        if ((Input.GetButton("Submit") || Input.GetKeyDown(KeyCode.E)) && !studioEditor.cannotPlace && !Input.GetButton("Movement") && !Input.GetMouseButton(0))
            studioEditor.PlaceItem();
    }

    IEnumerator GridMovementRoutine(string direction)
    {
        // Move item LEFT:
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

    Vector2 SnapToGrid() // Rounds mouse position to the nearest small-grid point.
    {
        float currentItemHeight = spriteRenderer.bounds.size.y * 2; // (Item sizes multiplied by two to get an int remainder when dividied by two.)
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
        // Activate and determine rotation of highlight sprite:
        switch (itemRotation)
        {
            case ItemDirection.Front:
                floorItemDetails.frontHighlight.SetActive(true);
                floorItemDetails.frontHighlight.transform.position = transform.position;
                break;
            case ItemDirection.Left:
                floorItemDetails.leftHighlight.SetActive(true);
                floorItemDetails.leftHighlight.transform.position = transform.position;
                break;
            case ItemDirection.Back:
                floorItemDetails.backHighlight.SetActive(true);
                floorItemDetails.backHighlight.transform.position = transform.position;
                break;
            case ItemDirection.Right:
                floorItemDetails.rightHighlight.SetActive(true);
                floorItemDetails.rightHighlight.transform.position = transform.position;
                break;
            default:
                break;
        }
    }

    void UnhighlightItem()
    {
        switch (itemRotation) // Determines which highlight rotation is active, then deactivates it.
        {
            case ItemDirection.Front:
                floorItemDetails.frontHighlight.SetActive(false);
                break;
            case ItemDirection.Left:
                floorItemDetails.leftHighlight.SetActive(false);
                break;
            case ItemDirection.Back:
                floorItemDetails.backHighlight.SetActive(false);
                break;
            case ItemDirection.Right:
                floorItemDetails.rightHighlight.SetActive(false);
                break;
            default:
                break;
        }
    }

    void RotateItem()
    {
        itemRotation++; // Turns current item.

        if (itemRotation == ItemDirection.Null)
            itemRotation = ItemDirection.Front; // If the item rotation enum reaches the end of the drop-down list (Null), start the list over at Front.
    }

    void CheckItemRotation()
    {
        // Change item sprite and shape collider depending on rotation:
        switch (itemRotation)
        {
            case ItemDirection.Front:
                spriteRenderer.sprite = studioItemDetails.defaultSprite; // Update item sprite.
                activeShapeCollider = frontShapeCollider; // Update which shape collider we want to click on.
                rightShapeCollider.gameObject.layer = GameManager.ignoreClickLayer; // Set last shape collider to no-longer-clickable.
                floorItemDetails.rotatedSquareCollider.SetActive(false); // Deactivate last square collider.
                floorItemDetails.defaultSquareCollider.SetActive(true); // Activate appropriate square collider. (Make sure you deactivate/activate in that order!)
                break;
            case ItemDirection.Left:
                spriteRenderer.sprite = floorItemDetails.leftSprite;
                activeShapeCollider = leftShapeCollider;
                frontShapeCollider.gameObject.layer = GameManager.ignoreClickLayer;
                floorItemDetails.defaultSquareCollider.SetActive(false);
                floorItemDetails.rotatedSquareCollider.SetActive(true);
                break;
            case ItemDirection.Back:
                spriteRenderer.sprite = floorItemDetails.backSprite;
                activeShapeCollider = backShapeCollider;
                leftShapeCollider.gameObject.layer = GameManager.ignoreClickLayer;
                floorItemDetails.rotatedSquareCollider.SetActive(false);
                floorItemDetails.defaultSquareCollider.SetActive(true);
                break;
            case ItemDirection.Right:
                spriteRenderer.sprite = floorItemDetails.rightSprite;
                activeShapeCollider = rightShapeCollider;
                backShapeCollider.gameObject.layer = GameManager.ignoreClickLayer;
                floorItemDetails.defaultSquareCollider.SetActive(false);
                floorItemDetails.rotatedSquareCollider.SetActive(true);
                break;
            default:
                break;
        }
        activeShapeCollider.gameObject.layer = GameManager.clickableLayer; // Make the active shape collider clickable.
    }

    void CalculateSortingOrder()
    {
        float bottomOfItem = transform.position.y - (spriteRenderer.bounds.size.y / 2); // Determines position of the bottom of the item.
        int adjustedItemOrder = -(Mathf.RoundToInt(bottomOfItem * 100) / 5); // Calculates an int based on the item's position, rounded to the nearest ten.
        spriteRenderer.sortingOrder = adjustedItemOrder + selectedItemOrder; // Sets item's sorting order to that int (+ 1 if item editor is active).
        float distanceFromCamera = ((bottomOfItem * 100) / 500);
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

        // Refreshing UI buttons and no-symbol positions in Late Update so they don't follow mouse position out of bounds.
        if (itemLeftSide < -StudioEditorManager.studioBounds || itemRightSide > StudioEditorManager.studioBounds || itemTopSide > StudioEditorManager.studioBounds || itemBottomSide < -StudioEditorManager.studioBounds)
            studioEditor.ItemEditorButtonFollow(this.gameObject, spriteRenderer.bounds.size.y, "Floor Item");
        if (studioEditor.cannotPlace)
            studioEditor.NoSymbolFollow(this.gameObject);
    }
}
