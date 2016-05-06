using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVO;

public class RVOController : MonoBehaviour
{
	public GameObject agent;
	public float agentRootSpeed;

	//private RVO.Vector2[] goals;
	private List<RVO.Vector2> goals;
	private int nrOfAgents = 0;
	private Simulator sim;
	//	private GameObject[] agents;
	//	private GameObject[] formationGroups;
	//	private Formation[] formations;
	private List<GameObject> agents;
	private List<GameObject> formationGroups;
	private List<Formation> formations;


	void Start()
	{
		agentRootSpeed = 2f;
		// Find number of agents
		//		formationGroups = new GameObject[transform.childCount];
		//		formations = new Formation[transform.childCount];
		formationGroups = new List<GameObject>(transform.childCount);
		formations = new List<Formation>(transform.childCount);
		for (int i = 0; i < formationGroups.Capacity; i++)
		{
			// Save each formation group
			formationGroups.Add(transform.GetChild(i).gameObject);
			nrOfAgents += formationGroups[i].GetComponent<Spawner>().nrOfAgents;

			formations.Add(formationGroups[i].GetComponent<Formation>());
			//	Debug.Log(nrOfAgents);
		}

		//		goals = new RVO.Vector2[nrOfAgents];
		goals = new List<RVO.Vector2>(nrOfAgents);
		for (int i = 0; i < goals.Capacity; i++)
		{
			goals.Add(new RVO.Vector2());
		}
		sim = Simulator.Instance;
		//		agents = new GameObject[nrOfAgents];
		agents = new List<GameObject>(nrOfAgents);

		SetupScenario(); // Add the agents to the simulator
										 //SetPreferredVelocities();
										 //InstantiateAgents();
	}

	// Update is called once per frame
	void Update()
	{
		if (!ReachedGoal())
		{
			SetPreferredVelocities();
			//sim.setTimeStep(Time.deltaTime);
			sim.doStep();
			UpdateAgentPos();
		}
	}

	public void UpdateController()
	{
		GameObject newGroup = GetComponent<GroupSpawner>().Groups.Last();
		formationGroups.Add(newGroup);
		nrOfAgents += formationGroups[transform.childCount - 1].GetComponent<Formation>().nrOfAgents;
		formations.Add(formationGroups[transform.childCount - 1].GetComponent<Formation>());
		for (int i = 0; i < newGroup.transform.childCount; i++)   //newGroup.transform.childCount is the number of agents added
		{
			goals.Add(formations.Last().TargetPositionsRVO[i]);
		}
		for (int i = 0; i < newGroup.transform.childCount; i++)
		{
			RVO.Vector2 pos = new RVO.Vector2(newGroup.transform.GetChild(i).position.x, newGroup.transform.GetChild(i).position.z);
			sim.addAgent(pos);
			agents.Add(newGroup.transform.GetChild(i).gameObject);
		}

	}
	//TODO: Lerp agent rotation
	void UpdateAgentPos()
	{
		for (int i = 0; i < nrOfAgents; i++)
		{
			// Get agent position and current velocity of RVO agent
			RVO.Vector2 pos = sim.getAgentPosition(i);
			RVO.Vector2 velocity = sim.getAgentVelocity(i);
			// Create temporary velocity vector
			Vector3 temp = new Vector3(velocity.x(), 0f, velocity.y());
			// Get current look direction of GameObject agent
			Vector3 lookDir = agents[i].transform.forward;
			// If current RVO agent has no velocity
			if (temp != new Vector3(0f, 0f, 0f))
				lookDir = temp;

			Quaternion prefabRot = agents[i].transform.rotation;
			var newDir = Quaternion.LookRotation(lookDir).eulerAngles;
			agents[i].transform.rotation = Quaternion.Slerp(agents[i].transform.rotation,
					Quaternion.Euler(newDir), Time.deltaTime * agentRootSpeed);
			// CheckDistance(agents[i].transform.position);
			agents[i].transform.position = new Vector3(pos.x(), 0f, pos.y());
		}
	}
	/*
void CheckDistance(Vector3 pos)
{
	float dist = (new Vector3(0f, 0f, 0f) - pos).magnitude;
	if (dist > distanceToMid) distanceToMid = dist;
}
*/

	// Add all the agents to the simulator
	void SetupScenario()
	{
		//TODO: Change time step to something Unity related!
		/* Specify the global time step of the simulation. */
		sim.setTimeStep(Time.deltaTime);

		/*
 * Specify the default parameters for agents that are subsequently
 * added.
 */
		sim.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 2.5f, 2.0f, new RVO.Vector2(0.0f, 0.0f));

		/*
 * Add agents, specifying their start position, and store their
 * goals on the opposite side of the environment.
 */
		//int agentCounter = 0;
		for (int i = 0; i < formationGroups.Capacity; ++i)
		{
			GameObject currentGroup = formationGroups[i];
			//			Vector3[] startPostions = currentGroup.GetComponent<Spawner>().StartPositions;

			for (int j = 0; j < currentGroup.transform.childCount; j++)
			{
				//				RVO.Vector2 pos = new RVO.Vector2(startPostions[j].x, startPostions[j].z);
				RVO.Vector2 pos = new RVO.Vector2(currentGroup.transform.position.x, currentGroup.transform.position.z);
				sim.addAgent(pos);
				//				agents[agentCounter++] = currentGroup.transform.GetChild(j).gameObject;
				agents.Add(currentGroup.transform.GetChild(j).gameObject);
			}
		}
		/*
		for (int i = 0; i < agents.Capacity; i++)
		{

		}
		*/
	}
	void SetPreferredVelocities()
	{
		/*
 * Set the preferred velocity to be a vector of unit magnitude
 * (speed) in the direction of the goal.
 */
		int currentGroupIndex = 0;
		for (int i = 0; i < formationGroups.Count; i++)
		{
			RVO.Vector2[] targetPositions = formationGroups[i].GetComponent<Formation>().TargetPositionsRVO;
			for (int j = 0; j < targetPositions.Length; j++)
			{
				RVO.Vector2 newGoal = new RVO.Vector2(targetPositions[j].x(), targetPositions[j].y());
				goals[currentGroupIndex++] = newGoal;

			}
		}
		for (int i = 0; i < sim.getNumAgents(); i++)
		{
			RVO.Vector2 goalVector = goals[i] - sim.getAgentPosition(i);

			if (RVOMath.absSq(goalVector) > 1.0f)
			{
				goalVector = RVOMath.normalize(goalVector);
			}

			sim.setAgentPrefVelocity(i, goalVector);
		}
	}

	bool ReachedGoal()
	{
		/* Check if all agents have reached their goals. */
		for (int i = 0; i < goals.Count; i++)
		{
			if (RVOMath.absSq(sim.getAgentPosition(i) - goals[i]) > sim.getAgentRadius(i) * sim.getAgentRadius(i))
			{
				return false;
			}
		}

		return true;
	}
}
