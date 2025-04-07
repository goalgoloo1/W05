using System.Collections;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;

public class IngameMenuComplete : MonoBehaviour
{
    RectTransform transformBasis;
    [SerializeField] NextStageBtn nextStageBtn;
    [SerializeField] TextMeshProUGUI recordText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transformBasis = GetComponent<RectTransform>();
        //CursorManager.Instance.SetCursorVisible();
        MenuUIManager.Instance.SetMenuUIComplete(this);
    }
    internal void SetComplete(string stageName, float elapsedTime)
    {
        Cursor.lockState = CursorLockMode.None; // 마우스 잠금 해제
        Cursor.visible = true; // 커서 보이게 하기
        
        Debug.Log("Going to " + stageName);

        nextStageBtn.SetStage(stageName);
        ChangeTimeText(elapsedTime);

        StartCoroutine(SetCompleteCo());
    }

    private void ChangeTimeText(float newValue)
    {
        int minutes = (int)(newValue / 60);
        float remainingSeconds = newValue % 60;
        int seconds = (int)remainingSeconds;
        int deciseconds = (int)((remainingSeconds - seconds) * 100);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", minutes, seconds, deciseconds);
        recordText.text = timeText;
    }

    private IEnumerator SetCompleteCo()
    {
        yield return new WaitForSeconds(2f);
        transformBasis.gameObject.GetComponent<CanvasGroup>().alpha = 1;
    }
}
