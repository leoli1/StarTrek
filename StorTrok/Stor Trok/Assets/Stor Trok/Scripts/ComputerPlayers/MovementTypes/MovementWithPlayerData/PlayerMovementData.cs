using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementInputKeys {
	W,A,S,D,E,Q
}
[System.Serializable]
public struct SpaceshipInput{
	public List<MovementInputKeys> rotation_input;
	//public List<MovementInputKeys> speed_input;
	public float speed;

}

[System.Serializable]
public struct SpaceshipRelativeSituation{
	public SerializableVector3 relative_position;
	public SerializableVector3 rotation;

	public const float position_weight = 3;
	public const float rotation_weight = 1;
}

[System.Serializable]
public class PlayerMovementData {

	public static List<PlayerMovementData> data_set = new List<PlayerMovementData>();
	public static string data_path;
	public static bool data_set_loaded = false;
	public SpaceshipInput player_input;
	public SpaceshipRelativeSituation player_situation;

	public static void load_data_set(){
		List<PlayerMovementData> data = SaveData.ReadFromFile<List<PlayerMovementData>> (data_path);
		PlayerMovementData.data_set = data;
		data_set_loaded = true;
		Debug.Log (data.Count.ToString() + " player movement data points");
	}
	public static void save_data_set(){
		SaveData.SaveToFile (data_path, data_set);
	}

	public static float compare_player_movement_data (PlayerMovementData data1, PlayerMovementData data2){
		return data1.compare_player_movement_data (data2);
	}
	public float compare_player_movement_data(PlayerMovementData data){
		return this.compare_player_movement_data (data.player_situation);
	}
		
	public float compare_player_movement_data(SpaceshipRelativeSituation sit){
		float pos_val = Vector3.Distance (this.player_situation.relative_position.unity_vector3, sit.relative_position.unity_vector3);
		float pos_weight = 1/SpaceshipRelativeSituation.position_weight;
		float rot_val = Vector3.Distance (this.player_situation.rotation.unity_vector3, sit.rotation.unity_vector3);
		float rot_weight = 1/SpaceshipRelativeSituation.rotation_weight;

		return (pos_weight * pos_val + rot_weight * rot_val) / (pos_weight + rot_weight);
	}

	public static PlayerMovementData find_best_match(SpaceshipRelativeSituation situation){
		PlayerMovementData best_match = null;
		float best_val = -1;
		foreach (PlayerMovementData data_point in data_set) {
			float val = data_point.compare_player_movement_data (situation);
			if (best_val == -1 || val < best_val) {
				best_val = val;
				best_match = data_point;
			}
		}
		return best_match;
	}
}
