using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UI;

public class P3_AmmoHudBar : MonoBehaviour
{
    #region Variables 

    [SerializeField] private Slider ammoSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private Gradient fillGradient;
    [SerializeField] private SliderMark<int> sliderMarks;

    #endregion

    public void SetCurrentAmmo(int currentAmount)
    {
        ammoSlider.value = currentAmount;
        sliderFill.color = fillGradient.Evaluate(ammoSlider.normalizedValue);
    }

    public void SetMaxAmmo(int maxAmount)
    {
        ammoSlider.maxValue = maxAmount;
        ammoSlider.value = maxAmount;
        sliderFill.color = fillGradient.Evaluate(1f);
        sliderMarks.value = maxAmount;
    }
}