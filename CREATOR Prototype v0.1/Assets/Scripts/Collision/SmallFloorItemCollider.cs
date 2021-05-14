using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SmallFloorItemCollider : MonoBehaviour
{
    public bool isOnSurface;

    private Vector2 surfaceHeight;
    private int surfaceSortingOrder;
    private float bottomOfSurfaceItem;
    private bool isOnEdge;
    private bool isLoweredFromSurface;

    private GameObject smallFloorItem;
    private SmallFloorItemBehavior smallFloorItemDetails;
    private StudioEditorManager studioEditor;
    private SurfaceCollider surfaceCollider;

    void Awake()
    {
        smallFloorItem = transform.parent.parent.parent.gameObject;
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        smallFloorItemDetails = smallFloorItem.GetComponent<SmallFloorItemBehavior>();
    }

    void Update()
    {
        if (studioEditor.studioEditorActive)
            isOnSurface = smallFloorItemDetails.isOnSurface;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Edge Collider") && studioEditor.studioEditorActive)
        {
            isOnEdge = true;
            studioEditor.cannotPlace = true;
        }
        else if (other.gameObject.CompareTag("Surface Collider") && !isOnEdge && studioEditor.studioEditorActive)
        {
            surfaceCollider = other.gameObject.GetComponent<SurfaceCollider>();

            surfaceHeight = surfaceCollider.surfaceHeight;
            smallFloorItemDetails.currentSurfaceHeight = surfaceHeight;

            surfaceSortingOrder = surfaceCollider.surfaceSortingOrder;
            smallFloorItemDetails.currentSurfaceSortingOrder = surfaceSortingOrder;

            bottomOfSurfaceItem = surfaceCollider.bottomOfTable;
            smallFloorItemDetails.bottomOfSurfaceItem = bottomOfSurfaceItem;

            if (!isLoweredFromSurface)
            {
                Vector2 currentPos = transform.position;
                transform.position = currentPos - surfaceHeight;
                isLoweredFromSurface = true;
            }
            smallFloorItemDetails.isOnSurface = true;
        }
        else if (other.gameObject.CompareTag("Floor Collider") && studioEditor.studioEditorActive)
                studioEditor.cannotPlace = true;
        else if (other.gameObject.CompareTag("Small Floor Item Collider") && studioEditor.studioEditorActive)
            studioEditor.cannotPlace = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Edge Collider"))
        {
            isOnEdge = false;
            studioEditor.cannotPlace = false;
        }
        else if (other.gameObject.CompareTag("Surface Collider"))
        {
            if (isLoweredFromSurface)
            {
                Vector2 currentPos = transform.position;
                transform.position = currentPos + surfaceHeight;
                isLoweredFromSurface = false;
            }
            smallFloorItemDetails.isOnSurface = false;
            surfaceHeight = new Vector2(0, 0);
        }
        else if (other.gameObject.CompareTag("Floor Collider") || other.gameObject.CompareTag("Small Floor Item Collider"))
            studioEditor.cannotPlace = false;
    }
}
