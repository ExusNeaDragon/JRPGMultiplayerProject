using UnityEngine;

public class SkillCreatorMenu : MonoBehaviour
{
    public GameObject creatorCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creatorCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            creatorCanvas.SetActive(!creatorCanvas.activeSelf);
        }
    }
}
