using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceInventoryButton : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerInventory inventory;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        inventory = GameObject.Find("Player Inventory").GetComponent<PlayerInventory>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint))
            inventory.PlaceInventoryItem();
    }
}
