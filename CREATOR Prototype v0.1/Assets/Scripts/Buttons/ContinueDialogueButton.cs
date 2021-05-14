using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueDialogueButton : MonoBehaviour
{
    private GameManager gameManager;
    private DialogueManager dialogueManager;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint))
            dialogueManager.NextLineOfDialogue();
    }
}
