using UnityEditor;
using UnityEngine;
using System.IO;

public class FilePathAttribute : PropertyAttribute
{
    public string Filter;

    public FilePathAttribute(string filter = "")
    {
        Filter = filter;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FilePathAttribute))]
public class FilePathDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        FilePathAttribute filePathAttribute = (FilePathAttribute)attribute;

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Calculate rects
        Rect textFieldRect = position;
        textFieldRect.width -= 35;
        Rect buttonRect = position;
        buttonRect.x += position.width - 25;
        buttonRect.width = 25;

        // Draw fields
        EditorGUI.BeginChangeCheck();
        string value = EditorGUI.TextField(textFieldRect, property.stringValue);
        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = value;
        }

        if (!GUI.Button(buttonRect, "...")) return;
        // Open the file panel without a filter to allow all file types
        string path = EditorUtility.OpenFilePanel("Select File", Application.dataPath, filePathAttribute.Filter);

        if (string.IsNullOrEmpty(path) || !path.StartsWith(Application.dataPath)) return;
        property.stringValue = "Assets" + path.Substring(Application.dataPath.Length);
        property.serializedObject.ApplyModifiedProperties();
    }
}
#endif