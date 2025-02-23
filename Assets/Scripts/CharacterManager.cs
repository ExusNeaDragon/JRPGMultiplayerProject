using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    public GameObject gridTilePrefab;
    public Transform GridParent;
    
    public GameObject[] playerPrefabs=new GameObject[5]; // List of selectable player characters
    private GameObject selectedCharacter; // Current character to place
    private Vector2 characterGridOrigin;
    private Dictionary<Vector2, GameObject> occupiedTiles = new Dictionary<Vector2, GameObject>(); // Track occupied tiles
    private List<Vector2> GridPositions = new List<Vector2>();
    private bool inBattle;
    private int changingTileIndex;

    private int gridSizeX = 5;  
    private int gridSizeY = 1; 
    public float tileSpacing = 1f;

    void Start()
    {
        DontDestroyOnLoad(gameObject); 
        characterGridOrigin=new Vector2(-7,4.3f);
        GenerateCharacterGrid(GridParent, characterGridOrigin,GridPositions);
    }

    void GenerateCharacterGrid(Transform parent, Vector2 origin, List<Vector2> gridPositions)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 position = new Vector2(origin.x + (x * tileSpacing), origin.y + (y * tileSpacing));
                gridPositions.Add(position);
                GameObject tile = Instantiate(gridTilePrefab, position, Quaternion.identity, parent);
                InterfaceUnit character = playerPrefabs[x].GetComponent<InterfaceUnit>();
                tile.AddComponent<CharacterTileClickHandler>().Init(this, x);
                if (character != null && character.Icon != null){
                    GameObject iconObject = new GameObject("Icon");
                    SpriteRenderer iconRenderer = iconObject.AddComponent<SpriteRenderer>();
                    iconRenderer.sprite = character.Icon;
                    iconObject.transform.position = position;
                    iconObject.transform.SetParent(tile.transform);
                }
            }
        }
    }
    public void ToggleIsInBattle(bool battle){
        inBattle=battle;
    }
    public bool CheckBattle(){
        return inBattle;
    }
    public GameObject[] GetCharacterPrefabs(){
        Debug.Log(playerPrefabs[0].name);
        return playerPrefabs;
    }
    public bool CheckPrefabExistsInArray(string prefabName){
        foreach (GameObject prefab in playerPrefabs){
            if (prefabName.Equals(prefab.name)){
                return true;
            }
        }
        return false;
    }
    public void GoToCharacterSelection(int index){
        //changeScene
        SceneManager.LoadScene("CharacterSelectionScene");
        changingTileIndex=index;
    }
    public void changeCharaterPrefabs(GameObject prefab, string player,bool party){
        if(party && (CheckPrefabExistsInArray(prefab.name)==false)){
            if(player.Equals("playerOne") && changingTileIndex==0 ){
                playerPrefabs[changingTileIndex]= prefab;
            }else if(player.Equals("playerTwo") && changingTileIndex==1){
                playerPrefabs[changingTileIndex]= prefab;
            }
        }else{
            if((CheckPrefabExistsInArray(prefab.name)==false)){
                playerPrefabs[changingTileIndex]= prefab;
            }
        }

    }
}