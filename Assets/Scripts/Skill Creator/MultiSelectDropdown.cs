using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelectDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Transform dropdownListContainer; // UI Panel to hold checkboxes
    public GameObject optionPrefab; // Prefab for checkbox options
    public List<string> selectedOptions = new List<string>();

    private void Start()
    {
        PopulateOptions();
    }

void PopulateOptions()
{
    if (dropdownListContainer == null)
    {
        Debug.LogError("dropdownListContainer is NULL! Assign it in the Inspector.");
        return;
    }

    if (dropdown == null)
    {
        Debug.LogError("dropdown is NULL! Assign it in the Inspector.");
        return;
    }

    dropdownListContainer.gameObject.SetActive(false);
    dropdown.onValueChanged.AddListener(delegate { ToggleDropdown(); });
}

    void ToggleDropdown()
    {
        dropdownListContainer.gameObject.SetActive(!dropdownListContainer.gameObject.activeSelf);
    }

    public void SelectOption(Toggle toggle, string option)
    {
        if (toggle.isOn)
            selectedOptions.Add(option);
        else
            selectedOptions.Remove(option);
    }

    public List<string> GetSelectedOptions()
    {
        return selectedOptions;
    }
}
