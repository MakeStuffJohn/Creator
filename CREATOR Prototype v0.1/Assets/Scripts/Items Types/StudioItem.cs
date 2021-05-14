using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StudioItem : MonoBehaviour
{
    public int itemID;
    public Sprite defaultSprite;
    public GameObject sourceInventoryItem;
    public bool wasPlacedFromInventory;
}
