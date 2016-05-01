using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GroupSpawner : MonoBehaviour {

	public float width;
	public float height;
	public float spawnTime = 3;

	private List<GameObject> groups;
	private System.Random random;
	// Use this for initialization
	void Start () {
		random = new System.Random();
		InvokeRepeating("Spawn", spawnTime, spawnTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//TODO: Implement this 
	void Spawn()
	{
		//Randomize start position for group
		float minX = transform.position.x - width / 2;
		float maxX = transform.position.x + width / 2;
		float minZ = transform.position.z - height / 2;
		float maxZ = transform.position.z + height / 2;
		Vector3 startPosition = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(minZ, maxZ));

		//Randomize nr of agents in group (2 or 3)
		int nrOfAgents = UnityEngine.Random.Range(2, 3);

		//Randomize group formation
		Array values = Enum.GetValues(typeof(FormationState));
		FormationState formation = (FormationState)values.GetValue(random.Next(values.Length));

		//Instantiate the new agents
		InstantiateGroup(nrOfAgents, startPosition, formation);

		
		//Update the RVOController with the new agents

	}
	void InstantiateGroup(int nrOfAgents, Vector3 startPosition, FormationState formation)
	{
		//Spawn leader at startPosition

		//Spawn rest of group at their respective positions
	}
}
