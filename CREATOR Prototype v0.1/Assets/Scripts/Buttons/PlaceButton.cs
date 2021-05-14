using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlaceButton : MonoBehaviour
{
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    private StudioEditorManager studioEditor;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private Collider2D thisCollider;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        studioEditor = GameObject.Find("Studio Editor Manager").GetComponent<StudioEditorManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0) && (thisCollider == gameManager.overlapPoint) && !studioEditor.cannotPlace)
            studioEditor.PlaceItem();

        if (!studioEditor.cannotPlace)
        {
            spriteRenderer.sprite = activeSprite;
            spriteRenderer.color = Color.white;
        }

        if (studioEditor.cannotPlace)
        {
            spriteRenderer.sprite = inactiveSprite;
            spriteRenderer.color = StudioEditorManager.semitransparent;
        }
    }
}
