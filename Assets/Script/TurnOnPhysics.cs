using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnPhysics : MonoBehaviour 
{
	float timer = 0;
	bool switchPhysicsOn = false;
	[SerializeField] int turnOnAfterDuration = 38;
	void Update() {

		if (Time.timeScale == 1)
		{
			timer += Time.deltaTime;	
		}

		if (timer > turnOnAfterDuration && !switchPhysicsOn)
		{
			switchPhysicsOn = true;

			for (int i = 0; i < this.transform.childCount; i++)
			{
				this.transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
			}
		}
	}
}
