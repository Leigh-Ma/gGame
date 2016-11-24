using UnityEngine;
using System.IO;
using ProtoBuf;

namespace gNet{
	public static class gPB {

		public static T LoadFromResource<T>(string resourcePath)
		{
			TextAsset objAsset = Resources.Load (resourcePath, typeof(TextAsset)) as TextAsset;

			if (objAsset == null) {
				return default(T);
			}

			T deserializeObj = default(T);
			using(MemoryStream m = new MemoryStream(objAsset.bytes))
			{
				deserializeObj = (T)ProtoBuf.Serializer.Deserialize<T> (m);
			}

			return deserializeObj;
		}

		public static T LoadFromFile<T>(string objPath)
		{
			if(!File.Exists(objPath))
			{
				return default(T);
			}

			T deserializeObj = default(T);
			using (FileStream f = new FileStream (objPath, FileMode.Open))
			{
				deserializeObj = (T)ProtoBuf.Serializer.Deserialize<T> (f);
			}

			return deserializeObj;
		}

		public static void SaveToPath<T>(string objPath, string fileName, T obj)
		{
			if(!Directory.Exists(objPath))
			{
				Directory.CreateDirectory (objPath);
			}

			using (FileStream f = new FileStream (objPath + fileName, FileMode.OpenOrCreate))
			{
				ProtoBuf.Serializer.Serialize (f, obj);
			}

			return;
		}

		public static byte[] pbEncode<T>(T obj)
		{
			using(MemoryStream m = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize (m, obj);
				return m.ToArray ();
			}
		}

		public static T pbDecode<T> (byte[] bytes)
		{
			Debug.Log ("pbDecode: data len: " + bytes.Length.ToString ());
			using (MemoryStream m = new MemoryStream (bytes))
			{
				return (T)ProtoBuf.Serializer.Deserialize<T> (m);
			}
		}

	}
}