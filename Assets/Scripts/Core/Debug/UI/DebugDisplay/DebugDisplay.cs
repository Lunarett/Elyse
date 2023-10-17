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
    
    public static DebugDisplay Instance { get; private set; }

    public static DebugDisplay EnsureInstance()
    {
        if (Instance != null) return Instance;
        // Create a new GameObject to host the DebugDisplay component
        var debugDisplayObject = new GameObject("DebugDisplay");
        Instance = debugDisplayObject.AddComponent<DebugDisplay>();

        return Instance;
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one instance of DebugDisplay found!");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        scrollDisplay = root.Q<ScrollView>("ScrollDisplay");
        infoPanel = root.Q<VisualElement>("InfoPanel");
    }

    public void AddLog(Texture2D icon, string message, Color textColor)
    {
        // Clone the log element
        var logElement = logElementTree.CloneTree();

        // Set the icon and message
        logElement.Q<VisualElement>("Icon").style.backgroundImage = icon;
        var logMessageLabel = logElement.Q<Label>("LogMessage");
        logMessageLabel.text = message;
        logMessageLabel.style.color = textColor;  // Set text color

        // Add to the scroll view
        scrollDisplay.Add(logElement);
    }

    public void AddLogWithFoldout(Texture2D icon, string message, string[] additionalInfo, Color textColor)
    {
        // Clone the log foldout element
        var logElementFoldout = logElementFoldoutTree.CloneTree();

        // Set the icon
        logElementFoldout.Q<VisualElement>("Icon").style.backgroundImage = icon;

        // Find the foldout and set its main text to the message
        var foldout = logElementFoldout.Q<Foldout>("Foldout");
        if(foldout == null)
        {
            UnityEngine.Debug.LogError("#Foldout Foldout not found.");
            return;
        }

        foldout.text = message; // The main text of the foldout is the log message
        foldout.value = false;
        
        // Option 1: Create a new label for each piece of additional information
        foreach (var info in additionalInfo)
        {
            var newLabel = new Label(info);
            newLabel.style.marginBottom = 0; // Reduces the space below the label
            newLabel.style.marginTop = 0; // Reduces the space above the label
            foldout.Add(newLabel);
        }
        
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