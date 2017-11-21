﻿using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace Services.Core
{
    public static class Utils
    {
        private static volatile object writeLock = new object();

        public static T ReadBinary<T>(string path)
        {
            var binaries = File.ReadAllBytes(path);
            return Utils.ByteArrayTo<T>(binaries);
        }

        public static void WriteBinary(object obj, string path, bool seperateThread = true)
        {
            if (seperateThread)
            {
                var t = new Thread(() => WriteObjectAtPath(obj, path));
                t.Start();
            }
            else
            {
                WriteObjectAtPath(obj, path);
            }
        }

        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ByteArrayTo<T>(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return (T)obj;
            }
        }

        public static T ReadFromResources<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }

        public static UnityEngine.Object ReadFromResources(string path, Type type)
        {
            return Resources.Load(path, type);
        }

        public static T ReadJsonFromResources<T>(string path)
        {
            var json = Resources.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<T>(json.text);
        }

        public static object ReadJsonFromResources(string path, Type type)
        {
            var json = Resources.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject(json.text, type);
        }

        public static T DeserializeJson<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static object DeserializeJson(string text, Type type)
        {
            return JsonConvert.DeserializeObject(text, type);
        }

        private static void WriteObjectAtPath(object obj, string path)
        {
            lock (writeLock)
            {
                var b = Utils.ObjectToByteArray(obj);
                File.WriteAllBytes(path, b);
                LogWrapper.Log("binaries successfully saved to: {1}", path);
            }
        }

        public static T[] ReadAllJsonResourcesDirectory<T>(string directoryPath)
        {
            var jsons = Resources.LoadAll<TextAsset>(directoryPath);

            if (jsons != null)
            {
                T[] files = new T[jsons.Length];
                for (int i = 0; i < jsons.Length; i++)
                    files[i] = JsonConvert.DeserializeObject<T>(jsons[i].text);
				
                return files;
            }
            else throw new UnityException("Directory not found: " + directoryPath);
        }

        public static string GenerateUniqueId()
        {
            var ticks = DateTime.Now.Ticks;
            var guid = Guid.NewGuid().ToString();
            var uniqueId = ticks.ToString() +'-'+ guid;
            return uniqueId;
        }

        public static Vector2 GetScreenPos(Services.Core.Rect rect, UIAnchor anchor)
        {
            switch (anchor)
            {
                case UIAnchor.CENTER : return new Vector2(Screen.width / 2 + (rect.x - rect.width / 2), Screen.height / 2 + (rect.y  - rect.height / 2));
                case UIAnchor.CENTER_BOTTOM : return new Vector2(Screen.width / 2 + (rect.x - rect.width / 2), rect.y);
                case UIAnchor.CENTER_LEFT : return new Vector2(rect.x, Screen.height / 2 + (rect.y  - rect.height / 2));
                case UIAnchor.CENTER_RIGHT : return new Vector2(Screen.width - (rect.x + rect.width), Screen.height / 2 + (rect.y  - rect.height / 2));
                case UIAnchor.CENTER_TOP : return new Vector2(Screen.width / 2 + (rect.x - rect.width / 2), Screen.height - (rect.y + rect.height));
                case UIAnchor.LEFT_BOTTOM : return new Vector2(rect.x, rect.y);
                case UIAnchor.LEFT_TOP : return new Vector2(rect.x, Screen.height - (rect.y + rect.height));
                case UIAnchor.RIGHT_BOTTOM : return new Vector2(Screen.width - (rect.x + rect.width), rect.y);
                case UIAnchor.RIGHT_TOP : return new Vector2(Screen.width - (rect.x + rect.width), Screen.height - (rect.y + rect.height));
            }
            return Vector2.zero;
        }

        public static void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments, Color color, float duration = 0)
        {
            float angle = 0f;
            Quaternion rot = Quaternion.LookRotation(forward, up);
            Vector3 lastPoint = Vector3.zero;
            Vector3 thisPoint = Vector3.zero;

            for (int i = 0; i < segments + 1; i++)
            {
                thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
                thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

                if (i > 0)
                {
                    Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
                }

                lastPoint = thisPoint;
                angle += 360f / segments;
            }
        }
    }
}