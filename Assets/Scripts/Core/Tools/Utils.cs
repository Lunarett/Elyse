using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;

namespace Pulsar.Utils
{
    public static class Utils
    {
        public static void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (child == null)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
        
        public static bool CheckForNull<T>(T objectToCheck) where T : class
        {
            if (objectToCheck != null) return false;
            StackTrace stackTrace = new StackTrace(true);
            StackFrame frame = stackTrace.GetFrame(1);
            var method = frame.GetMethod();
            string callingClassName = method.DeclaringType.Name;
            string className = typeof(T).Name;
            string methodName = method.Name;
            int line = frame.GetFileLineNumber();

            UnityEngine.Debug.LogError($"{callingClassName}: Failed to get {className}. Called from {methodName} at line {line}.");
            return true;
        }

    }

}
