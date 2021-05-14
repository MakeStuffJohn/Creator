using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StudioEditorButton : MonoBehaviour
{
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    private SpriteRenderer spriteRenderer;
    private Collider2D thisCollider;
    private StudioEditorManager studioEditor;
    private GameManager gameManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint))
            StudioEditorMode();

        if (!studioEditor.studioEditorActive && spriteRenderer.sprite == activeSprite)
            spriteRenderer.sprite = inactiveSprite; // Delete this bit if it proves unnecessary!

        if (studioEditor.studioEditorActive && spriteRenderer.sprite == inactiveSprite)
            spriteRenderer.sprite = activeSprite;
    }

    void StudioEditorMode()
    {
        // Activates Studio Editor and changes button sprite to "Exit Editor".
        if (studioEditor.studioEditorActive == false)
        {
            studioEditor.studioEditorActive = true;
            spriteRenderer.sprite = activeSprite;
        }

        // Deactivates Studio Editor and changes button back to "Edit Studio" sprite.
        else if (studioEditor.studioEditorActive == true)
        {
            studioEditor.studioEditorActive = false;
            spriteRenderer.sprite = inactiveSprite;
        }
    }
}
