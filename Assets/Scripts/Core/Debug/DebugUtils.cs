using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Debug
{
    public static class DebugUtils
    {
        public static bool CheckForNull<T>(T objectToCheck) where T : class
        {
            if (objectToCheck != null) return false;
            
            LogErrorFromCaller($"Failed to get {typeof(T).Name}");
            return true;
        }

        public static bool CheckValidListIndex<T>(List<T> list, int index)
        {
            if (index >= 0 && index < list.Count) return true;

            LogErrorFromCaller($"Index {index} is out of range for list of type {typeof(T).Name} with size {list.Count}");
            return false;
        }

        public static void LogErrorFromCaller(string errorMessage)
        {
            StackTrace stackTrace = new StackTrace(true);
            StackFrame frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();
            string callingClassName = method.DeclaringType.Name;
            string methodName = method.Name;
            int line = frame.GetFileLineNumber();

            UnityEngine.Debug.LogError($"{callingClassName}: {errorMessage} Called from {methodName} at line {line}.");
        }
        
        public static bool CheckRange(float value, float min, float max)
        {
            if (value >= min && value <= max) return true;

            LogErrorFromCaller($"Value {value} is out of range. Expected between {min} and {max}.");
            return false;
        }
        
        public static void LogAllListItems<T>(List<T> list)
        {
            string items = string.Join(", ", list);
            UnityEngine.Debug.Log($"List items: {items}");
        }
        
        public static void LogAllArrayItems<T>(T[] array)
        {
            string items = string.Join(", ", array);
            UnityEngine.Debug.Log($"Array items: {items}");
        }
        
        public static void TimeMethodExecution(Action action)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Method execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}