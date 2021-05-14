using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableBehavior : MonoBehaviour
{
    public Table tableDetails;

    private FloorItemBehavior floorItem;

    void Awake()
    {
        floorItem = GetComponent<FloorItemBehavior>();
    }

    void Start()
    {
        CheckSurfaceColliderRotation();
    }

    void Update()
    {
        if (floorItem.itemEditorActive)
        {
            CheckSurfaceColliderRotation();
            Debug.Log("Item Editor Active!!!!!!!");
        }
    }

    void CheckSurfaceColliderRotation()
    {
        switch (floorItem.itemRotation)
        {
            case ItemDirection.Front:
                tableDetails.rotatedSurfaceCollider.SetActive(false);
                tableDetails.rightEdge.SetActive(false);

                tableDetails.defaultSurfaceCollider.SetActive(true);
                tableDetails.frontEdge.SetActive(true);
                break;
            case ItemDirection.Left:
                tableDetails.defaultSurfaceCollider.SetActive(false);
                tableDetails.frontEdge.SetActive(false);

                tableDetails.rotatedSurfaceCollider.SetActive(true);
                tableDetails.leftEdge.SetActive(true);
                break;
            case ItemDirection.Back:
                tableDetails.rotatedSurfaceCollider.SetActive(false);
                tableDetails.leftEdge.SetActive(false);

                tableDetails.defaultSurfaceCollider.SetActive(true);
                tableDetails.backEdge.SetActive(true);
                break;
            case ItemDirection.Right:
                tableDetails.defaultSurfaceCollider.SetActive(false);
                tableDetails.backEdge.SetActive(false);

                tableDetails.rotatedSurfaceCollider.SetActive(true);
                tableDetails.rightEdge.SetActive(true);
                break;
            default:
                break;
        }
    }
}
