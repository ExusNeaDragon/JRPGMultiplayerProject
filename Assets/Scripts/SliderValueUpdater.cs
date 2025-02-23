using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueUpdater : MonoBehaviour
{
    public Slider slider;
    public TMP_Text valueText;

    void Start()
    {
        if (slider != null && valueText != null)
        {
            valueText.text = slider.value.ToString("0"); // Initialize text
            slider.onValueChanged.AddListener(UpdateValue);
        }
        else
        {
            Debug.LogWarning("Slider or Value Text not assigned in " + gameObject.name);
        }
    }

    void UpdateValue(float value)
    {
        valueText.text = value.ToString("0"); // Display as a whole number
    }
}
