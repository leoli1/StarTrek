using UnityEngine;
using System.Collections;

public class tObjScript : MonoBehaviour {

	Rigidbody rigidbody;
	public GameObject schild;

	void Start () {
		rigidbody = this.GetComponent<Rigidbody> ();
		rigidbody.AddForce (new Vector3 (-100, 0, 0));
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.name == "Schild") {
			ContactPoint contact = collision.contacts [0];
			schild.GetComponent<SchildScript> ().col (contact.point);
			Destroy (gameObject);
		}
	}
}
