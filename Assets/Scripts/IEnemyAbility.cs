using UnityEngine;

public interface IEnemyAbility
{
    //SpecialActive are conditon met (ex at 50% hp slow all player down by 15%)
    //SpecialPassive are actives on cooldown/permenant (ex heal hp every 5 seconds or gain 10% attack against players at full hp)
    public enum EnemyType { Melee, Ranged, SpecialActive, SpecialPassive }

    EnemyType Type { get; }

    void Execute(EnemyAI enemy);
    void Execute(EnemyAI enemy, Transform player);
}
