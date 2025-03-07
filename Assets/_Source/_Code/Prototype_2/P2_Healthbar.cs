using UnityEngine;
using UnityEngine.UI;

public class P2_Healthbar : MonoBehaviour
{
    #region

    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image sliderFill;

    #endregion
    
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        sliderFill.color = gradient.Evaluate(1f);
    }

    public void SetCurrentHealth(int health)
    {
        slider.value = health;
        sliderFill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
