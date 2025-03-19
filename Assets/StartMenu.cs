using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StartMenu : MonoBehaviour
{
    [SerializeField] public Button playButton;
    [SerializeField] public Button quitButton;

    void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("WorldLoader"); // Load scene locally

        });

        quitButton.onClick.AddListener(() =>{
            Application.Quit();
        });
    }

}
