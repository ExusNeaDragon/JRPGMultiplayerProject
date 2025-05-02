using UnityEngine;
using UnityEngine.SceneManagement;

public class MaintainMenuMusic : MonoBehaviour
{
    void Start()
    {
        if (FindObjectsOfType<MaintainMenuMusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "Level 1")
                Destroy(gameObject);
        };
    }
}