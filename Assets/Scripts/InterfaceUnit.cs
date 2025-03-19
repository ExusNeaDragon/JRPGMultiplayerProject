using UnityEngine;

public interface InterfaceUnit
{
    Sprite Icon { get; } // Character image/icon

    void Attack(Unit target); // Attack another character
    void MoveToward(Unit newPosition); // Move character to a new position
    void TakeDamage(int damage); // Reduce health when hit

}
