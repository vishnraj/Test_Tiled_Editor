using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Utility;

// Will determine how items are picked up and assigned 
// to player later as well as notifying other systems
// about these items 
public class ItemSystem2D : MonoBehaviour 
{
    public string equipped;
    public GameObject current_ammo_type;
    public GameObject none;
    public int current_ammo_amount;
    public GameObject HUD;
    public int max_items;
    public int start_location_x;
    public int start_location_y;
    public int offset_x;

    private List<string> inventory;
    private List<GameObject> menu_objects;
    private Transform equipped_icon;
    private Transform equipped_status;
    private Transform ammo_icon;
    private Transform ammo_remaining;

    // Use this for initialization
    private void Start() {
        inventory = new List<string>();
        menu_objects = new List<GameObject>();

        equipped_icon = HUD.transform.Find("EquippedIcon");
        equipped_status = HUD.transform.Find("EquippedStatus");

        ammo_icon = HUD.transform.Find("AmmoIcon");
        ammo_remaining = HUD.transform.Find("AmmoRemaining");

        max_items = 3;

        update_equipped();
    }

    // Update is called once per frame
    private void Update() {
        display_item_selection();
        update_equipped();
        unequip_item();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject item = other.gameObject;
        if (item.GetComponent<Utility.Item>() != null) {
            // really this will later transfer the ownership to
            // the inventory system, which from there the player
            // will actually equip the item - but for now, we will
            // just test by equipping it here

            // we cannot exceed the max_items that player
            // can currently equip
            // we also cannot add two of the same item - instead
            // this wil be considered ammo at a later step
            if (inventory.Count < max_items && !inventory.Contains(item.name)) {
                inventory.Add(item.name);
            }
            Destroy(item);
        }      
    }

    private void unequip_item() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            equipped = "";
        }
    }

    private void set_item_menu_hud(string name, int current_index) {
        // create the object 
        // add rect transform and set the position
        // add image and set the sprite
        // set transform parent to UI
        // add it to menu_objects for later destruction

        GameObject ui_component = new GameObject();
        ui_component.transform.SetParent(HUD.transform);
        ui_component.AddComponent<RectTransform>();
        RectTransform rt = ui_component.GetComponent<RectTransform>();
        rt.position = new Vector3(start_location_x + current_index * offset_x, start_location_y, 0);
        rt.anchorMax = new Vector2(.5f, .5f);
        rt.anchorMin = new Vector2(.5f, .5f);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x - .5f * rt.sizeDelta.x, rt.sizeDelta.y - .5f * rt.sizeDelta.y);
        ui_component.AddComponent<Image>();
        ui_component.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(Resources.Load("Prefabs/" + name)),
            new Rect(0, 0, 128, 128), new Vector2()); ; // this needs to be configured
                                                        // size of textures can differ from sprite to sprite
        menu_objects.Add(ui_component);
    }

    private void display_item_selection() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Time.timeScale = 0; // pause game upon entering
                                // menu

            // For now, given start_location
            // near the left of the screen
            // we will place each item prefab
            // texture apart based on offset
            // from each subsequent texture
            // for blank items, we use none

            // Removal of the item from the list
            // below should shift all indexes to
            // the left - therefore it should
            // always be fine to iterate over
            // the list from left to right

            if (menu_objects.Count != max_items) {
                int num_items = 0;
                foreach (string i in inventory) {
                    // only for the intial case where we have nothing equipped
                    // we would automatically highlight and select the very
                    // first menu item - in later cases, we would start
                    // with the location of what is equipped and let the
                    // player cycle through items to pick desired item
                    if (num_items == 0 && equipped == "") {
                        equipped = i;
                    }

                    set_item_menu_hud(i, num_items);

                    ++num_items;
                }
                for (; num_items < max_items; ++num_items) {
                    set_item_menu_hud(none.name, num_items);
                }
            }
        } else if (Input.GetKeyUp(KeyCode.E)) {
            Time.timeScale = 1; // start game upon exiting
                                // menu

            foreach (GameObject g in menu_objects) {
                Destroy(g);
            }
            menu_objects.Clear();
        }
    }

    private void update_ammo_remaining() {
        ammo_remaining.gameObject.GetComponent<Text>().text = "Ammo Amount: " + current_ammo_amount.ToString();

        if (current_ammo_amount > 5) {
            ammo_remaining.gameObject.GetComponent<Text>().color = new Color(0, 255, 255);
        } else {
            ammo_remaining.gameObject.GetComponent<Text>().color = new Color(255, 0, 0);
        }

        ammo_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(current_ammo_type),
            new Rect(0, 0, 128, 128), new Vector2());
    }

    private void set_equipped_none() {
        equipped_status.gameObject.GetComponent<Text>().text = "None equipped";
        equipped_status.gameObject.GetComponent<Text>().color = new Color(255, 255, 0);


        equipped_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(none),
            new Rect(0, 0, 128, 128), new Vector2());
    }

    private void set_ammo_none() {
        ammo_remaining.gameObject.GetComponent<Text>().text = "Ammo not set ";
        ammo_remaining.gameObject.GetComponent<Text>().color = new Color(255, 255, 0);

        ammo_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(none),
            new Rect(0, 0, 128, 128), new Vector2());
    }

    public void update_equipped() {
        if (equipped == "") {
            set_equipped_none();
            set_ammo_none();
            return;
        }

        switch (equipped) {
            case "Raygun":
                equipped_status.gameObject.GetComponent<Text>().text = "Raygun equipped";
                equipped_status.gameObject.GetComponent<Text>().color = new Color(0, 255, 255);

                if (current_ammo_type == null) {
                    set_ammo_none(); // player can set later
                    break;
                }

                update_ammo_remaining();
                break;

            default:
                break;
        }

        equipped_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(Resources.Load("Prefabs/" + equipped)),
            new Rect(0, 0, 128, 128), new Vector2());
    }
}
