using UnityEngine;
using System.Collections;
using System;
public class ChangeFormationTimer : MonoBehaviour {

	public float timer = 5;

	private float cooldown;
	private Formation[] formations;
	private System.Random rnd;
	private int currentFormationIndex;
	// Use this for initialization
	void Start () {
		cooldown = timer;
		formations = GetComponents<Formation>();
		rnd = new System.Random();
		for(int i = 0; i < formations.Length; i++)
		{
			if (formations[i].enabled == true)
				currentFormationIndex = i;
		}
	}
	
	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;
		if(cooldown <= 0)
		{

			int newFormationIndex = currentFormationIndex;
			while(newFormationIndex == currentFormationIndex)
				newFormationIndex = rnd.Next(0, formations.Length);
			
			Formation currentFormationScript = formations[currentFormationIndex];
			Formation newFormation = formations[newFormationIndex];
			newFormation.enabled = true;
			currentFormationScript.enabled = false;
			currentFormationIndex = newFormationIndex;
			cooldown = timer;
		}
	}
}
