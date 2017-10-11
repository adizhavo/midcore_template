namespace Framework.Log
{
    public static class LogWrapper 
    {
        public static void DebugLog(string message)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.Log(message);
            #endif
        }

        public static void DebugError(string message)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogError(message);
            #endif
        }

        public static void DebugWarning(string message)
        {
            #if ADHOC || UNITY_EDITOR
            UnityEngine.Debug.LogWarning(message);
            #endif
        }

        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void Warning(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}