using UnityEngine;
using UnityEngine.UI;

public class FireCoolDownSlider : MonoBehaviour
{
    Slider fireCoolDownSlider;

    private void Start()
    {
        fireCoolDownSlider = GetComponent<Slider>();
        PlayerManager.Instance.OnChangeFireCoolDownAction += ChangeSliderValue;
    }

    private void ChangeSliderValue(float currentValue, float maxValue)
    {
        fireCoolDownSlider.maxValue = maxValue;
        fireCoolDownSlider.value = currentValue;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnChangeFireCoolDownAction -= ChangeSliderValue;
    }
}
