using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GashaponMachine : MonoBehaviour
{
    public GameObject inventoryItem;
    public GameObject playerItems;
    public GameObject gashaponScreen;
    public List<Item> commonPrizePool = new List<Item>();
    public List<Item> rarePrizePool = new List<Item>();
    public List<Item> epicPrizePool = new List<Item>();
    public List<Item> legendaryPrizePool = new List<Item>();

    private int rareOdds = 30;
    private int epicOdds = 10;
    private int legendaryOdds = 1;

    private PlayerInventory inventory;
    private GameManager gameManager;

    private InventoryItem inventoryItemDetails;
    // private Collider2D thisCollider;
    // private Collider2D gashaponLeverCollider;

    void Awake()
    {
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inventoryItemDetails = inventoryItem.GetComponent<InventoryItem>();
        // Get thisCollider.
        // Get gashaponLeverCollider;
    }

    void Update()
    {
        // if (click on thisCollider), then bring up Gashapon screen;
        // Animation for dropping in token? Manually drag the token over to the slot, then snaps to animation? (Or token moves on a line.)
        // if (drag on gashaponLever), lowering gameManager.mousePos = lower gashapon level sprite;
        // if (drag on gashaponLever reaches certain negative mousePos), then RollGashapon();
    }

    void RollGashapon()
    {
        gameManager.gashaponTokens--;

        int playerRoll = Random.Range(0, 100);

        if (playerRoll > rareOdds)
            WinCommonPrize();
        else if (playerRoll <= rareOdds && playerRoll > epicOdds)
            WinRarePrize();
        else if (playerRoll <= epicOdds && playerRoll > legendaryOdds)
            WinEpicPrize();
        else if (playerRoll <= legendaryOdds)
            WinLegendaryPrize();
    }

    void WinCommonPrize()
    {
        Item prize = commonPrizePool[Random.Range(0, commonPrizePool.Count)];

        if (inventory.itemsInInventory.Count <= PlayerInventory.inventoryCapacity)
        {
            inventory.itemsInInventory.Add(prize);
            inventoryItemDetails.itemDetails = prize;
            inventoryItemDetails.itemSlot = inventory.itemsInInventory.Count - 1;
            inventoryItemDetails.isNew = true;
            Instantiate(inventoryItem, playerItems.transform);
        }

        // if (inventory is full), then add it to archive. Tell the player it went to the archive.

        // Play common prize animation.

        // If prize is a one-off upgrade, remove it from the prize pool:
        if (prize.isAnUpgrade == true)
            commonPrizePool.Remove(prize);

        // Increase odds for rarer prize next spin:
        rareOdds++;
        epicOdds++;
        legendaryOdds++;
    }

    void WinRarePrize()
    {
        Item prize = rarePrizePool[Random.Range(0, rarePrizePool.Count)];

        if (inventory.itemsInInventory.Count <= PlayerInventory.inventoryCapacity)
        {
            inventory.itemsInInventory.Add(prize);
            inventoryItemDetails.itemDetails = prize;
            inventoryItemDetails.itemSlot = inventory.itemsInInventory.Count - 1;
            inventoryItemDetails.isNew = true;
            Instantiate(inventoryItem, playerItems.transform);
        }

        // if (inventory is full), then add it to archive. Tell the player it went to the archive.

        // Play rare prize animation.

        // If prize is a one-off upgrade, remove it from the prize pool:
        if (prize.isAnUpgrade == true)
            rarePrizePool.Remove(prize);
    }

    void WinEpicPrize()
    {
        Item prize = epicPrizePool[Random.Range(0, epicPrizePool.Count)];

        if (inventory.itemsInInventory.Count <= PlayerInventory.inventoryCapacity)
        {
            inventory.itemsInInventory.Add(prize);
            inventoryItemDetails.itemDetails = prize;
            inventoryItemDetails.itemSlot = inventory.itemsInInventory.Count - 1;
            inventoryItemDetails.isNew = true;
            Instantiate(inventoryItem, playerItems.transform);
        }

        // if (inventory is full), then add it to archive. Tell the player it went to the archive.

        // Play epic prize animation.

        // If prize is a one-off upgrade, remove it from the prize pool:
        if (prize.isAnUpgrade == true)
            epicPrizePool.Remove(prize);
    }

    void WinLegendaryPrize()
    {
        Item prize = legendaryPrizePool[Random.Range(0, legendaryPrizePool.Count)];

        if (inventory.itemsInInventory.Count <= PlayerInventory.inventoryCapacity)
        {
            inventory.itemsInInventory.Add(prize);
            inventoryItemDetails.itemDetails = prize;
            inventoryItemDetails.itemSlot = inventory.itemsInInventory.Count - 1;
            inventoryItemDetails.isNew = true;
            Instantiate(inventoryItem, playerItems.transform);
        }

        // if (inventory is full), then add it to archive. Tell the player it went to the archive.

        // Play legendary prize animation.

        // If prize is a one-off upgrade, remove it from the prize pool:
        if (prize.isAnUpgrade == true)
            legendaryPrizePool.Remove(prize);

        rareOdds = 30;
        epicOdds = 10;
        legendaryOdds = 1;
    }
}
