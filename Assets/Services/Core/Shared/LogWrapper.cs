namespace Services.Core
{
    public static class LogWrapper 
    {
        public static void DebugLog(string message)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.Log(message);
            #endif
        }

        public static void DebugLog(string message, params object[] args)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogFormat(message, args);
            #endif
        }

        public static void DebugError(string message)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogError(message);
            #endif
        }

        public static void DebugError(string message, params object[] args)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogErrorFormat(message, args);
            #endif
        }

        public static void DebugWarning(string message)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogWarning(message);
            #endif
        }

        public static void DebugWarning(string message, params object[] args)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogWarningFormat(message, args);
            #endif
        }

        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void Log(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void Error(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void Warning(string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(message, args);
        }
    }
}