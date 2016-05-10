using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public int nrOfAgents;
	public GameObject agent;
	[HideInInspector] public GameObject[] agents;
	public Vector3 leadersTarget;
	public bool startRVO;

//	private FormationState spawnFormation;
	private float distanceBetweenAgents = 10f;
	private Vector3[] startPositions;
	private RVOController rvo;

	void Awake()
	{
		rvo = GetComponentInParent<RVOController>();
	}

	void Start()
	{
		agents = new GameObject[nrOfAgents];
		startPositions = new Vector3[nrOfAgents];
		CreateStartPositions();
		InstantiateAgents();
		GetComponent<Formation>().UpdateLeadersTarget(leadersTarget); //Set the target for the leader
	}

	// Generate starting positions for each agent
	void CreateStartPositions()
	{
		float xPos = transform.position.x;
		for (int i = 0; i < nrOfAgents; i++)
		{
			if (i % 2 != 0)
			{
				xPos += distanceBetweenAgents;
			}
			Vector3 spawnPos = new Vector3(xPos, 0f, transform.position.z);
			startPositions[i] = spawnPos;
			xPos *= -1;
		}
	}

	// Instantiate the agents into the scene and save them in "agents"
	void InstantiateAgents()
	{
		for(int i = 0; i < nrOfAgents; i++)
		{
			agents[i] = Instantiate(agent, startPositions[i], Quaternion.identity) as GameObject;
			agents[i].transform.parent = transform;
		}
	}
	public Vector3[] StartPositions
	{
		get { return startPositions; }
	}
}


