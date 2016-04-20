using UnityEngine;
using System.Collections;

public class AgentSpawner : MonoBehaviour {

	public int nrOfAgents;
	public GameObject leader;
	public GameObject agent;

//	private Formation formation;
	private Vector3 [] startPositions;
	// Use this for initialization
	void Awake () {
		CalculateStartPositions();
		//Initiate leader
//		formation = GetComponent<Formation>();
//		GameObject childObject =  Instantiate(leader, formation.TemplatePositions[0], Quaternion.identity) as GameObject;
		GameObject childObject = Instantiate(leader, startPositions[0], Quaternion.identity) as GameObject;
		childObject.transform.parent = transform;		//Makes leader child of this object
		for (int i = 1; i < nrOfAgents; i++)
		{
			Spawn(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//Spawns one agent
	void Spawn(int agentNr)
	{
		//Randomize start position?
//		GameObject childObject = Instantiate(agent, formation.TemplatePositions[agentNr], Quaternion.identity) as GameObject;
		GameObject childObject = Instantiate(agent, startPositions[agentNr], Quaternion.identity) as GameObject;
		childObject.transform.parent = transform;   //Makes leader child of this object
	}
	void CalculateStartPositions()
	{
		startPositions = new Vector3[nrOfAgents];
		BoxCollider collider = GetComponent<BoxCollider>();
		//float midPointX = transform.position.x;
		//float midPointZ = transform.position.z;
		Bounds bounds = collider.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
/*
		Vector2 xBoundaries = new Vector2();
		Vector2 yBoundaries = new Vector2();
*/		
		for (int i = 0; i < nrOfAgents; i++)
		{
			startPositions[i] = new Vector3(Random.Range(min.x, max.x), 0.0f, Random.Range(min.z, max.z));
		}
	}
}
