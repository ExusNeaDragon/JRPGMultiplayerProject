using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public GameObject gridTilePrefab; // Grid tile prefab
    public Transform playerGridParent, enemyGridParent; // Separate parents for grids
    public GameObject playerPrefab, enemyPrefab; // Prefabs for player and enemy units

    public int gridSizeX = 5;
    public int gridSizeY = 5;
    public float tileSpacing = 1f;

    private Vector2 playerGridOrigin = new Vector2(-3, 0); // Left side
    private Vector2 enemyGridOrigin = new Vector2(3, 0); // Right side

    private List<Vector2> playerGridPositions = new List<Vector2>();
    private List<Vector2> enemyGridPositions = new List<Vector2>();

    public List<Unit> units = new List<Unit>(); // List of all units in battle
    private int currentTurnIndex = 0;

    void Start()
    {
        GenerateGrid(playerGridParent, playerGridOrigin, playerGridPositions);
        GenerateGrid(enemyGridParent, enemyGridOrigin, enemyGridPositions);
        SpawnUnits();
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
                Instantiate(gridTilePrefab, position, Quaternion.identity, parent);
            }
        }
    }

    void SpawnUnits()
    {
        Vector2 playerStartPos = playerGridPositions[12]; // Center of 5x5 grid
        Vector2 enemyStartPos = enemyGridPositions[12];

        GameObject playerUnit = Instantiate(playerPrefab, playerStartPos, Quaternion.identity);
        GameObject enemyUnit = Instantiate(enemyPrefab, enemyStartPos, Quaternion.identity);

        Unit playerScript = playerUnit.GetComponent<Unit>();
        Unit enemyScript = enemyUnit.GetComponent<Unit>();

        playerScript.battleManager = this;
        enemyScript.battleManager = this;

        units.Add(playerScript);
        units.Add(enemyScript);

        Debug.Log($"Player spawned at {playerStartPos}, Enemy spawned at {enemyStartPos}");
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
