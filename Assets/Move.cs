using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	public float speed;
	public float agility;

	private float walkSpeed;
	private float curSpeed;

	void Start() {
		walkSpeed = (float)(speed + (agility/5));
	}

	void FixedUpdate() {
		curSpeed = walkSpeed;

		// Move senteces
		gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Lerp(0, Input.GetAxis("Horizontal")* curSpeed, 0.8f),
		                                    			Mathf.Lerp(0, Input.GetAxis("Vertical")* curSpeed, 0.8f));
	}
}
