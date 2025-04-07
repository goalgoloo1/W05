using System.Collections.Generic;
using UnityEngine;

public class PlayInstructionControl : MonoBehaviour
{
    [SerializeField] List<GameObject> instructionPanels;
    [SerializeField] GameObject previousPanel;

    private void Start()
    {
        previousPanel = instructionPanels[0]; //control panel.
    }

    public void ShowPanelByIndex(int index)
    {
        previousPanel.SetActive(false);
        previousPanel = instructionPanels[index];
        instructionPanels[index].SetActive(true);
    }
}
