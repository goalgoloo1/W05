using UnityEngine;
using UnityEngine.UI;

public class ReloadingTimeSlider : MonoBehaviour
{
    Slider reloadingTimeSlider;

    private void Start()
    {
        reloadingTimeSlider = GetComponent<Slider>();
        PlayerManager.Instance.OnChangeReloadingTimeAction += ChangeSliderValue;
    }

    private void ChangeSliderValue(float currentValue, float maxValue)
    {
        reloadingTimeSlider.maxValue = maxValue;
        reloadingTimeSlider.value = maxValue - currentValue;
    }
}
