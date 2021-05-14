using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTest : MonoBehaviour
{
    public GameObject continueArrow;
    [TextArea(3, 3)] public string[] allDialogue;
    public bool isTypingOut;
    public float typeOutSpeed = 0.04f;

    private int dialogueIndex;

    private TextMeshPro activeDialogue;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        activeDialogue = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (gameManager.dialogueIsActive) // Have something set dialogue to active, dummy.
        {
            if (isTypingOut)
                continueArrow.gameObject.SetActive(false);
            else if (!isTypingOut)
                continueArrow.gameObject.SetActive(true);
        }
    }

    IEnumerator TypeOutRoutine()
    {
        isTypingOut = true;

        foreach (char letter in allDialogue[dialogueIndex].ToCharArray())
        {
            activeDialogue.text += letter;
            yield return new WaitForSeconds(typeOutSpeed);
        }

        isTypingOut = false;
        if (typeOutSpeed == 0)
            typeOutSpeed = 0.04f;
    }

    public void TriggerDialogue()
    {
        StartCoroutine(TypeOutRoutine());
    }

    public void NextLineOfDialogue()
    {
        if (dialogueIndex < allDialogue.Length - 1)
        {
            dialogueIndex++; // Adding to index for array.
            activeDialogue.text = ""; // Clearing out last line of dialogue.
            StartCoroutine(TypeOutRoutine()); // Running next line of dialogue.
        }
    }

    public void ResetDialogue()
    {
        activeDialogue.text = "";
        dialogueIndex = 0;
    }
}
