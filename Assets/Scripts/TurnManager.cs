using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private Queue<UnitCombat> turnQueue = new Queue<UnitCombat>(); // Holds turn order
    private UnitCombat currentUnit; // The unit currently taking action

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void InitializeTurns(List<UnitCombat> allUnits)
    {
        foreach (UnitCombat unit in allUnits)
        {
            turnQueue.Enqueue(unit); // Add all units to the queue
        }

        NextTurn(); // Start the first turn
    }

    public void NextTurn()
    {
        if (turnQueue.Count == 0) return; // No units left

        currentUnit = turnQueue.Dequeue(); // Get the next unit
        turnQueue.Enqueue(currentUnit); // Put them back in the queue

        currentUnit.StartTurn(); // Activate unit’s turn
    }
}
