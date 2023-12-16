using UnityEngine;
using System.Collections.Generic;

public class LogDisplayManager : MonoBehaviour
{
    private class Message
    {
        public string text;
        public float endTime;
        public Color textColor;
    }

    private List<Message> messages = new List<Message>();
    private GUIStyle guiStyle = new GUIStyle();
    private Texture2D backgroundTexture;
    private Font customFont;

    void Awake()
    {
        customFont = Resources.Load<Font>("UI/Fonts/SourceCodePro-Semibold");
        if (customFont == null)
        {
            Debug.LogError("Custom font not found.");
            return;
        }

        guiStyle.font = customFont;
        guiStyle.fontSize = 22;
        guiStyle.padding = new RectOffset(10, 10, 5, 5);
        guiStyle.alignment = TextAnchor.MiddleLeft;

        // Create a background texture
        backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.8f)); // Semi-transparent black
        backgroundTexture.Apply();
    }

    void OnGUI()
    {
        float yPos = 10f; // Starting y position
        foreach (var message in messages)
        {
            if (Time.time < message.endTime)
            {
                GUIContent content = new GUIContent(message.text);
                Vector2 size = guiStyle.CalcSize(content);

                // Draw background for the entire text string
                Rect backgroundRect = new Rect(10, yPos, size.x + 20, size.y + 10);
                GUI.DrawTexture(backgroundRect, backgroundTexture); // Draw the texture as background

                // Set text color and draw the text
                guiStyle.normal.textColor = message.textColor;
                Rect textRect = new Rect(10, yPos, size.x, size.y);
                GUI.Label(textRect, message.text, guiStyle);

                yPos += size.y + 10; // Increment y position for the next message
            }
        }

        messages.RemoveAll(msg => Time.time >= msg.endTime);
    }

    public void AddMessage(string text, float duration, Color color)
    {
        messages.Add(new Message { text = text, endTime = Time.time + duration, textColor = color });
    }
}