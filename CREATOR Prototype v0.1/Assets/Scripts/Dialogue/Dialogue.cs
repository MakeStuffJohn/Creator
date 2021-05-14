using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* public enum DialoguePriority
{
    Basic,
    Situational,
    Quest,
    Critical
} */

public enum CollabReaction
{
    Default,
    Happy,
    Shocked,
    Skeptical,
    Sad,
    Custom
}

public enum ReactionFX
{
    None,
    Love,
    Shock,
    Confusion,
    Nervous,
    Angry,
    Defeated,
    Success
}

public enum PlayerReaction
{
    None,
    Love,
    Shock,
    Confusion,
    Nervous,
    Angry,
    Defeated,
    Success
}

[System.Serializable]
public class Dialogue
{
    public string dialogueName;
    public int dialogueOrder;
    // public DialoguePriority dialoguePriority;
    [TextArea(4, 4)] public string[] dialogueArray;
    public CollabReaction[] collabReactions;
    public ReactionFX[] reactionFX;
    public PlayerReaction[] playerReactions;

    // If player gets to respond at the end of the dialogue entry:
    public bool playerResponds;
    [TextArea(2, 2)] public string[] playerResponseOptions;
    public int[] playerResponseID;

    public bool hasBeenSaid;
    public bool isRelevant;
}
