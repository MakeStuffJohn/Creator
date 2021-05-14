using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tenne : MonoBehaviour
{
    public GameObject tenneHeroSprite;
    [SerializeField] private Sprite tenneDefaultHero;
    [SerializeField] private Sprite tenneHappyHero;
    [SerializeField] private Sprite tenneShockedHero;
    [SerializeField] private Sprite tenneSkepticalHero;
    [SerializeField] private Sprite tenneSadHero;

    public bool hasImportantDialogue;
    public bool hasDailyDialogue;

    private GameManager gameManager;
    private DialogueManager dialogueManager;
    private TenneDialogue tenneDialogue;
    private SpriteRenderer heroSpriteRenderer;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        dialogueManager = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
        tenneDialogue = GameObject.Find("Tenne Dialogue").GetComponent<TenneDialogue>();
        heroSpriteRenderer = tenneHeroSprite.GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();

        DialogueManager.onNewLineOfDialogue += SetHeroSprite;

        // Set hasDailyDialogue to true if it's a new day.
        // (Create a date var that updates to the current day when the daily dialogue is said, then check it against today's date on awake to see if it's a new day.)
        // ((But how will it retain that variable when it's asleep? Maybe put it in the Game Manager.))
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint))
            tenneDialogue.DetermineDialogue();

        if (!gameManager.dialogueIsActive && tenneHeroSprite.activeSelf)
            tenneHeroSprite.SetActive(false);
    }

    void SetHeroSprite(CollabReaction heroSprite)
    {
        if (dialogueManager.activeCollaborator == this.gameObject)
        {
            if (!tenneHeroSprite.activeSelf)
                tenneHeroSprite.SetActive(true);

            switch (heroSprite)
            {
                case CollabReaction.Default:
                    heroSpriteRenderer.sprite = tenneDefaultHero;
                    break;
                case CollabReaction.Happy:
                    heroSpriteRenderer.sprite = tenneHappyHero;
                    break;
                case CollabReaction.Shocked:
                    heroSpriteRenderer.sprite = tenneShockedHero;
                    break;
                case CollabReaction.Skeptical:
                    heroSpriteRenderer.sprite = tenneSkepticalHero;
                    break;
                case CollabReaction.Sad:
                    heroSpriteRenderer.sprite = tenneSadHero;
                    break;
                // Add custom reaction sprites here!
            }
        }
    }
}
