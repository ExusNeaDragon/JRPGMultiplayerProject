using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillCostCalculator : MonoBehaviour
{
    public MultiSelectDropdown buffDropdown, debuffDropdown, attributeDropdown, componentDropdown;
    public ScalingSlider scalingSlider;
    public TMP_Text costDisplay;

    private int baseMPCost = 10;
    private int baseStaminaCost = 5;

    private void Update()
    {
        CalculateCosts();
    }

    void CalculateCosts()
    {
        List<string> buffs = buffDropdown.GetSelectedOptions();
        List<string> debuffs = debuffDropdown.GetSelectedOptions();
        List<string> attributes = attributeDropdown.GetSelectedOptions();
        List<string> components = componentDropdown.GetSelectedOptions();

        float scalingFactor = scalingSlider.powerSlider.value / 100f;

        int mpCost = baseMPCost + (components.Count * 2) + (attributes.Count * 3);
        int staminaCost = baseStaminaCost + (buffs.Count * 1) + (debuffs.Count * 1);

        mpCost = Mathf.RoundToInt(mpCost * scalingFactor);
        staminaCost = Mathf.RoundToInt(staminaCost * scalingFactor);

        costDisplay.text = $"MP Cost: {mpCost} | Stamina Cost: {staminaCost}";
    }
}
