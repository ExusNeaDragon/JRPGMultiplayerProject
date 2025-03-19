using System.Collections.Generic;
using UnityEngine;

public class OptionListManager : MonoBehaviour
{
    public GameObject optionPrefab; // Assign your OptionPrefab in the Inspector
    public Transform optionParent;  // The parent container (e.g., a ScrollView)

    private List<GameObject> createdOptions = new List<GameObject>();

    public void PopulateOptions(List<string> options)
    {
        // Clear previous options
        foreach (GameObject option in createdOptions)
        {
            Destroy(option);
        }
        createdOptions.Clear();

        // Create new options
        foreach (string optionName in options)
        {
            GameObject newOption = Instantiate(optionPrefab, optionParent);
            newOption.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = optionName;
            createdOptions.Add(newOption);
        }
    }
}
