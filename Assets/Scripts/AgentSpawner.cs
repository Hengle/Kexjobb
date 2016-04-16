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
		float xPos = 0;
		for (int i = 0; i < nrOfAgents; i++)
		{
			if (i % 2 != 0)
			{
				xPos += 10f;
			}
			startPositions[i] = new Vector3(xPos, 0f, 0f);
			xPos *= -1;
		}
	}
}
