using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class SerializableVector3{
	public float x;
	public float y;
	public float z;

	public Vector3 unity_vector3{
		get{ 
			return new Vector3 (x, y, z);
		}
	}
	public SerializableVector3(){
		x = 0;
		y = 0;
		z = 0;
	}
	public SerializableVector3(Vector3 v){
		x = v.x;
		y = v.y;
		z = v.z;
	}
}

public class SaveData { // standard methoden, zum speichern/lesen von dateien mithilfe des binaryformatters
	public static void SaveToFile<T>(string path, T obj){
		using (Stream s = File.Open (path, FileMode.Create)) {
			var f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
			f.Serialize (s, obj);
		} 
	}
	public static T ReadFromFile<T>(string path) {
		try{
			using (Stream s = File.Open (path, FileMode.Open)) {
				var f = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ();
				return (T)f.Deserialize (s);
			}
		} catch (FileNotFoundException){
			return default(T);
		}
	}
	public static bool file_exists(string path){
		return File.Exists (path);
	}
}
