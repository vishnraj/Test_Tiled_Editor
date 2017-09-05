using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public GameObject player;
	public float adjust_player_radius;

	Vector3 offset;
	bool has_reached_edge;

	// Use this for initialization
	void Start() {
		has_reached_edge = false;
	}
 
	bool is_touching_viewport_edge() {
		Vector2 max_camera_coord = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		Vector2 min_camera_coord = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));	

		bool reached_max_x = player.transform.position.x + player.GetComponent<CircleCollider2D>().radius / adjust_player_radius >= max_camera_coord.x;
		bool reached_max_y = player.transform.position.y + player.GetComponent<CircleCollider2D>().radius / adjust_player_radius >= max_camera_coord.y;
		bool reached_min_x = player.transform.position.x - player.GetComponent<CircleCollider2D>().radius / adjust_player_radius <= min_camera_coord.x;
		bool reached_min_y = player.transform.position.y - player.GetComponent<CircleCollider2D>().radius / adjust_player_radius <= min_camera_coord.y;

		if (reached_max_x || reached_max_y || reached_min_x || reached_min_y) {
			return true;
		}

		return false;	
	}

	void Update() {
		if (is_touching_viewport_edge()) {
			if (!has_reached_edge) {
				Vector3 local_offset;
				local_offset = transform.position - player.transform.position;
				offset = new Vector3(local_offset.x, local_offset.y, local_offset.z);
			}
			has_reached_edge = true;
		} else {
			has_reached_edge = false;
		}
	}	

	void move_camera() {
		Vector3 position = player.transform.position + offset;
		transform.position = position;
	}	

	// LateUpdate is called after Update each frame
	void LateUpdate() {
		if (has_reached_edge) {
			move_camera();
		}
	}
}
