using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionItem : MonoBehaviour
{
    public Toggle toggle; // Assign this in Inspector
    public TMP_Text optionLabel; // Assign this in Inspector

    public void SetupOption(string labelText)
    {
        optionLabel.text = labelText; // Set label dynamically
    }
}
