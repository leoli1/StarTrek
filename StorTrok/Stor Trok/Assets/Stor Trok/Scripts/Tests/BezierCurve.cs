using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour {

	public int number_of_points_on_curve = 100;
	public Transform[] controll_points = new Transform[0];


	private LineRenderer line_renderer;
	private int[] coefficients;

	void Awake () {
		line_renderer = GetComponent<LineRenderer> ();
		calc_coefficients (1);
	}

	void Update () {
		if (controll_points.Length > 1) {
			calculate_curve ();
		}
	}

	void calc_coefficients(int degree){
		coefficients = new int[degree + 1];
		for (int i = 0; i < degree + 1; i++) {
			coefficients [i] = binom_coefficient (degree, i);
		}
	}

	int fak(int n){
		return n == 0 ? 1 : n * (fak (n - 1));
	}
	int binom_coefficient(int n, int k){ // n über k
		return fak(n)/(fak(k)*fak(n-k));
	}

	Vector3 get_point_coordinate(float t){
		Vector3 p = Vector3.zero;
		for (int i = 0; i < coefficients.Length; i++) {
			p += controll_points [i].position * Mathf.Pow (t, i) * Mathf.Pow (1 - t, coefficients.Length-i-1) * coefficients [i];// bernsteinpolynom
		}
		return p;
	}

	void calculate_curve(){
		if (controll_points.Length != coefficients.Length)
			calc_coefficients (controll_points.Length - 1);
		
		line_renderer.positionCount = number_of_points_on_curve;

		Vector3[] positions = new Vector3[number_of_points_on_curve];
		for (int i = 0; i < number_of_points_on_curve; i++) {
			positions [i] = get_point_coordinate ((float)i / number_of_points_on_curve);
		}
		line_renderer.SetPositions (positions);
	}
}
