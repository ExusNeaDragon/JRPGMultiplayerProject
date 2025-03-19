using UnityEngine;

public class UnitCombat : MonoBehaviour
{
    public bool isPlayer; // Is this the player or an enemy?

    public void StartTurn()
    {
        if (isPlayer)
        {
            Debug.Log("Player's Turn - Choose an action.");
            // Enable player actions
        }
        else
        {
            Debug.Log("Enemy's Turn - AI thinking...");
            EnemyTurn();
        }
    }

    public void Attack(UnitCombat target)
    {
        Debug.Log(gameObject.name + " attacks " + target.gameObject.name);
        TurnManager.Instance.NextTurn(); // End turn after attacking
    }

    private void EnemyTurn()
    {
        // Simple AI - Attack player and end turn
        UnitCombat player = FindFirstObjectByType<UnitCombat>(); // Updated method
        if (player != null && player.isPlayer) // Ensure it's actually the player
        {
            Attack(player);
        }
    }
}
