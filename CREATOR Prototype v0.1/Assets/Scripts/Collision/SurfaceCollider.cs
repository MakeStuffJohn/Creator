using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceCollider : MonoBehaviour
{
    public Vector3 surfaceHeight;
    public int surfaceSortingOrder;
    public float bottomOfTable;

    private StudioEditorManager studioEditor;
    private GameObject parentTable;
    private TableBehavior tableDetails;
    private SpriteRenderer tableSpriteRenderer;

    void Awake()
    {
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        parentTable = transform.parent.parent.parent.gameObject;
        tableDetails = parentTable.gameObject.GetComponent<TableBehavior>();
        tableSpriteRenderer = parentTable.gameObject.GetComponent<SpriteRenderer>();
        surfaceHeight = tableDetails.tableDetails.surfaceHeight;
    }

    void Update()
    {
        if (studioEditor.studioEditorActive)
        {
            surfaceSortingOrder = tableSpriteRenderer.sortingOrder;
            bottomOfTable = parentTable.transform.position.y - (tableSpriteRenderer.bounds.size.y / 2);
        }
    }
}
