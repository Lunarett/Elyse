using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;

namespace Pulsar.Debug
{
    public static class DebugUtils
    {
        private static LogDisplayManager _display;

        private static void CreateAndDisplayMessage(string message, float duration, Color color, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            if (_display == null)
            {
                GameObject displayObject = new GameObject("MessageDisplayManager");
                _display = displayObject.AddComponent<LogDisplayManager>();
            }

            StackTrace stackTrace = new StackTrace(true);
            StackFrame frame = stackTrace.GetFrame(2);
            var method = frame.GetMethod();

            string className = method.DeclaringType.Name;
            string methodName = method.Name;
            int line = frame.GetFileLineNumber();

            string formattedMessage = $"{callerName} {className}::{methodName} Line: {line} - {message}";
            _display.AddMessage(formattedMessage, duration, color);
        }

        public static void Log(string message, float duration = 5f)
        {
            UnityEngine.Debug.Log(message);
            CreateAndDisplayMessage(message, duration, Color.cyan);
        }

        public static void Warn(string message, float duration = 5f)
        {
            UnityEngine.Debug.LogWarning(message);
            CreateAndDisplayMessage($"Warning! {message}", duration, Color.yellow);
        }

        public static void Error(string message, float duration = 5f)
        {
            UnityEngine.Debug.LogError(message);
            CreateAndDisplayMessage($"Error! {message}", duration, Color.red);
        }

        public static void Success(string message, float duration = 5f)
        {
            UnityEngine.Debug.Log(message);
            CreateAndDisplayMessage($"Success! {message}", duration, Color.green);
        }

        public static void Print(string message, Color color, float duration = 5f)
        {
            UnityEngine.Debug.Log(message);
            CreateAndDisplayMessage(message, duration, color);
        }

        public static bool CheckForNull<T>(T objectToCheck, string message = "", float duration = 5.0f) where T : class
        {
            if (objectToCheck != null) return false;
            Error(string.IsNullOrEmpty(message) ? $"The class {typeof(T).Name} has returned null!" : message, duration);
            return true;
        }

        public static bool CheckValidListIndex<T>(List<T> list, int index, string message = "", float duration = 5.0f)
        {
            if (list != null && index >= 0 && index < list.Count) return true;
            Error(string.IsNullOrEmpty(message) ? $"List error for {typeof(T).Name} at index {index}." : message, duration);
            return false;
        }

        public static bool CheckRange(float value, float min, float max, string message = "", float duration = 5.0f)
        {
            if (!(value < min) && !(value > max)) return true;
            Error(string.IsNullOrEmpty(message) ? $"Value {value} out of range ({min} to {max})." : message, duration);
            return false;
        }
    }
}