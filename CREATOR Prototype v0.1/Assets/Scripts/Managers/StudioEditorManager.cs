using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemDirection
{
    Front,
    Left,
    Back,
    Right,
    Null
}

[System.Serializable]
public class StudioEditorManager : MonoBehaviour
{
    public GameObject cancelButton;
    public GameObject noSymbol;
    public GameObject rotateButton;
    public GameObject placeButton;

    public bool studioEditorActive;
    public bool cannotPlace;
    public int itemEditorsInUse = 0;
    public GameObject activeItem;
    public StudioItem activeItemDetails;

    public static float studioBounds = 6.5f;
    public static Color semitransparent = new Color(1, 1, 1, 0.5f);

    public void ItemEditorButtonFollow(GameObject item, float itemHeight, string type)
    {
        float placeButtonOffset = -0.25f;
        float additionalPlaceButtonOffset = 0;
        Vector3 cancelButtonOffset = new Vector3(1, 0, 0);
        Vector3 rotateButtonOffset = new Vector3(-1, 0, 0);
        float topOfItem = itemHeight + placeButtonOffset;

        if (type == "Wall Item")
            additionalPlaceButtonOffset = -0.5f; // If the item is on the wall, rearanges buttons to keep them centered without the rotate button (since you can't rotate wall items). 

        Vector3 placeButtonPos = new Vector3((item.transform.position.x + additionalPlaceButtonOffset), (item.transform.position.y + topOfItem), item.transform.position.z - 2);

        placeButton.transform.position = placeButtonPos; // Keep place button above selected item.

        if ((item.transform.position.y + topOfItem) > (studioBounds - 0.5f))
        {
            // If item is towards the top of the screen, moves the buttons to below the item.
            placeButtonPos.y = item.transform.position.y + -topOfItem;
            placeButton.transform.position = placeButtonPos;
        }

        // The other buttons' positions are determined by the place button's position.
        cancelButton.transform.position = placeButtonPos + cancelButtonOffset;
        if (rotateButton)
            rotateButton.transform.position = placeButtonPos + rotateButtonOffset;
    }

    public void NoSymbolFollow(GameObject item)
    {
        noSymbol.transform.position = item.transform.position; // Keep no-symbol over top of selected item.
    }

    public void PlaceItem()
    {
        // Deactivates button UI and sets items in use back to 0 (triggering the rest of the placement code in the item's behavior script).
        itemEditorsInUse--;
        cancelButton.SetActive(false);
        placeButton.SetActive(false);
        if (rotateButton)
            rotateButton.SetActive(false);
    }
}
