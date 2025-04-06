using System;
using System.Collections;
using UnityEngine;

public class IngameMenuGameover : MonoBehaviour
{
    RectTransform transformBasis;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transformBasis = GetComponent<RectTransform>();
        MenuUIManager.Instance.SetMenuUIGameover(this);
    }

    internal void SetGameover()
    {
        StartCoroutine(SetGameoverCo());
    }

    private IEnumerator SetGameoverCo()
    {
        yield return new WaitForSeconds(2f);
        transformBasis.gameObject.GetComponent<CanvasGroup>().alpha = 1;
    }
}
