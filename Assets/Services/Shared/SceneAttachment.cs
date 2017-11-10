using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Services.Core
{
    public class SceneAttachment : MonoBehaviour
    {
        public const string GAME_OBJECT_NAME = "_sceneAttachment";
        private static SceneAttachment instance;
        private static readonly List<Action<bool>> pauseListeners = new List<Action<bool>>();
        private static readonly List<Action> quitListeners = new List<Action>();

        public static SceneAttachment GetInstance()
        {
            if (instance == null)
            {
                var gameObject = new GameObject(GAME_OBJECT_NAME);
                DontDestroyOnLoad(gameObject);
                instance = gameObject.AddComponent<SceneAttachment>();
            }

            return instance;
        }

        public static void ListenPause(Action<bool> callback)
        {
            pauseListeners.Add(callback);
        }

        public static void RemovePause(Action<bool> callback)
        {
            pauseListeners.Remove(callback);
        }

        public static void ListenQuit(Action callback)
        {
            quitListeners.Add(callback);
        }

        public static void RemoveQuit(Action callback)
        {
            quitListeners.Remove(callback);
        }

        public static void AttachCoroutine(IEnumerator routine)
        {
            instance.StartCoroutine(routine);
        }

        public static IEnumerator WaitCoroutine(IEnumerator routine)
        {
            yield return instance.StartCoroutine(routine);
        }

        public static void StopCoroutines()
        {
            instance.StopAllCoroutines();
        }

        void OnApplicationPause(bool pause)
        {
            foreach(var callback in pauseListeners)
                callback(pause);
        }

        void OnApplicationQuit()
        {
            foreach(var callback in quitListeners)
                callback();
        }
    }
}

