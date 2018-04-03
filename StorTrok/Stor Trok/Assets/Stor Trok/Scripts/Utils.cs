using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum BlendMode{
	Opaque,
	Cutout,
	Fade,
	Transparent
}
public static class Utils {

	public static bool is_destroyed(this GameObject go){
		Spaceship s = Spaceship.get_spaceship (go);
		if (s != null)
			return s.destroyed;
		DestroyableObject o = DestroyableObject.get_destroyable_object (go);
		if (o != null)
			return o.destroyed;
		return !go.activeInHierarchy;
	}

	public static bool item_is_null(Item i){
		return i == null || i.ID == 0;
	}

	public static List<Object> GetScriptAssetsOfType<T>()
	{
		//MonoScript[] scripts = (MonoScript[])Object.FindObjectsOfTypeIncludingAssets( typeof( MonoScript ) );
		Object[] scripts = Resources.LoadAll("GameData/Items");//( typeof( MonoScript ) );

		List<Object> result = new List<Object>();

		foreach( Object m in scripts )
		{
			if( m != null && m.GetType().IsSubclassOf( typeof( T ) ) || m.GetType()==typeof(T) )
			{
				result.Add(m);
			}
		}
		return result;
	}

	public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
	{
		switch (blendMode) {
		case BlendMode.Opaque:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt ("_ZWrite", 1);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = -1;
			break;
		case BlendMode.Cutout:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt ("_ZWrite", 1);
			material.EnableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 2450;
			break;
		case BlendMode.Fade:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt ("_ZWrite", 0);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.EnableKeyword ("_ALPHABLEND_ON");
			material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		case BlendMode.Transparent:
			material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt ("_ZWrite", 0);
			material.DisableKeyword ("_ALPHATEST_ON");
			material.DisableKeyword ("_ALPHABLEND_ON");
			material.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		}
	}

	public static Vector3 get_random_point_on_path(List<Transform> path){
		int len = path.Count;
		float p = Random.value;
		float path_length = 0;
		for (int i = 1; i < len; i++) {
			path_length += Vector3.Distance (path [i - 1].position, path [i].position);
		}
		float point = path_length * p;

		float cur_path_length = 0;
		for (int i = 1; i < len; i++) {
			float add_dist = Vector3.Distance (path [i - 1].position, path [i].position);
			float prev_path_length = cur_path_length;
			cur_path_length += add_dist;
			if (point <= cur_path_length) {
				float perc_in_abschnitt = (point - prev_path_length) / (cur_path_length - prev_path_length);
				return Vector3.Lerp (path [i - 1].position, path [i].position, perc_in_abschnitt);
			}
		}
		return Vector3.zero;

	}

	public static bool point_in_rect(Vector2 point, Rect rect){
		return point.x >= rect.xMin && point.x <= rect.xMax && point.y >= rect.yMin && point.y <= rect.xMax;
	}
	public static float clamp_angle(float angle, float min, float max){
		float x = angle;
		if (Mathf.Abs (x - 360) < Mathf.Abs (x)) {
			x = Mathf.Clamp (x, 360+min, 360);
		} else {
			x = Mathf.Clamp (x, 0, max);
		}
		return x;
	}

	public static string string_mul_int(int mul, string s){
		string news = "";
		for (int i = 0; i < mul; i++) {
			news += s;
		}
		return news;
	}

	public static string get_mark_text(int number){
		string s = "";
		if (number <= 0)
			return "";
		
		if (number < 4) {
			s += string_mul_int (number, "I");//number * "I";
		} else if (number < 5) {
			s += "IV";
		} else if (number < 8) {
			s += "V" + string_mul_int (number, "I");//+ number * "I";
		} else if (number < 11) {
			s += string_mul_int (10 - number, "I") + "X";//(10 - number) * "I" + "X";
		} else if (number < 14) {
			s += string_mul_int (number - 10, "I") + "X";//(number - 10) * "I" + "X";
		}
		return s;
	}

	public static GameObject get_gameobject_in_children(GameObject parent, string child_name){
		Transform[] children = parent.GetComponentsInChildren<Transform> ();
		foreach (Transform trans in children) {
			if (trans.gameObject.name == child_name) {
				return trans.gameObject;
			}
		}
		return null;
	}

	public static bool is_spaceship(GameObject go){
		return Spaceship.get_spaceship (go) != null;
	}
	public static bool is_destroyable_object(GameObject go){
		return go.GetComponent<DestroyableObject> () != null;
	}
/*	public static bool is_destroyed(GameObject go){
		if (is_spaceship (go)) {
			return Spaceship.get_spaceship(go).destroyed;
		} else if (is_destroyable_object (go)) {
			return DestroyableObject.get_destroyable_object (go).destroyed;
		}
		return false;
	}*/
	public static Vector3 random_vector3(float length){
		return Random.onUnitSphere * length;
		//return (new Vector3 (Random.Range (-1, 1), Random.Range (-1, 1), Random.Range (-1, 1))).normalized * length;
	}
	/*public static void apply_damage(GameObject g, float dmg, float dmg_to_shields){
		if (is_spaceship (g)) {
			Spaceship s = Spaceship.get_spaceship (g);
			s.apply_damage(
		}
	}*/
}

public class Range{
	public float min;
	public float max;

	public float medium{
		get{ 
			return (min + max) * 0.5f;
		}
	}

	public Range(float min, float max){
		this.min = min;
		this.max = max;
	}
	public bool contains_number(float number){
		return number >= min && number <= max;
	}

}

public class AngleRange{
	public float min;
	public float max;
	public AngleRange(float min, float max){
		this.min = min;
		this.max = max;
	}
	public bool contains_number(float number){
		if (number < 0) {
			number = 360 + number;
		}
		return number >= min && number <= max;
	}
}