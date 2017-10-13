using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	public float speed;
	public float agility;

	private float walk_speed;
	private float cur_speed;

	private void Start() {
		walk_speed = (float)(speed + (agility/5));
	}

	private void FixedUpdate() {
		cur_speed = walk_speed;

		// Move senteces
		gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Lerp(0, Input.GetAxis("Horizontal")* cur_speed, 0.8f),
		                                    			Mathf.Lerp(0, Input.GetAxis("Vertical")* cur_speed, 0.8f));
	}
}
