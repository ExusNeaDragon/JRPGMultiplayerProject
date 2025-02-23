using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public GameObject gridTilePrefab; // Grid tile prefab

    public int gridSizeX = 12;
    public int gridSizeY = 7;
    public float tileSpacing = 1f;
    public Transform GridParent;
    
    public List<GameObject> playerPrefabs; // List of selectable player characters
    public List<GameObject> enemyPrefabs;  // List of enemy prefabs

    private GameObject selectedCharacter; // Current character to place
    private Vector2 GridOrigin;
    private Dictionary<Vector2, GameObject> occupiedTiles = new Dictionary<Vector2, GameObject>(); // Track occupied tiles
    private List<Vector2> GridPositions = new List<Vector2>();
    private CharacterManager characterManager;
    private List<int> usedIndex=new List<int>();
    private int index;


    public List<Unit> units = new List<Unit>(); // List of all units in battle
    private int currentTurnIndex = 0;

    void Start()
    {
        playerPrefabs=new List<GameObject>();
        //enemyPrefabs=new List<GameObject>();
        characterManager=GameObject.Find("CharacterManager").GetComponent<CharacterManager>();
        GameObject[] prefabs=characterManager.GetCharacterPrefabs();
        foreach(GameObject prefab in prefabs){
            playerPrefabs.Add(prefab);
        }
        characterManager.ToggleIsInBattle(true);
        float totalWidth = (gridSizeX - 1) * tileSpacing;
        float totalHeight = (gridSizeY - 1) * tileSpacing;
        GridOrigin = new Vector2(-totalWidth / 2, -totalHeight / 2);
        GenerateGrid(GridParent, GridOrigin, GridPositions);

        SpawnEnemies();
        StartCoroutine(StartBattle());
    }

    void GenerateGrid(Transform parent, Vector2 origin, List<Vector2> gridPositions)
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 position = new Vector2(origin.x + (x * tileSpacing), origin.y + (y * tileSpacing));
                gridPositions.Add(position);
                GameObject tile = Instantiate(gridTilePrefab, position, Quaternion.identity, parent);
                tile.AddComponent<TileClickHandler>().Init(this, position);
            }
        }
    }
    public void OnTileClicked(Vector2 tilePosition)
    {
        Debug.Log($"OnTileClicked called with position: {tilePosition}");
        
        if (selectedCharacter == null)
        {
            Debug.Log("No character selected. Cannot spawn.");
            return;
        }

        float leftBoundary = GridOrigin.x;
        float rightBoundary = GridOrigin.x + (4 * tileSpacing); 
        float topBoundary = GridOrigin.y + ((gridSizeY - 1) * tileSpacing);
        float bottomBoundary = GridOrigin.y;


        if (tilePosition.x < leftBoundary || tilePosition.x >= rightBoundary || 
            tilePosition.y < bottomBoundary || tilePosition.y > topBoundary || 
            occupiedTiles.ContainsKey(tilePosition))
        {
            Debug.Log("Invalid tile or tile already occupied.");
            return;
        }

        // Spawn the player unit
        if(!usedIndex.Contains(index)){
            GameObject unit = Instantiate(selectedCharacter, tilePosition, Quaternion.identity);
            occupiedTiles[tilePosition] = unit; // Track the occupied tile
            usedIndex.Add(index);
            Debug.Log($"Player unit spawned at {tilePosition}");
        }else{
            Debug.Log($"Player unit already spawned");
        }

    }
    void SpawnEnemies()
    {
        for (int i = 0; i < 3; i++)
        {
            int enemyX = Random.Range(gridSizeX - 4, gridSizeX);  // Last 4 columns
            int enemyY = Random.Range(0, gridSizeY); 
            Vector2 enemyStartPos = GetGridPosition(enemyX, enemyY);

            if (enemyStartPos != Vector2.zero && enemyPrefabs.Count > 0)  
            {
                GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
                Instantiate(randomEnemy, enemyStartPos, Quaternion.identity);
            }
        }
    }

    Vector2 GetGridPosition(int x, int y)
    {
        foreach (var pos in GridPositions)
        {
            if (Mathf.Approximately(pos.x, GridOrigin.x + (x * tileSpacing)) &&
                Mathf.Approximately(pos.y, GridOrigin.y + (y * tileSpacing)))
            {
                return pos;
            }
        }
        return Vector2.zero;
    }
    public void SelectCharacter(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < playerPrefabs.Count)
        {
            selectedCharacter = playerPrefabs[characterIndex];
            index=characterIndex;
            Debug.Log($"Selected {selectedCharacter.name}");
        }
        else
        {
            Debug.Log("Invalid character index selected.");
        }
    }



    IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(1f);
        StartTurn();
    }

    void StartTurn()
{
    if (currentTurnIndex >= units.Count)
    {
        currentTurnIndex = 0; // Reset turn order
    }

    Unit currentUnit = units[currentTurnIndex];

    // Display turn message only once when turn starts
    Debug.Log(currentUnit.unitName + "'s turn!");

    if (currentUnit.isPlayer)
    {
        currentUnit.StartPlayerTurn();
    }
    else
    {
        StartCoroutine(EnemyTurn(currentUnit)); // AI takes action
    }
}


    public void EndTurn()
    {
        currentTurnIndex++;
        StartTurn();
    }

IEnumerator EnemyTurn(Unit enemy)
{
    yield return new WaitForSeconds(1f); // Small delay before action

    Unit player = FindClosestPlayer(enemy);

    if (player != null && enemy.CanAttack(player))
    {
        enemy.Attack(player);
    }
    else
    {
        enemy.MoveToward(player);
    }

    yield return new WaitForSeconds(1f); // Prevents immediate turn swap
}




    Unit FindClosestPlayer(Unit enemy)
    {
        Unit closest = null;
        float closestDistance = float.MaxValue;

        foreach (Unit unit in units)
        {
            if (unit.isPlayer)
            {
                float distance = Vector2Int.Distance(enemy.gridPosition, unit.gridPosition);
                if (distance < closestDistance)
                {
                    closest = unit;
                    closestDistance = distance;
                }
            }
        }
        return closest;
    }
}


