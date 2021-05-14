using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotateButton : MonoBehaviour
{
    public static Action onItemRotate;

    private GameManager gameManager;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint) && (onItemRotate != null))
            onItemRotate();
    }
}
