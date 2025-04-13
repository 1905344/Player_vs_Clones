using UnityEngine;
using UnityEngine.UI;

public class P3_ProgressBar : MonoBehaviour
{
    #region

    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image sliderFill;

    #endregion
    public void SetMaxValue(int max_value)
    {
        slider.maxValue = max_value;
        slider.value = max_value;

        sliderFill.color = gradient.Evaluate(1f);
    }

    public void SetCurrentValue(int current_value)
    {
        slider.value = current_value;
        sliderFill.color = gradient.Evaluate(slider.normalizedValue);
    }
}