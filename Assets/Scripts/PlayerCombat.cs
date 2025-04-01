using UnityEngine;
using Unity.Netcode;

public enum WeaponType { Melee, Ranged }

public class PlayerCombat : NetworkBehaviour
{
    public WeaponType currentWeaponType = WeaponType.Melee;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [ServerRpc]
    public void AttackServerRpc()
    {
        AttackClientRpc();
    }

    [ClientRpc]
    void AttackClientRpc()
    {
        if (currentWeaponType == WeaponType.Melee)
        {
            MeleeAttack();
        }
        else if (currentWeaponType == WeaponType.Ranged)
        {
            RangedAttack();
        }
    }

    void MeleeAttack()
    {
        Debug.Log("Melee attack executed!");
        // Implement melee attack logic (e.g., collision detection, animations)
    }

    void RangedAttack()
    {
        Debug.Log("Ranged attack executed!");
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }
}