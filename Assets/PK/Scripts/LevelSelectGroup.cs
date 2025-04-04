using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectGroup : MonoBehaviour
{
    [SerializeField] List<string> stageTitle;
    [SerializeField] GameObject stageAccessBtn;
    private GameObject stageBtnInstantiated;
    private TextMeshProUGUI stageIndex;
    private string stageIndexTitleText = "스테이지 ";
    
    void Start()
    {
        for(int i = 0; i < stageTitle.Count; i++) // lay up some buttons under this object
        {
            stageBtnInstantiated = Instantiate(
                stageAccessBtn, Vector3.zero, Quaternion.identity
            );
            stageBtnInstantiated.transform.SetParent(transform);
            stageBtnInstantiated.GetComponentInChildren<TextMeshProUGUI>().text = stageIndexTitleText + (i + 1);
            stageBtnInstantiated.GetComponentInChildren<LevelSelectBtn>().DefineLevel(stageTitle[i], i);
        }
    }
}
