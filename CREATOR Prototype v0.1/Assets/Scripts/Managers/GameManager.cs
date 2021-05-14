using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public GameObject testDialogue;
    public int gashaponTokens;
    public Vector2 mousePos;
    public bool menuIsOpen;
    public GameObject openMenu;
    public bool dialogueIsActive;
    public Collider2D overlapPoint;

    public static int clickableLayer = 12; 
    public static int ignoreClickLayer = 2;
    public static int uiLayer = 5;
    public static int dialogueLayer = 13;

    private int dialogueLayerMask; // NO layers except for dialogue.
    private int uiLayerMask;
    private int ignoreClickLayerMask;

    private Camera mainCamera;
    private ItemDatabase itemDatabase;
    private PlayerInventory inventory;
    private StudioEditorManager studioEditor;
    private DialogueManager dialogueManager;

    void Awake()
    {
        mainCamera = Camera.main;
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();

        dialogueLayerMask = 1 << dialogueLayer;
        uiLayerMask = 1 << uiLayer;
        ignoreClickLayerMask = 1 << ignoreClickLayer; //  ALL layers except for layer 2 (the Ignore Raycast layer).
        ignoreClickLayerMask = ~ignoreClickLayerMask;
    }

    void Update()
    {
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Sets "menu is open" status depending on if there's an active open menu object or not:
        if (openMenu == null && menuIsOpen)
            menuIsOpen = false;
        else if (openMenu != null && !menuIsOpen)
            menuIsOpen = true;

        SpawnTestItems();
        CheckOverlapPoint();

        if (dialogueIsActive)
            DialogueInput();
    }

    void CheckOverlapPoint()
    {
        if (dialogueIsActive) // Disables all clicks except for dialogue UI if dialogue is active.
        {
            if (Input.GetMouseButtonDown(0) || dialogueManager.playerIsResponding)
            {
                overlapPoint = Physics2D.OverlapPoint(mousePos, dialogueLayerMask);
            }
        }
        else if (menuIsOpen) // Only UI assets are clickable while a menu is open.
        {
            // Switches active overlap point to closest collider on left click:
            if (Input.GetMouseButtonDown(0))
                overlapPoint = Physics2D.OverlapPoint(mousePos, uiLayerMask);

            // While inventory is open, checks if overlap point has changed every frame, then updates it to the nearest collider if it has:
            if (inventory.inventoryOpen && (Physics2D.OverlapPoint(mousePos) != overlapPoint))
                overlapPoint = Physics2D.OverlapPoint(mousePos, uiLayerMask);
        }
        else if (studioEditor.studioEditorActive)
        {
            // While studio editor is active, checks if overlap point has changed every frame, then updates it to hovered-over studio item if it has:
            if (Physics2D.OverlapPoint(mousePos) != overlapPoint)
            {
                overlapPoint = Physics2D.OverlapPoint(mousePos, ignoreClickLayerMask);
            }
            // (Updates every frame for the item highlights to function).
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // If no other requirements are met, only checks active collider on left click:
            overlapPoint = Physics2D.OverlapPoint(mousePos, ignoreClickLayerMask);
        }
    }

    void DialogueInput()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (!dialogueManager.isTypingOut && !dialogueManager.playerIsResponding)
                dialogueManager.NextLineOfDialogue(); // On button down, go to next liine of dialogue if dialogue isn't typing out.
            else if (dialogueManager.isTypingOut && !dialogueManager.autoSkipText)
                dialogueManager.typeOutSpeed = 0; // On button down, increase type-out speed if dialogue is typing out.
        }
    }

    void SpawnTestItems()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            itemDatabase.AddItem(1010201, true); // Spawn chair in inentory.

        if (Input.GetKeyDown(KeyCode.Alpha2))
            itemDatabase.AddItem(1010301, true); // Spawn table in inventory.

        if (Input.GetKeyDown(KeyCode.Alpha3))
            itemDatabase.AddItem(1011101, true); // Spawn calendar in inventory.

        if (Input.GetKeyDown(KeyCode.Alpha4))
            itemDatabase.AddItem(1011001, true); // Spawn art print in inventory.

        if (Input.GetKeyDown(KeyCode.Alpha5))
            itemDatabase.AddItem(1011901, true); // Spawn candle in inventory.

        if (Input.GetKeyDown(KeyCode.Alpha6))
            itemDatabase.AddItem(1011902, true); // Spawn legendary cactus in inventory. -Haley

        if (Input.GetKeyDown(KeyCode.Alpha7))
            itemDatabase.AddItem(1012102, true); // Spawn Devoid Carpet in inventory.

        if (Input.GetKeyDown(KeyCode.Alpha8))
            itemDatabase.AddItem(1012002, true); // Spawn Devoid Wallpaper in inventory.

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            // Spawn lots of items:
            itemDatabase.AddItem(1010201, true);
            itemDatabase.AddItem(1010301, true);
            itemDatabase.AddItem(1011101, true);
            itemDatabase.AddItem(1011001, true);
            itemDatabase.AddItem(1011901, true);
            itemDatabase.AddItem(1011902, true);
            itemDatabase.AddItem(1012102, true);
            itemDatabase.AddItem(1012002, true);
        }
    }
}
