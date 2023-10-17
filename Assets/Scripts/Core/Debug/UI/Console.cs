// using UnityEngine;
// using UnityEngine.UIElements;
// using System.Diagnostics;
//
// namespace Pulsar.Debug
// {
//     public static class Console
//     {
//         private static DebugDisplay debugDisplay;
//
//         private static void Init()
//         {
//             if (debugDisplay == null)
//             {
//                 debugDisplay = DebugDisplay.Instance;
//                 if (debugDisplay == null)
//                 {
//                     UnityEngine.Debug.LogError("No DebugDisplay instance found in the scene.");
//                 }
//             }
//         }
//
//         public static void Log(string message)
//         {
//             if (debugDisplay == null)
//             {
//                 debugDisplay = DebugDisplay.Instance;
//                 if (debugDisplay == null)
//                 {
//                     UnityEngine.Debug.LogError("No DebugDisplay instance found in the scene.");
//                     return;  
//                 }
//                 else
//                 {
//                     UnityEngine.Debug.Log("DebugDisplay instance found.");
//                 }
//             }
//     
//             LoggerSettings settings = LoggerSettings.GetOrCreateSettings();
//             if(settings == null) 
//             {
//                 UnityEngine.Debug.LogError("Settings are null");
//                 return; 
//             }
//             else
//             {
//                 UnityEngine.Debug.Log("LoggerSettings retrieved successfully.");
//             }
//
//             UnityEngine.Debug.Log("Attempting to add log with foldout...");
//             debugDisplay.AddLogWithFoldout(settings.logIcon, message, GetCallerInfo(), settings.logTextColor);
//             UnityEngine.Debug.Log("Log with foldout added successfully.");
//             UnityEngine.Debug.Log(message);
//         }
//
//
//         public static void Warn(string message)
//         {
//             if (debugDisplay == null) 
//             {
//                 Init();
//             }
//             
//             
//             LoggerSettings settings = LoggerSettings.GetOrCreateSettings();
//             debugDisplay.AddLogWithFoldout(settings.warnIcon, message, GetCallerInfo(), settings.warnTextColor);
//             UnityEngine.Debug.LogWarning(message);
//         }
//
//         public static void Error(string message)
//         {
//             if (debugDisplay == null) 
//             {
//                 Init();
//             }
//
//             LoggerSettings settings = LoggerSettings.GetOrCreateSettings();
//             debugDisplay.AddLogWithFoldout(settings.errorIcon, message, GetCallerInfo(), settings.errorTextColor);
//             UnityEngine.Debug.LogError(message);
//         }
//
//         public static void Success(string message)
//         {
//             if (debugDisplay == null) 
//             {
//                 Init();
//             }
//             
//             LoggerSettings settings = LoggerSettings.GetOrCreateSettings();
//             debugDisplay.AddLogWithFoldout(settings.successIcon, message, GetCallerInfo(), settings.successTextColor);
//             UnityEngine.Debug.Log(message);
//         }
//
//         private static string[] GetCallerInfo()
//         {
//             StackTrace stackTrace = new StackTrace(true);
//             StackFrame callingFrame = stackTrace.GetFrame(2);  // 0 is GetCallerInfo, 1 is Log/Warn/Error/Success, 2 is the actual caller
//             System.Reflection.MethodBase method = callingFrame.GetMethod();
//
//             string script = method.ReflectedType.Name + ".cs";
//             string methodName = method.Name + "()";
//             string line = "Line: " + callingFrame.GetFileLineNumber();
//
//             return new string[] { "Script: " + script, "Method: " + methodName, line };
//         }
//     }
// }
