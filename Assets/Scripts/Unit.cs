using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public bool isPlayer;
    public int health = 100;
    public int attack = 10;
    public int defense = 5;
    public int attackRange = 1;

    public Vector2Int gridPosition; // Current position on the grid
    public BattleManager battleManager;
    public UnitMovement movement;

    private bool hasActed = false; // Prevent multiple attacks in one turn

    void Start()
    {
        movement = GetComponent<UnitMovement>();
    }

    public void StartPlayerTurn()
    {
        Debug.Log(unitName + " (Player) turn started! Choose an action.");
        hasActed = false; // Reset turn lock for the new turn
    }

    void Update()
    {
        if (isPlayer && !hasActed) // Ensure input is only allowed when it's the player's turn
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Unit enemy = FindEnemyInRange();
                if (enemy != null)
                {
                    Attack(enemy);
                }
                else
                {
                    Debug.Log("No enemy in range!");
                }
            }

            if (Input.GetKeyDown(KeyCode.W)) Move(Vector2Int.up);
            if (Input.GetKeyDown(KeyCode.S)) Move(Vector2Int.down);
            if (Input.GetKeyDown(KeyCode.A)) Move(Vector2Int.left);
            if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);
        }
    }

    void Move(Vector2Int direction)
    {
        if (hasActed) return; // Prevent moving after attacking
        movement.Move(direction);
        hasActed = true;
    }

    public bool CanAttack(Unit target)
    {
        return Vector2Int.Distance(gridPosition, target.gridPosition) <= attackRange;
    }

public void Attack(Unit target)
{
    if (CanAttack(target))
    {
        Debug.Log(unitName + " attacks " + target.unitName + " for " + attack + " damage!");
        target.TakeDamage(attack);

        battleManager.EndTurn(); // End turn after attacking
    }
}

public void MoveToward(Unit target)
{
    if (target == null) return;

    Vector2Int direction = target.gridPosition - gridPosition;

    // Normalize movement to only move one tile per turn
    direction = new Vector2Int(
        Mathf.Clamp(direction.x, -1, 1),
        Mathf.Clamp(direction.y, -1, 1)
    );

    movement.Move(direction);

    // Ensure the turn ends only after moving
    battleManager.EndTurn();
}

    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage - defense, 1);
        health -= actualDamage;

        Debug.Log(unitName + " took " + actualDamage + " damage! Health: " + health);

        if (health <= 0)
        {
            Debug.Log(unitName + " has been defeated!");
            gameObject.SetActive(false);
        }
    }

    Unit FindEnemyInRange()
    {
        foreach (Unit unit in battleManager.units)
        {
            if (!unit.isPlayer && CanAttack(unit))
            {
                return unit;
            }
        }
        return null;
    }


}
