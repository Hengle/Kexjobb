using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GroupSpawnerAreas : MonoBehaviour
{
	public GameObject spawnObject;
	public float spawnTime = 3; // Delay between spawns
	public GameObject group2;
	public GameObject group3;

	private List<GameObject> groups; // List of groups that have spawned
	private System.Random random; // For easier access to random
	private RVOController_Test rvo; // For access to RVOController Script
	private Vector3 randomStart; // For random star position
	private SpawnAreasScript spawnerAreasScript;
	// Reference RVOController script
	void Awake()
	{
		rvo = GetComponent<RVOController_Test>();
		spawnerAreasScript = spawnObject.GetComponent<SpawnAreasScript>();
	}

	void Start()
	{
		groups = new List<GameObject>();
		random = new System.Random();
		InvokeRepeating("Spawn", 1, spawnTime);
	}


	void Spawn()
	{
		//Randomize start position for group

		//		Vector3 startPosition = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(minZ, maxZ));
		Vector3[] pos = spawnerAreasScript.GenerateRandomStartPosAndGoal();
		Vector3 startPosition = pos[0];
		randomStart = startPosition; // Use this in formation
		Vector3 goalPosition = pos[1];
		//Randomize nr of agents in group (2 or 3)
		int nrOfAgents = UnityEngine.Random.Range(2, 4);

		//Randomize group formation
		Array values = Enum.GetValues(typeof(FormationState));
		FormationState formation = (FormationState)values.GetValue(random.Next(values.Length));

		//Instantiate the new agents
		InstantiateGroup(nrOfAgents, startPosition, goalPosition, formation);

	}

	// Spawns a group ini the scene
	void InstantiateGroup(int nrOfAgents, Vector3 startPosition, Vector3 leadersGoal,
		FormationState formation)
	{
		//Spawn group at startPosition
		if (nrOfAgents == 2)
		{
			GameObject group = Instantiate(group2, startPosition, Quaternion.identity) as GameObject;
			group.transform.parent = transform;
			groups.Add(group);

		}
		else if (nrOfAgents == 3)
		{
			GameObject group = Instantiate(group3, startPosition, Quaternion.identity) as GameObject;
			group.transform.parent = transform;
			groups.Add(group);
		}

		// Add the right formation script to the group
		switch (formation)
		{
			case FormationState.HorizontalRow:
				groups.Last().AddComponent<HorizontalRowFormation_Test>().enabled = true;
				break;
			case FormationState.Triangle:
				groups.Last().AddComponent<TriangleFormation_Test>().enabled = true;
				break;
			case FormationState.VerticalRow:
				groups.Last().AddComponent<VerticalRowFormation_Test>().enabled = true;
				break;
		}
		//Set target pos for group leader
		GameObject leader = groups.Last().transform.GetChild(0).gameObject;
		Agent_Test agentScript = leader.GetComponent<Agent_Test>();
		agentScript.TargetPos = leadersGoal;
		agentScript.TargetPosRVO = new RVO.Vector2(leadersGoal.x, leadersGoal.z);
		agentScript.IsLeader = true; 

		//Update the RVOController with the new agents
		rvo.AddGroupToSim(groups.Last());
	}

	public List<GameObject> Groups
	{
		get { return groups; }
	}
	public Vector3 RandomStartPos
	{
		get { return randomStart; }
	}
}
