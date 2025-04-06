using UnityEngine;
using UnityEngine.UI;

public class EvadeTimeoutSlider : MonoBehaviour
{
    Slider evadeTimeoutSlider;
    CanvasGroup canvasGroup;
    private void Start()
    {
        PlayerManager.Instance.OnChangeEvadeCoolDownAction += UpdateSliderValue;
        evadeTimeoutSlider = GetComponent<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void UpdateSliderValue(float currentValue, float maxValue)
    {
        evadeTimeoutSlider.maxValue = maxValue;
        evadeTimeoutSlider.value = currentValue;

        if(currentValue >= maxValue)
        {
            canvasGroup.alpha = 0;
        }
        else
        {
            canvasGroup.alpha = 1;
        }
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnChangeEvadeCoolDownAction -= UpdateSliderValue;
    }
}
