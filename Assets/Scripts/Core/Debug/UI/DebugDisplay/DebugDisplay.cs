using UnityEngine;
using UnityEngine.UIElements;

public class DebugDisplay : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset logElementTree;
    [SerializeField] private VisualTreeAsset infoElementTree;
    [SerializeField] private VisualTreeAsset logElementFoldoutTree;

    private VisualElement root;
    private ScrollView scrollDisplay;
    private VisualElement infoPanel;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        scrollDisplay = root.Q<ScrollView>("ScrollDisplay");
        infoPanel = root.Q<VisualElement>("InfoPanel");
    }

    public void AddLog(Texture2D icon, string message)
    {
        // Clone the log element
        var logElement = logElementTree.CloneTree();

        // Set the icon and message
        logElement.Q<VisualElement>("Icon").style.backgroundImage = icon;
        logElement.Q<Label>("LogMessage").text = message;

        // Add to the scroll view
        scrollDisplay.Add(logElement);
    }

    public void AddLogWithFoldout(Texture2D icon, string message, string[] additionalInfo)
    {
        // Clone the log foldout element
        var logElementFoldout = logElementFoldoutTree.CloneTree();

        // Set the icon and message
        logElementFoldout.Q<VisualElement>("Icon").style.backgroundImage = icon;
        logElementFoldout.Q<Label>("LogMessage").text = message;

        // Create and add additional information labels inside the foldout
        var foldout = logElementFoldout.Q<Foldout>("Foldout");
        foreach (var info in additionalInfo)
        {
            var newLabel = new Label(info);
            foldout.Add(newLabel);
        }

        // Add to the scroll view
        scrollDisplay.Add(logElementFoldout);
    }

    public void AddInfo(string info, Color textColor)
    {
        Label newInfo = new Label(info)
        {
            style = { color = textColor }
        };
        infoPanel.Add(newInfo);
    }

    public void ClearInfo()
    {
        infoPanel.Clear();
    }
}