using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    [SerializeField] public Button singleButton;
    [SerializeField] public Button multiButton;
    [SerializeField] public Button quitButton;

    void Start()
    {
        singleButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("WorldLoaderSingleplayer"); // Load scene locally

        });
        multiButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("WorldLoaderMultiplayer"); // Load scene locally

        });

        quitButton.onClick.AddListener(() =>{
            Application.Quit();
        });
    }

}
