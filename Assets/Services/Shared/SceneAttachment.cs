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
        private readonly List<Action<bool>> pauseListeners = new List<Action<bool>>();
        private readonly List<Action> quitListeners = new List<Action>();

        private bool booted;

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

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            booted = true;
        }

        public static void ListenPause(Action<bool> callback)
        {
            GetInstance().pauseListeners.Add(callback);
        }

        public static void RemovePause(Action<bool> callback)
        {
            GetInstance().pauseListeners.Remove(callback);
        }

        public static void ListenQuit(Action callback)
        {
            GetInstance().quitListeners.Add(callback);
        }

        public static void RemoveQuit(Action callback)
        {
            GetInstance().quitListeners.Remove(callback);
        }

        public static void AttachCoroutine(IEnumerator routine)
        {
            GetInstance().StartCoroutine(routine);
        }

        public static IEnumerator WaitCoroutine(IEnumerator routine)
        {
            yield return GetInstance().StartCoroutine(routine);
        }

        public static void StopCoroutines()
        {
            GetInstance().StopAllCoroutines();
        }

        void OnApplicationPause(bool pause)
        {
            if (booted)
            {
                foreach (var callback in pauseListeners)
                    callback(pause);
            }
        }

        void OnApplicationQuit()
        {
            if (booted)
            {
                foreach (var callback in quitListeners)
                    callback();
            }
        }
    }
}

