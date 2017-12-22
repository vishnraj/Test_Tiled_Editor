using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Utility;

// Will determine how items are picked up and assigned 
// to player later as well as notifying other systems
// about these items 
public class ItemSystem2D : MonoBehaviour {
    public string equipped;
    public string current_ammo_type;
    public GameObject none;
    public int current_ammo_amount;
    public GameObject HUD;
    public Sprite selection;
    public int max_items;
    public float offset_x;

    private bool item_menu_on;
    private int current_inventory_index;
    private int equipped_index;
    private float start_location_x;
    private float start_location_y;
    private float start_location_z;
    private List<string> inventory;
    private Dictionary<string, int> ammo_amounts;
    private List<GameObject> menu_objects;
    private Transform equipped_icon;
    private Transform equipped_status;
    private Transform ammo_icon;
    private Transform ammo_remaining;
    private GameObject highlighter;

    // Use this for initialization
    private void Start() {
        equipped = current_ammo_type = "";
        inventory = new List<string>();
        ammo_amounts = new Dictionary<string, int>();
        menu_objects = new List<GameObject>();

        equipped_icon = HUD.transform.Find("EquippedIcon");
        equipped_status = HUD.transform.Find("EquippedStatus");

        ammo_icon = HUD.transform.Find("AmmoIcon");
        ammo_remaining = HUD.transform.Find("AmmoRemaining");

        max_items = 3;
        equipped_index = current_inventory_index = -1;

        update_equipped();
    }

    // Update is called once per frame
    private void Update() {
        toggle_item_menu();
        cycle_through_items();
        equip_item();
        unequip_item();
        drop_item();
        update_equipped();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject item = other.gameObject;
        if (item.GetComponent<Utility.Item>() != null) {
            pick_up(item);
        }
    }
    

    private void OnTriggerStay2D(Collider2D other) {
        GameObject item = other.gameObject;
        if (item.GetComponent<Utility.Item>() != null) {
            pick_up(item);
        }
    }

    private void pick_up(GameObject item) {
        // can only happen during
        // active game state
        if (item_menu_on) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (inventory.Count < max_items && !inventory.Contains(item.name)) {
                string name = item.name;
                if (name.Contains("("))
                    name =  name.Substring(0, name.LastIndexOf("("));

                inventory.Add(name);

                if (item.GetComponent<Utility.Range_type>() != null) {
                    ammo_amounts.Add(name, item.GetComponent<Utility.Range_type>().ammo_held);
                }
            } else if (inventory.Contains(item.name)) {
                if (item.GetComponent<Utility.Range_type>() != null) {
                    ammo_amounts[name] += item.GetComponent<Utility.Range_type>().ammo_held;
                    if (item.name == equipped) {
                        current_ammo_amount = ammo_amounts[name];
                    }
                }
            }
            Destroy(item);
        }
    }

    private void set_ammo_type(string weapon_name) {
        switch (weapon_name) {
            case "Raygun":
                current_ammo_type = "Rayblast";
                current_ammo_amount = ammo_amounts[weapon_name];
                break;
            case "Shockgun":
                current_ammo_type = "Shockblast";
                current_ammo_amount = ammo_amounts[weapon_name];
                break;
            default:
                break;
        }
    }

    private void select_item_on_menu() {
        GameObject selected_ui_component = menu_objects[current_inventory_index]; // has to be in here
                                                                   // if not, very odd
        RectTransform rt = selected_ui_component.GetComponent<RectTransform>();
        RectTransform rt_highlight;

        if (highlighter == null) {
            highlighter = new GameObject();
            highlighter.transform.SetParent(HUD.transform, false);
            highlighter.AddComponent<RectTransform>();
            rt_highlight = highlighter.GetComponent<RectTransform>();
            rt_highlight.anchorMax = new Vector2(.5f, .5f);
            rt_highlight.anchorMin = new Vector2(.5f, .5f);
            rt_highlight.sizeDelta = new Vector2(rt_highlight.sizeDelta.x * .6f, rt_highlight.sizeDelta.y * .6f);
            highlighter.AddComponent<Image>();
            highlighter.GetComponent<Image>().sprite = selection;
            highlighter.transform.localScale = Vector3.one;
        }

        rt_highlight = highlighter.GetComponent<RectTransform>();
        rt_highlight.position = new Vector3(rt.position.x, rt.position.y, start_location_z);
        selected_ui_component.transform.SetParent(highlighter.transform);
    }

    private void deselect_item_on_menu() {
        menu_objects[current_inventory_index].transform.SetParent(HUD.transform);
    }

    private void set_item_menu_hud(string name, int current_index) {
        // create the object 
        // add rect transform and set the position
        // add image and set the sprite
        // set transform parent to UI
        // add it to menu_objects for later destruction

        GameObject ui_component = new GameObject();
        ui_component.transform.SetParent(HUD.transform, false);
        ui_component.AddComponent<RectTransform>();
        RectTransform rt = ui_component.GetComponent<RectTransform>();
        rt.position = new Vector3(start_location_x + (current_index + 1) * offset_x, start_location_y, start_location_z);
        rt.anchorMax = new Vector2(.5f, .5f);
        rt.anchorMin = new Vector2(.5f, .5f);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x * .5f, rt.sizeDelta.y * .5f);
        ui_component.AddComponent<Image>();
        ui_component.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(Resources.Load("Prefabs/" + name)),
            new Rect(0, 0, 128, 128), new Vector2()); // this needs to be configured
                                                      // size of textures can differ from sprite to sprite
        menu_objects.Add(ui_component);
    }

    private void display_items() {
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

        start_location_x = equipped_icon.position.x;
        start_location_y = equipped_icon.position.y;
        start_location_z = HUD.GetComponent<RectTransform>().transform.position.z;

        int num_items = 0;
        foreach (string i in inventory) {
            set_item_menu_hud(i, num_items);
            ++num_items;
        }
        for (; num_items < max_items; ++num_items) {
            set_item_menu_hud(none.name, num_items);
        }

        if (equipped == "") {
            current_inventory_index = 0;
        } else {
            current_inventory_index = equipped_index;
        }
    }

    private void toggle_item_menu() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Time.timeScale = 0; // pause game upon entering
                                // menu

            display_items();
            select_item_on_menu();
            item_menu_on = true;
        } else if (Input.GetKeyUp(KeyCode.E)) {
            foreach (GameObject g in menu_objects) {
                Destroy(g);
            }
            menu_objects.Clear();
            Destroy(highlighter);
            item_menu_on = false;

            if (equipped == "") {
                equipped_index = current_inventory_index = -1; // for consistency
                                                               // we don't do anything
                                                               // with -1 case yet
            }

            Time.timeScale = 1; // start game upon exiting
                                // menu
        }
    }

    private void cycle_through_items() {
        if (!item_menu_on) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            deselect_item_on_menu();

            if (current_inventory_index + 1 == max_items) {
                current_inventory_index = 0;
            } else {
                ++current_inventory_index;
            }

            select_item_on_menu();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            deselect_item_on_menu();

            if (current_inventory_index - 1 < 0) {
                current_inventory_index = max_items - 1;
            } else {
                --current_inventory_index;
            }

            select_item_on_menu();
        }
    }

    private void equip_item() {
        if (!item_menu_on) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (inventory.Count != 0 && current_inventory_index < inventory.Count) {
                equipped = inventory[current_inventory_index];
                equipped_index = current_inventory_index;
            }

            set_ammo_type(equipped);
        }
    }

    private void unequip_item() {
        if (!item_menu_on) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            equipped = "";
        }
    }

    private void drop_item() {
        if (!item_menu_on) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            if (current_inventory_index < inventory.Count) {
                string dropped = inventory[current_inventory_index];

                if (equipped_index == current_inventory_index) {
                    equipped = "";
                    equipped_index = -1; // consistency
                }

                inventory.RemoveAt(current_inventory_index);

                // need to refind the equipped item
                // if it has moved
                if (equipped != "") {
                    for (int i = 0; i < inventory.Count; ++i) {
                        if (inventory[i] == equipped) {
                            equipped_index = i;
                            break;
                        }
                    }
                }
         
                foreach (GameObject g in menu_objects) {
                    Destroy(g);
                }
                menu_objects.Clear();
                display_items();

                // instantiate the object under the player
                GameObject item = (GameObject)Instantiate(Resources.Load("Prefabs/" + dropped));
                item.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                if (item.GetComponent<Utility.Range_type>() != null) {
                    item.GetComponent<Utility.Range_type>().ammo_held = ammo_amounts[dropped];
                    ammo_amounts.Remove(dropped);
                }
            }
        }
    }

    private void update_ammo_remaining() {
        ammo_remaining.gameObject.GetComponent<Text>().text = "Ammo Amount: " + current_ammo_amount.ToString();

        if (current_ammo_amount > 5) {
            ammo_remaining.gameObject.GetComponent<Text>().color = new Color(0, 255, 255);
        } else {
            ammo_remaining.gameObject.GetComponent<Text>().color = new Color(255, 0, 0);
        }

        ammo_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(Resources.Load("Prefabs/" + current_ammo_type)),
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
                break;
            case "Shockgun":
                equipped_status.gameObject.GetComponent<Text>().text = "Shockgun equipped";
                equipped_status.gameObject.GetComponent<Text>().color = new Color(0, 255, 255);
                break;
            default:
                break;
        }

        if (current_ammo_type != "") {
            update_ammo_remaining();
        } else {
            set_ammo_none();
        }

        equipped_icon.gameObject.GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(Resources.Load("Prefabs/" + equipped)),
            new Rect(0, 0, 128, 128), new Vector2());
    }
}
