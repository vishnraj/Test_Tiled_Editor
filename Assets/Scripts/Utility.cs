using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    abstract public class Item : MonoBehaviour {

        // Use this for initialization
        private void Start() {

        }

        // Update is called once per frame
        private void Update() {

        }

        abstract public void action();
    }

    abstract public class Range_type : Item {
        public int ammo_held = 10; // default starting amount
    }
}

