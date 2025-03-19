using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScalingSlider : MonoBehaviour
{
    public Slider powerSlider;
    public TMP_InputField powerInput;

    private void Start()
    {
        powerSlider.onValueChanged.AddListener(UpdateInputField);
        powerInput.onEndEdit.AddListener(UpdateSlider);
        powerSlider.value = 90; // Default value
        powerInput.text = powerSlider.value.ToString();
    }

    void UpdateInputField(float value)
    {
        powerInput.text = value.ToString("0");
    }

    void UpdateSlider(string value)
    {
        if (float.TryParse(value, out float newValue))
        {
            newValue = Mathf.Clamp(newValue, 30, 500);
            powerSlider.value = newValue;
        }
    }
}
