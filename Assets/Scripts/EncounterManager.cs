using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Encounter triggered!");
            SceneManager.LoadScene("BattleScene"); // Load the battle scene
        }
    }
}
