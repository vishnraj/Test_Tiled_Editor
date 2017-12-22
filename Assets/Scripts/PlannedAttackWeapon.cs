using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlannedAttackWeapon : MonoBehaviour {
    // This is an abstract class, however, it will
    // provide a function that is called every frame
    // this is the function that will check for mouse
    // input and add up to N data points that the player
    // selects to a queue of attack vectors

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public abstract void apply_attack();
}
