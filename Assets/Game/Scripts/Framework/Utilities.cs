using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Framework.Utils
{
    public static class Util 
    {
        private static volatile object writeLock = new object();

        public static T ReadBinary<T>(string path)
        {
            var binaries = File.ReadAllBytes(path);
            return Util.ByteArrayTo<T>(binaries);
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

        private static void WriteObjectAtPath(object obj, string path)
        {
            lock (writeLock)
            {
                var b = Util.ObjectToByteArray(obj);
                File.WriteAllBytes(path, b);
            }
        }
    }
}