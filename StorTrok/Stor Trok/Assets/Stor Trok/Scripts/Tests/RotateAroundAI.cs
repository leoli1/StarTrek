using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAI : MonoBehaviour {

	//float angle = 0;
	float radius = 0;
	public GameObject center;
	public float velo = 1;
	void Start () {
		radius = (transform.position - center.transform.position).magnitude;
		//angle = Vector3.Angle (transform.position - center.transform.position, Vector3.right);
	}

	Vector3 get_pos(float angle){
		return new Vector3 (radius * Mathf.Cos (angle), 0, radius * Mathf.Sin (angle));
	}

	// Update is called once per frame
	void Update () {
		//angle = Vector3.Angle (transform.position - center.transform.position, Vector3.right)+Time.deltaTime;

		//radius = (transform.position - center.transform.position).magnitude;
		//this.GetComponent<Rigidbody> ().velocity = new Vector3 (radius * (-Mathf.Sin (angle)), 0, radius * Mathf.Cos (angle));
		//this.GetComponent<Rigidbody>().MovePosition(this.transform.position + Vector3.left*Time.deltaTime);
		//angle += Time.deltaTime;
		/*float current_angle = Vector3.Angle (transform.position - center.transform.position, Vector3.right);
		float new_angle = current_angle + Time.deltaTime;
		print (current_angle);
		print (new_angle);
		Vector3 new_pos = get_pos (new_angle);
		this.transform.position = new_pos;
		//this.GetComponent<Rigidbody> ().MovePosition (new_pos);
		print (this.GetComponent<Rigidbody> ().velocity);*/
		/*Vector3 up = Vector3.up;//uQuaternion.AngleAxis (90, transform.position - center.transform.position) * center.transform.right;
		Vector3 new_look_dir = Quaternion.AngleAxis(90, up)*( transform.position - center.transform.position);
		transform.rotation = Quaternion.LookRotation (new_look_dir);*/
		transform.LookAt (center.transform.position);
		transform.Rotate (transform.up, 90);
		//transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(90, transform.up)*transform.eulerAngles);

		GetComponent<Rigidbody> ().velocity = transform.forward.normalized * velo;
	}
	void OnDrawGizmos(){
		Gizmos.DrawLine (transform.position, transform.position + transform.up * 2f);
		Gizmos.DrawLine (transform.position, transform.position + transform.forward * 3f);
	}
}
