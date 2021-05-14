﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static Action<CollabReaction> onNewLineOfDialogue;

    public GameObject dialogueBox;
    public GameObject dialogueText;
    public GameObject continueArrow;
    public GameObject playerResponse1;
    public GameObject playerResponse2;
    public GameObject playerResponse3;
    public bool isTypingOut;
    public bool autoSkipText;
    public bool playerIsResponding;
    public float typeOutSpeed = 0.04f;

    public Dialogue activeDialogue;
    public GameObject activeCollaborator;

    public static int textLine = 34;

    private int index;
    private int id;
    private string[] dialogueArray;
    private List<char> parsedDialogue = new List<char>();
    private Vector2 activePos = new Vector2(0, 0);
    private Vector2 inactivePos = new Vector2(0, -20);
    private Vector2 response2DefaultPos = new Vector2(0, -5.3f);
    private Vector2 response2SecondPos = new Vector2(0, -6.1f);
    private Vector2 response2ThirdPos = new Vector2(0, -6.9f);
    private Vector2 response3DefaultPos = new Vector2(0, -6.1f);
    private Vector2 response3SecondPos = new Vector2(0, -6.9f);
    private Vector2 response3ThirdPos = new Vector2(0, -7.7f);
    private float defaultTypeSpeed = 0.04f;
    private float slowTypeSpeed = 0.25f;
    private float fastTypeSpeed = 0.02f;
    // private float defaultFontSize = 8;
    private float smallFont = 4;
    private float bigFont = 12;
    private float boldKerning = -0.55f;

    private GameManager gameManager;
    private ItemDatabase itemDatabase;
    private TextMeshPro currentText;
    private TextMeshPro response1;
    private TextMeshPro response2;
    private TextMeshPro response3;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        itemDatabase = GameObject.Find("Item Database").GetComponent<ItemDatabase>();
        currentText = dialogueText.GetComponent<TextMeshPro>();
        response1 = playerResponse1.GetComponent<TextMeshPro>();
        response2 = playerResponse2.GetComponent<TextMeshPro>();
        response3 = playerResponse3.GetComponent<TextMeshPro>();

        PlayerResponseButton.onPlayerResponse += PlayerResponseChosen;

        transform.position = inactivePos;
        dialogueText = null;
    }

    void Update()
    {
        if (gameManager.dialogueIsActive) // Have something set dialogue to active, dummy.
        {
            if (isTypingOut || playerIsResponding)
                continueArrow.gameObject.SetActive(false);
            else
                continueArrow.gameObject.SetActive(true);
        }
    }

    IEnumerator TypeOutRoutine()
    {
        bool isRichText = false;
        string richText = "";

        ParseWords();

        isTypingOut = true;
        // Cue speaking stem in music.

        foreach (char letter in parsedDialogue)
        {
            if (letter == '<')
               isRichText = true;
            else if (letter == '>')
                isRichText = false;
            
            if (!isRichText)
            {
                if (richText != "")
                {
                    richText += letter;
                    RichText(richText);
                    richText = ""; // Then clear the richText.
                }
                else
                    currentText.text += letter;

                yield return new WaitForSeconds(typeOutSpeed);
            }
            else if (isRichText)
            {
                richText += letter;
            }
        }

        isTypingOut = false;
        if (typeOutSpeed != defaultTypeSpeed)
        {
            typeOutSpeed = defaultTypeSpeed;
            // Reset speaking stem speed.
        }
        // Turn off speaking stem in music.

        if (autoSkipText)
        {
            NextLineOfDialogue();
            autoSkipText = false;
        }
    }

    public void StartDialogue(GameObject collab, Dialogue dialogue)
    {
        index = 0;
        currentText.text = "";
        
        dialogueArray = dialogue.dialogueArray;
        activeCollaborator = collab;
        activeDialogue = dialogue;

        gameManager.dialogueIsActive = true;

        if (onNewLineOfDialogue != null)
        {
            onNewLineOfDialogue(activeDialogue.collabReactions[index]);
        }

        transform.position = activePos;
        // Play reaction FX with switch statement. (Check if it's != ReactionFX.None first.)
        // Play player reaction FX with switch statement.

        StartCoroutine(TypeOutRoutine());
    }

    public void NextLineOfDialogue()
    {
        if (index < dialogueArray.Length - 1)
        {
            index++; // Adds to index to get the next line in the array.
            currentText.text = ""; // Clearing out last line of dialogue.
            
            if (onNewLineOfDialogue != null)
            {
                onNewLineOfDialogue(activeDialogue.collabReactions[index]); // Updating sprite reactions.
            }

            StartCoroutine(TypeOutRoutine()); // Running next line of dialogue.
        }
        else if (activeDialogue.playerResponds)
        {
            currentText.text = "";
            ActivatePlayerResponses();
        }
        else
        {
            gameManager.dialogueIsActive = false;
            transform.position = inactivePos;
        }
    }

    void RichText(string richText)
    {
        switch (richText)
        {
            // Custom rich texts:
            case "</n>": // Add line break.
                currentText.text += "\r\n";
                break;
            case "<b>": // Bold.
                currentText.text += "<b>";
                currentText.text += "<cspace=" + boldKerning + ">";
                break;
            case "</b>": // Turn off bold.
                currentText.text += "</b>";
                currentText.text += "</cspace>";
                break;
            case "<sc>": // Small caps.
                currentText.text += "<smallcaps>";
                break;
            case "</sc>": // Turn off small caps.
                currentText.text += "</smallcaps>";
                break;
            case "<slow>": // Slow down typing speed.
                typeOutSpeed = slowTypeSpeed;
                // Slow speaking stem by 100%.
                break;
            case "</slow>": // Reset typing speed.
                typeOutSpeed = defaultTypeSpeed;
                // Reset speaking stem speed.
                break;
            case "<fast>": // Speed up typing speed.
                typeOutSpeed = fastTypeSpeed;
                // Speed up speaking stem by 100%.
                break;
            case "</fast>": // Reset typing speed.
                typeOutSpeed = defaultTypeSpeed;
                // Reset speaking stem speed.
                break;
            case "<skip>": // Auto-skips text.
                autoSkipText = true;
                break;

            // Color-changing rich texts:
            case "<red>":
                currentText.text += "<color=red>";
                break;
            case "<olive>":
                currentText.text += "<color=#808000ff>";
                break;
            case string s when (s == "</red>" || s == "</olive>"):
                currentText.text += "</color>";
                break;

            // Size-changing rich texts:
            case "<big>":
                currentText.text += "<size=" + bigFont + ">";
                break;
            case "<small>":
                currentText.text += "<size=" + smallFont + ">";
                break;
            case string s when (s == "</big>" || s == "</small>"):
                currentText.text += "</size>";
                break;

            // ID-based rich texts:
            case string s when (s[1] == '1'):
                id = ConvertIDToInt(richText);
                break;
            case "<item>":
                itemDatabase.AddItem(id, true);
                id = 0;
                break;
            case "<quest>":
                // AddQuest();
                id = 0;
                break;
            case "<anim>":
                // CueAnim();
                id = 0;
                break;

            // Actual rich text:
            default:
                currentText.text += richText;
                break;
        }
    }

    void ParseWords()
    {
        int currentIndex = 0;
        int startOfWord = 0;
        int trueStartOfWord = 0;
        int endOfWord = 0;
        bool isRichText = false;
        string richText = "";

        parsedDialogue.Clear();

        parsedDialogue = dialogueArray[index].ToList();

        for (int i = 0; i < parsedDialogue.Count; i++)
        {
            char letter = parsedDialogue[i];

            if (letter == '<')
            {
                isRichText = true;
                richText += letter;
            }
            else if (letter == '>')
            {
                isRichText = false;
                richText += letter;
                
                if (richText == "</n>")
                {
                    startOfWord = 0;
                    endOfWord = 0;
                }
                else if (richText == "<big>")
                {
                    // Adjust for bigger font.
                }
                else if (richText == "<small>")
                {
                    // Adjust for smaller font.
                }

                richText = "";
            }
            else if (((letter == ' ' || letter == '-' || letter == '/') && !isRichText) || i == parsedDialogue.Count - 1)
            {
                // Resolve last word:
                if (endOfWord >= textLine - 1)
                {
                    // Add line break:
                    parsedDialogue.Insert((trueStartOfWord + 1), '<');
                    parsedDialogue.Insert((trueStartOfWord + 2), '/');
                    parsedDialogue.Insert((trueStartOfWord + 3), 'n');
                    parsedDialogue.Insert((trueStartOfWord + 4), '>');
                    i += 4;

                    int wordLength = (endOfWord - startOfWord) - 1;
                    currentIndex = wordLength;
                }

                // Then set up new word:
                startOfWord = currentIndex;
                trueStartOfWord = i;
                currentIndex++;
            }
            else if (!isRichText)
            {
                endOfWord = currentIndex;
                currentIndex++;
            }
            else if (isRichText)
            {
                richText += letter;
            }
        }
    }

    void ActivatePlayerResponses()
    {
        playerIsResponding = true;

        switch (activeDialogue.playerResponseOptions.Length)
        {
            case 1:
                playerResponse1.SetActive(true);
                response1.text = activeDialogue.playerResponseOptions[0];
                break;
            case 2:
                playerResponse1.SetActive(true);
                playerResponse2.SetActive(true);
                response1.text = activeDialogue.playerResponseOptions[0];
                response2.text = activeDialogue.playerResponseOptions[1];
                SetResponseTextPosition();
                break;
            case 3:
                playerResponse1.SetActive(true);
                playerResponse2.SetActive(true);
                playerResponse3.SetActive(true);
                response1.text = activeDialogue.playerResponseOptions[0];
                response2.text = activeDialogue.playerResponseOptions[1];
                response3.text = activeDialogue.playerResponseOptions[2];
                SetResponseTextPosition();
                break;
        }
    }

    void SetResponseTextPosition()
    {
        int response1Length = response1.text.Length;
        int response2Length = response2.text.Length;
        bool pos1 = false;
        bool pos2 = false;
        bool pos3 = false;

        if (response1Length < textLine)
        {
            pos1 = true;
        }
        else if (response1Length >= textLine && response1Length < (textLine * 2))
        {
            playerResponse2.transform.position = response2SecondPos;
            pos2 = true;
        }
        else if (response1Length >= (textLine * 2))
        {
            playerResponse2.transform.position = response2ThirdPos;
            pos3 = true;
        }

        if (activeDialogue.playerResponseOptions.Length == 3)
        {
            if (response2Length < textLine)
            {
                if (pos1)
                    playerResponse3.transform.position = response3DefaultPos;
                else if (pos2)
                    playerResponse3.transform.position = response3SecondPos;
                else if (pos3)
                    playerResponse3.transform.position = response3ThirdPos;
            }
            else if (response2Length >= textLine && response2Length < (textLine * 2))
            {
                if (pos1)
                    playerResponse3.transform.position = response3SecondPos;
                else if (pos2)
                    playerResponse3.transform.position = response3ThirdPos;
            }
            else if (response2Length >= (textLine * 2) && pos1)
            {
                playerResponse3.transform.position = response3ThirdPos;
            }
        }
    }

    void PlayerResponseChosen(int response)
    {
        // Reset response text box positions:
        playerResponse2.transform.position = response2DefaultPos;
        playerResponse3.transform.position = response3DefaultPos;

        //Reset response text:
        response1.text = "";
        response2.text = "";
        response3.text = "";

        //Set response text boxes to inactive:
        playerResponse1.SetActive(false);
        playerResponse2.SetActive(false);
        playerResponse3.SetActive(false);

        playerIsResponding = false;
    }

    int ConvertIDToInt(string id)
    {
        float idNum = 1000000;
        int n = 0;

        for (int i = 7; i > 2; i--)
        {
            switch (id[i])
            {
                case '1':
                    idNum += 1 * (Mathf.Pow(10, n));
                    break;
                case '2':
                    idNum += 2 * (Mathf.Pow(10, n));
                    break;
                case '3':
                    idNum += 3 * (Mathf.Pow(10, n));
                    break;
                case '4':
                    idNum += 4 * (Mathf.Pow(10, n));
                    break;
                case '5':
                    idNum += 5 * (Mathf.Pow(10, n));
                    break;
                case '6':
                    idNum += 6 * (Mathf.Pow(10, n));
                    break;
                case '7':
                    idNum += 7 * (Mathf.Pow(10, n));
                    break;
                case '8':
                    idNum += 8 * (Mathf.Pow(10, n));
                    break;
                case '9':
                    idNum += 9 * (Mathf.Pow(10, n));
                    break;
                default:
                    break;
            }
            n++;
        } 
        return (int) idNum;
    }
}
