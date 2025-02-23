using UnityEngine;

public class CharacterTileClickHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;  // Used to change the tile color
    private Color originalColor;
    private CharacterManager characterManager;
    private int tilePosition;
    private bool inBattle;


    public Color hoverColor = Color.blue; // Color when hovered

    void Start()
    {   
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the tile's renderer
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // Store the original color
        }
    }

    public void Init(CharacterManager manager, int index)
    {
        characterManager=manager;
        tilePosition = index;
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
        if(characterManager.CheckBattle()==false){
            characterManager.GoToCharacterSelection(tilePosition);
        }else if(characterManager.CheckBattle()==true){
            BattleManager battleManager=GameObject.Find("BattleManager").GetComponent<BattleManager>();
            Debug.Log($"Tile clicked at position: {tilePosition}");
            battleManager.SelectCharacter(tilePosition);
        }
        
    }
}
