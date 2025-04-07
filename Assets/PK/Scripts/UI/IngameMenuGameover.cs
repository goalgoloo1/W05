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
        Cursor.lockState = CursorLockMode.None; // 마우스 잠금 해제
        Cursor.visible = true; // 커서 보이게 하기

        StartCoroutine(SetGameoverCo());
    }

    private IEnumerator SetGameoverCo()
    {
        yield return new WaitForSeconds(2f);
        transformBasis.gameObject.GetComponent<CanvasGroup>().alpha = 1;
    }
}
