// using UnityEngine;
// using UnityEditor;
//
// [CreateAssetMenu(fileName = "LoggerSettings", menuName = "Pulsar/Logger Settings", order = 1)]
// public class LoggerSettings : ScriptableObject
// {
//     public Texture2D logIcon;
//     public Texture2D warnIcon;
//     public Texture2D errorIcon;
//     public Texture2D successIcon;
//
//     public Color logTextColor = Color.white;
//     public Color warnTextColor = Color.yellow;
//     public Color errorTextColor = Color.red;
//     public Color successTextColor = Color.green;
//
//     private const string settingsPath = "Assets/Pulsar/Debug/LoggerSettings.asset";
//
//     public static LoggerSettings GetOrCreateSettings()
//     {
//         var settings = AssetDatabase.LoadAssetAtPath<LoggerSettings>(settingsPath);
//         if (settings == null)
//         {
//             settings = CreateInstance<LoggerSettings>();
//             AssetDatabase.CreateAsset(settings, settingsPath);
//             AssetDatabase.SaveAssets();
//         }
//         return settings;
//     }
//
//     [SettingsProvider]
//     public static SettingsProvider CreateSettingsProvider()
//     {
//         return new AssetSettingsProvider(
//             settingsPath, () => GetOrCreateSettings()
//         );
//     }
// }
//
// [CustomEditor(typeof(LoggerSettings))]
// public class LoggerSettingsEditor : Editor
// {
//     private bool showIcons = true;
//     private bool showColors = true;
//
//     public override void OnInspectorGUI()
//     {
//         LoggerSettings settings = (LoggerSettings)target;
//
//         // Draw title
//         EditorGUILayout.LabelField("Logger Settings", EditorStyles.boldLabel);
//
//         // Draw icons section
//         showIcons = EditorGUILayout.Foldout(showIcons, "Icons");
//         if (showIcons)
//         {
//             EditorGUI.indentLevel++;
//             EditorGUIUtility.labelWidth = 100;
//             settings.logIcon = (Texture2D)EditorGUILayout.ObjectField("Log Icon", settings.logIcon, typeof(Texture2D), false);
//             settings.warnIcon = (Texture2D)EditorGUILayout.ObjectField("Warn Icon", settings.warnIcon, typeof(Texture2D), false);
//             settings.errorIcon = (Texture2D)EditorGUILayout.ObjectField("Error Icon", settings.errorIcon, typeof(Texture2D), false);
//             settings.successIcon = (Texture2D)EditorGUILayout.ObjectField("Success Icon", settings.successIcon, typeof(Texture2D), false);
//             EditorGUI.indentLevel--;
//         }
//
//         // Draw separator
//         EditorGUILayout.Space();
//         EditorGUILayout.Separator();
//         EditorGUILayout.Space();
//
//         // Draw colors section
//         showColors = EditorGUILayout.Foldout(showColors, "Text Colors");
//         if (showColors)
//         {
//             EditorGUI.indentLevel++;
//             settings.logTextColor = EditorGUILayout.ColorField("Log Text Color", settings.logTextColor);
//             settings.warnTextColor = EditorGUILayout.ColorField("Warn Text Color", settings.warnTextColor);
//             settings.errorTextColor = EditorGUILayout.ColorField("Error Text Color", settings.errorTextColor);
//             settings.successTextColor = EditorGUILayout.ColorField("Success Text Color", settings.successTextColor);
//             EditorGUI.indentLevel--;
//         }
//
//         // Apply changes
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(settings);
//         }
//     }
// }
