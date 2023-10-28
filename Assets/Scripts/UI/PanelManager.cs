using System.Collections.Generic;
using Pulsar.Debug;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels = new List<GameObject>();
    [SerializeField] private int startingActiveIndex = -1;
    private GameObject currentActivePanel;

    private void Start()
    {
        if (panels.Count <= 0) return;
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        if (startingActiveIndex < 0 || startingActiveIndex >= panels.Count) return;
        panels[startingActiveIndex].SetActive(true);
        currentActivePanel = panels[startingActiveIndex];
    }

    public void ShowPanel(int panelIndex)
    {
        if (!DebugUtils.CheckValidListIndex<GameObject>(panels, panelIndex)) return;
        if (panels[panelIndex] == null)
        {
            DebugUtils.LogErrorFromCaller("Panel does not exist!");
            return;
        }
        
        
        if (panelIndex < 0 || panelIndex >= panels.Count) return;
        if (currentActivePanel != null)
        {
            currentActivePanel.SetActive(false);
        }
        
        panels[panelIndex].SetActive(true);
        currentActivePanel = panels[panelIndex];
    }

    public void HidePanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Count || !panels[panelIndex].activeSelf) return;
        panels[panelIndex].SetActive(false);
        currentActivePanel = null;
    }

    public void TogglePanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= panels.Count) return;
        if (panels[panelIndex].activeSelf)
        {
            HidePanel(panelIndex);
        }
        else
        {
            ShowPanel(panelIndex);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}