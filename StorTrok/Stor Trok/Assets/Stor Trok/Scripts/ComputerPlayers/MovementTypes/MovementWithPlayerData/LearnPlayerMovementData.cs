using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnPlayerMovementData : MonoBehaviour { //learns patterns for this object

	public float datapoint_learn_dtime = 1;
	private float last_time_datapoint_learned = -5;
	public GameObject learn_object;

	void Start () {
		Spaceship s = GetComponent<Spaceship> ();
		if (s != null) {
			s.immortal = true;
		}
	}

	void Update () {
		if (Time.time - last_time_datapoint_learned >= datapoint_learn_dtime) {
			learn_new_datapoint ();
			last_time_datapoint_learned = Time.time;
		}
	}

	void OnDestroy(){
		if (this.enabled) {
			PlayerMovementData.save_data_set ();
			print (PlayerMovementData.data_set.Count.ToString () + " data points saved");
		}
	}

	List<MovementInputKeys> get_rot_input(){
		List<MovementInputKeys> k = new List<MovementInputKeys> ();
		if (Input.GetKey (KeyCode.D)) k.Add(MovementInputKeys.D);
		if (Input.GetKey (KeyCode.W)) k.Add(MovementInputKeys.W);
		if (Input.GetKey (KeyCode.S)) k.Add(MovementInputKeys.S);
		if (Input.GetKey (KeyCode.A)) k.Add(MovementInputKeys.A);
		return k;
	}
	List<MovementInputKeys> get_speed_input(){
		List<MovementInputKeys> k = new List<MovementInputKeys> ();
		if (Input.GetKey (KeyCode.Q)) k.Add (MovementInputKeys.Q);
		if (Input.GetKey (KeyCode.E))k.Add (MovementInputKeys.E);
		return k;
	}
	void learn_new_datapoint(){
		
		Vector3 r_pos = learn_object.transform.position - transform.position;
		Vector3 rot = learn_object.transform.rotation.eulerAngles;

		SpaceshipRelativeSituation rel_sit = new SpaceshipRelativeSituation ();
		rel_sit.relative_position = new SerializableVector3(r_pos);
		rel_sit.rotation = new SerializableVector3(rot);

		SpaceshipInput input = new SpaceshipInput ();
		input.rotation_input = get_rot_input ();
		//input.speed_input = get_speed_input ();
		input.speed = learn_object.GetComponent<Spaceship>().speed;

		PlayerMovementData data = new PlayerMovementData ();
		data.player_input = input;
		data.player_situation = rel_sit;
		PlayerMovementData.data_set.Add (data);
	}
}
