using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorItemCollider : MonoBehaviour
{
    private StudioEditorManager studioEditor;
    private SmallFloorItemCollider smallFloorItemCollider;

    void Awake()
    {
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Floor Collider") || other.gameObject.CompareTag("Floor Item Collider")) && studioEditor.studioEditorActive)
            studioEditor.cannotPlace = true; // On collision with wall, door, or another floor item's collider, you cannot place item.
        else if (other.gameObject.CompareTag("Small Floor Item Collider"))
        {
            smallFloorItemCollider = other.gameObject.GetComponent<SmallFloorItemCollider>();
            StartCoroutine(CheckSmallFloorItemSurfaceRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Floor Collider") || other.gameObject.CompareTag("Floor Item Collider"))
            studioEditor.cannotPlace = false;
        else if (other.gameObject.CompareTag("Small Floor Item Collider"))
        {
            smallFloorItemCollider = other.gameObject.GetComponent<SmallFloorItemCollider>();
            if (!smallFloorItemCollider.isOnSurface)
                studioEditor.cannotPlace = false;
        }
    }

    IEnumerator CheckSmallFloorItemSurfaceRoutine()
    {
        yield return new WaitForEndOfFrame();
        if (!smallFloorItemCollider.isOnSurface)
            studioEditor.cannotPlace = true;
        // Make this work better later!
    }
}
