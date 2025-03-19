using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;  // Used to change the tile color
    private Color originalColor;
    private BattleManager battleManager;
    private Vector2 tilePosition;

    public Color hoverColor = Color.yellow; // Color when hovered

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the tile's renderer
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Store the original color
        }
    }

    public void Init(BattleManager manager, Vector2 position)
    {
        battleManager = manager;
        tilePosition = position;
    }

    void OnMouseEnter()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor; // Change color when hovering
        }
    }

    void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // Reset color when exiting
        }
    }

    void OnMouseDown()
    {
        battleManager.OnTileClicked(tilePosition);
    }
}
