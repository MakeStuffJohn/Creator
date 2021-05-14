using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PlayerResponseButton : MonoBehaviour
{
    public static Action<int> onPlayerResponse;

    [SerializeField] private int responseNumber;
    private int chosenResponse;
    private int responseLength;
    private Color defaultColor = new Color(0.44f, 0.44f, 0.44f);
    private Color highlightColor = new Color(0.22f, 0.22f, 0.22f);
    private Vector2 oneLineOffset = new Vector2(0, 1.2f);
    private Vector2 oneLineSize = new Vector2(12, 0.5f);
    private Vector2 twoLineOffset = new Vector2(0, 0.8f);
    private Vector2 twoLineSize = new Vector2(12, 1.25f);
    private Vector2 threeLineOffset = new Vector2(0, 0.4f);
    private Vector2 threeLineSize = new Vector2(12, 2);
    private Vector2 fourLineOffset = new Vector2(0, 0);
    private Vector2 fourLineSize = new Vector2(12, 2.75f);

    private DialogueManager dialogueManager;
    private GameManager gameManager;
    private BoxCollider2D thisCollider;
    private TextMeshPro textMesh;

    void Awake()
    {
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        thisCollider = GetComponent<BoxCollider2D>();
        textMesh = GetComponent<TextMeshPro>();

        switch (responseNumber)
        {
            case 1:
                chosenResponse = dialogueManager.activeDialogue.playerResponseID[0];
                responseLength = dialogueManager.activeDialogue.playerResponseOptions[0].Length;
                break;
            case 2:
                chosenResponse = dialogueManager.activeDialogue.playerResponseID[1];
                responseLength = dialogueManager.activeDialogue.playerResponseOptions[1].Length;
                break;
            case 3:
                chosenResponse = dialogueManager.activeDialogue.playerResponseID[2];
                responseLength = dialogueManager.activeDialogue.playerResponseOptions[2].Length;
                break;
        }
    }

    void Start()
    {
        if (responseLength <= DialogueManager.textLine)
        {
            thisCollider.offset = oneLineOffset;
            thisCollider.size = oneLineSize;
        }
        else if (responseLength > DialogueManager.textLine && responseLength <= DialogueManager.textLine * 2)
        {
            thisCollider.offset = twoLineOffset;
            thisCollider.size = twoLineSize;
        }
        else if (responseLength > DialogueManager.textLine * 2 && responseLength <= DialogueManager.textLine * 3)
        {
            thisCollider.offset = threeLineOffset;
            thisCollider.size = threeLineSize;
        }
        else if (responseLength > DialogueManager.textLine * 3)
        {
            thisCollider.offset = fourLineOffset;
            thisCollider.size = fourLineSize;
        }
    }

    void LateUpdate()
    {
        if (thisCollider == gameManager.overlapPoint)
        {
            textMesh.color = highlightColor;

            if (Input.GetMouseButtonUp(0) && (onPlayerResponse != null))
                onPlayerResponse(chosenResponse);
        }
        else if (thisCollider != gameManager.overlapPoint && textMesh.color != defaultColor)
            textMesh.color = defaultColor;
    }
}
