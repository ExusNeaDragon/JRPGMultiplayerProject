using UnityEngine;
public interface IEnemyAbility
{
    void Execute(EnemyAI enemy);
    void Execute(EnemyAI enemy, Transform player);
}
