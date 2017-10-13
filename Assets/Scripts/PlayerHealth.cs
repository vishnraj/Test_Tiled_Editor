using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
    public GameObject player;
    public float health;

    public GameObject HUD;
    private Transform player_icon;
    private Transform health_remaining;

    // Use this for initialization
    private void Start() {
        player_icon = HUD.transform.Find("PlayerIcon");
        health_remaining = HUD.transform.Find("HealthRemaining");

        update_health_remaining();
    }



    // Update is called once per frame
    private void Update() {
        update_health_remaining();

        if (health <= 0) {
            Destroy(gameObject);
        }
    }

    private void update_health_remaining() {
        health_remaining.gameObject.GetComponent<Text>().text = "Health Remaining: " + health.ToString();

        if (health > 5) {
            health_remaining.gameObject.GetComponent<Text>().color = new Color(0, 255, 255);
        } else {
            health_remaining.gameObject.GetComponent<Text>().color = new Color(255, 0, 0);
        }

        player_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(player),
            new Rect(0, 0, 128, 128), new Vector2());
    }
}
