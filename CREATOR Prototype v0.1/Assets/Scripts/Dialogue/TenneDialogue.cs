using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TenneDialogue : MonoBehaviour
{
    public List<Dialogue> tenneCrit = new List<Dialogue>();
    public List<Dialogue> tenneQuest = new List<Dialogue>();
    public List<Dialogue> tenneSitch = new List<Dialogue>();
    public List<Dialogue> tenneBasic = new List<Dialogue>();

    private int determinedDialogue;
    private int empty = -1;
    private List<Dialogue> possibleDialogues = new List<Dialogue>();
    private List<int> possibleDialogueIndex = new List<int>();
    private bool isCrit;
    private bool isQuest;
    private bool isSitch;
    private bool isBasic;

    // CRITICAL dialogue entries:
    private int introduction;
    private int introRespo01;
    private int introRespo02;
    private int introRespo03;

    // QUEST dialogue entries:
    private int cactusQuestIntro;
    private int cactusQuestHint;
    private int cactusQuestComplete;

    // SITUATIONAL dialogue entries:
    private int roseLionReaction;

    private DialogueManager dialogueManager;
    private GameObject studioItems;
    private GameObject tenne;
    private Tenne tenneDetails;

    private void Awake()
    {
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
        studioItems = GameObject.Find("Studio Items");
        tenne = transform.parent.gameObject;
        tenneDetails = tenne.GetComponent<Tenne>();

        determinedDialogue = empty;

        PlayerResponseButton.onPlayerResponse += PlayerResponseChosen;

        // Assign each dialogue entry to its correct array int reference:
        AssignDialogueVariables();

        CheckForImportantDialogue();
        RefreshUsedDialogue();

        // Linq check on basic and situational dialogues. If all Basic dialogues or all Sitch dialogues come back as hasBeenSaid, set those respective hasBeenSaids to false.
    }

    void AssignDialogueVariables()
    {
        foreach (Dialogue dialogue in tenneCrit)
        {
            if (dialogue.dialogueName == "Introduction")
                introduction = tenneCrit.IndexOf(dialogue);
            else if (dialogue.dialogueName == "Intro Respo 01")
                introRespo01 = tenneCrit.IndexOf(dialogue);
            else if (dialogue.dialogueName == "Intro Respo 02")
                introRespo02 = tenneCrit.IndexOf(dialogue);
            else if (dialogue.dialogueName == "Intro Respo 03")
                introRespo03 = tenneCrit.IndexOf(dialogue);
        }

        foreach (Dialogue dialogue in tenneQuest)
        {
            if (dialogue.dialogueName == "Cactus Quest Intro")
                cactusQuestIntro = tenneQuest.IndexOf(dialogue);
            else if (dialogue.dialogueName == "Cactus Quest Complete")
                cactusQuestComplete = tenneQuest.IndexOf(dialogue);
            else if (dialogue.dialogueName == "Cactus Quest Hint")
                cactusQuestHint = tenneQuest.IndexOf(dialogue);
        }

        foreach (Dialogue dialogue in tenneSitch)
        {
            if (dialogue.dialogueName == "Rose Lion Reaction")
                roseLionReaction = tenneSitch.IndexOf(dialogue);
        }
    }

    void CheckForImportantDialogue()
    {
        UpdateCritDialogueReqs();
        UpdateQuestDialogueReqs();

        bool relevantCritDialogue = tenneCrit.Any(dia => dia.isRelevant == true);
        bool relevantQuestDialogue = tenneQuest.Any(dia => dia.isRelevant == true);

        if (relevantCritDialogue || relevantQuestDialogue)
        {
            tenneDetails.hasImportantDialogue = true;
            // Add apropriate balloon over collab's head!
        }
        else if (!relevantCritDialogue && !relevantQuestDialogue)
        {
            tenneDetails.hasImportantDialogue = false;
            // Remove balloon over collab's head!
        }
    }

    void RefreshUsedDialogue()
    {
        var usedDialogue = tenneBasic.Where(dia => dia.hasBeenSaid == true);

        if (usedDialogue.Count() == tenneBasic.Count)
        {
            foreach (Dialogue dia in tenneBasic)
                dia.hasBeenSaid = false;
            foreach (Dialogue dia in tenneSitch)
                dia.hasBeenSaid = false;
        }
    }

    public void DetermineDialogue()
    {
        // Maybe create a bool that triggers if all the critical dialogue has been said so it doesn't have to cycle through them every time.

        // If there's any critical dialogue to say according to the Tenne script:
        CheckForCritDialogue();

        if (determinedDialogue != empty)
        {
            isCrit = true;
            SubmitDialogue();
        }
        else if (determinedDialogue == empty)
        {
            CheckForQuestDialogue();

            if (determinedDialogue != empty)
            {
                isQuest = true;
                SubmitDialogue();
            }
            else if (determinedDialogue == empty)
            {
                RefreshUsedDialogue();
                CheckForSitchDialogue();

                if (possibleDialogues.Count > 0)
                {
                    isSitch = true;
                    SubmitDialogue();
                }
                else if (possibleDialogues.Count <= 0)
                {
                    CheckForBasicDialogue();
                    isBasic = true;
                    SubmitDialogue();
                }
            }
            // Only let them get one situational or basic dialogue per day from each character.
            // It then tells the Tenne script not to allow more dialogue for the day.
        }
    }

    void UpdateCritDialogueReqs()
    {
        // Each critical dialogue entry checks if it's relevant:
        Introduction();
    }

    void UpdateQuestDialogueReqs()
    {
        CactusQuestIntro();
        CactusQuestComplete();
        CactusQuestHint();
    }

    void UpdateSitchDialogueReqs()
    {
        RoseLionReaction();
    }

    void CheckForCritDialogue()
    {
        UpdateCritDialogueReqs();

        int lowestDialogueOrder = 99999;

        for (int i = 0; i < tenneCrit.Count; i++)
        {
            // Checks if the dialogue is relevant and lower than the dialogue order of the numbers before it. If it is, it sets the determined dialogue as its index number:
            if (tenneCrit[i].isRelevant && tenneCrit[i].dialogueOrder < lowestDialogueOrder)
            {
                determinedDialogue = i;
                lowestDialogueOrder = tenneCrit[i].dialogueOrder;
            }
        }
    }

    void CheckForQuestDialogue()
    {
        UpdateQuestDialogueReqs();

        int lowestDialogueOrder = 99999;

        for (int i = 0; i < tenneQuest.Count; i++)
        {
            // Checks if the dialogue is relevant and lower than the dialogue order of the numbers before it. If it is, it sets the determined dialogue as its index number:
            if (tenneQuest[i].isRelevant && tenneQuest[i].dialogueOrder < lowestDialogueOrder)
            {
                determinedDialogue = i;
                lowestDialogueOrder = tenneQuest[i].dialogueOrder;
            }
        }
    }

    void CheckForSitchDialogue()
    {
        UpdateSitchDialogueReqs();

        foreach (Dialogue dialogue in tenneSitch)
        {
            if (dialogue.isRelevant)
            {
                possibleDialogues.Add(dialogue);
                int dialogueIndex = tenneSitch.IndexOf(dialogue);
                possibleDialogueIndex.Add(dialogueIndex);
            }
        }
    }

    void CheckForBasicDialogue()
    {
        foreach (Dialogue dialogue in tenneBasic)
        {
            if (!dialogue.hasBeenSaid)
            {
                possibleDialogues.Add(dialogue);
                int dialogueIndex = tenneBasic.IndexOf(dialogue);
                possibleDialogueIndex.Add(dialogueIndex);
            }
        }
    }

    void PlayerResponseChosen(int response)
    {
        if (dialogueManager.activeCollaborator == tenne)
        {
            int dia = 0;
            bool isCritRespo = false;
            bool isQuestRespo = false;
            bool isSitchRespo = false;
            bool isBasicRespo = false;

            // Maybe put this switch in another method:
            switch (response)
            {
                case 101101:
                    dia = introRespo01;
                    isCritRespo = true;
                    break;
                case 101102:
                    dia = introRespo02;
                    isCritRespo = true;
                    break;
                case 101103:
                    dia = introRespo03;
                    isCritRespo = true;
                    break;
            }

            // Submit response dialogue:
            if (isCritRespo)
                dialogueManager.StartDialogue(tenne, tenneCrit[dia]);
            else if (isQuestRespo)
                dialogueManager.StartDialogue(tenne, tenneQuest[dia]);
            else if (isSitchRespo)
                dialogueManager.StartDialogue(tenne, tenneSitch[dia]);
            else if (isBasicRespo)
                dialogueManager.StartDialogue(tenne, tenneBasic[dia]);
        }
    }

    void SubmitDialogue()
    {
        int randomDialogue = 0;
        int selectedRandomDialogue = 0;

        if (isCrit)
        {
            dialogueManager.StartDialogue(tenne, tenneCrit[determinedDialogue]);
            tenneCrit[determinedDialogue].hasBeenSaid = true;
            determinedDialogue = empty;
            isCrit = false;
        }
        else if (isQuest)
        {
            dialogueManager.StartDialogue(tenne, tenneQuest[determinedDialogue]);
            tenneQuest[determinedDialogue].hasBeenSaid = true;
            determinedDialogue = empty;
            isQuest = false;
        }
        else if (isSitch)
        {
            randomDialogue = Random.Range(0, possibleDialogues.Count);
            selectedRandomDialogue = possibleDialogueIndex[randomDialogue];

            dialogueManager.StartDialogue(tenne, possibleDialogues[randomDialogue]);
            tenneSitch[selectedRandomDialogue].hasBeenSaid = true;
            possibleDialogues.Clear();
            possibleDialogueIndex.Clear();
            isSitch = false;
        }
        else if (isBasic)
        {
            randomDialogue = Random.Range(0, possibleDialogues.Count);
            selectedRandomDialogue = possibleDialogueIndex[randomDialogue];

            dialogueManager.StartDialogue(tenne, possibleDialogues[randomDialogue]);
            tenneBasic[selectedRandomDialogue].hasBeenSaid = true;
            possibleDialogues.Clear();
            possibleDialogueIndex.Clear();
            isBasic = false;
        }

        CheckForImportantDialogue(); // Update whether there should be a balloon over the collab's head.
    }

    void Introduction()
    {
        Dialogue dia = tenneCrit[introduction];

        if (!dia.hasBeenSaid)
            tenneCrit[introduction].isRelevant = true;
        else 
            tenneCrit[introduction].isRelevant = false;
    }

    void CactusQuestIntro()
    {
        Dialogue dia = tenneQuest[cactusQuestIntro];

        if (!dia.hasBeenSaid)
            tenneQuest[cactusQuestIntro].isRelevant = true;
        else
            tenneQuest[cactusQuestIntro].isRelevant = false;
    }

    void CactusQuestHint()
    {
        Dialogue dia = tenneQuest[cactusQuestHint];
        Dialogue questComplete = tenneQuest[cactusQuestComplete];

        if (!questComplete.isRelevant && !questComplete.hasBeenSaid)
            tenneQuest[cactusQuestHint].isRelevant = true;
        else
            tenneQuest[cactusQuestHint].isRelevant = false;
    }

    void CactusQuestComplete()
    {
        Dialogue dia = tenneQuest[cactusQuestComplete];
        int cactusCuties = 0;

        // Go through each studio item and check if it's a cactus cutie:
        foreach (Transform studioItem in studioItems.transform)
        {
            StudioItem itemID = studioItem.GetComponent<StudioItem>();
            if (itemID.itemID == 1011902) // If Cactus Cutie, then add to the cactus cutie count.
                cactusCuties++;
        }

        if (!dia.hasBeenSaid && cactusCuties == 10)
            tenneQuest[cactusQuestComplete].isRelevant = true;
        else
            tenneQuest[cactusQuestComplete].isRelevant = false;
    }

    void RoseLionReaction()
    {
        Dialogue dia = tenneSitch[roseLionReaction];
        bool roseLionArtPrint = false;

        // Go through each studio item and check if it's a Rose-Lyon Art Print:
        foreach (Transform studioItem in studioItems.transform)
        {
            StudioItem itemID = studioItem.GetComponent<StudioItem>();
            if (itemID.itemID == 1011001)
            {
                roseLionArtPrint = true;
                break;
            }
        }

        if (!dia.hasBeenSaid && roseLionArtPrint)
            tenneSitch[roseLionReaction].isRelevant = true;
        else
            tenneSitch[roseLionReaction].isRelevant = false;
    }
}
