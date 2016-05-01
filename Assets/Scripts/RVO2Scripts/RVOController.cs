using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RVO;

public class RVOController : MonoBehaviour
{

	public GameObject agent;

	private RVO.Vector2[] goals;
	private int nrOfAgents = 0;
	private Simulator sim;
	private GameObject[] agents;
	private GameObject[] formationGroups;
	private Formation[] formations;

	void Start()
	{
		// Find number of agents
		formationGroups = new GameObject[transform.childCount];
		formations = new Formation[transform.childCount];
		for (int i = 0; i < formationGroups.Length; i++)
		{
			// Save each formation group
			formationGroups[i] = transform.GetChild(i).gameObject;
			nrOfAgents += formationGroups[i].GetComponent<Spawner>().nrOfAgents;

			formations[i] = formationGroups[i].GetComponent<Formation>();
			//	Debug.Log(nrOfAgents);
		}

		goals = new RVO.Vector2[nrOfAgents];
		sim = Simulator.Instance;
		agents = new GameObject[nrOfAgents];

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
			sim.doStep();
			UpdateAgentPos();
		}
	}

	// Do we need this?
	void InstantiateAgents()
	{
		for (int i = 0; i < nrOfAgents; i++)
		{
			RVO.Vector2 pos = sim.getAgentPosition(i);
			RVO.Vector2 velocity = sim.getAgentPrefVelocity(i);
			Vector3 lookDir = new Vector3(velocity.x(), 0f, velocity.y());

			agents[i] = Instantiate(agent, new Vector3(pos.x(), 0f, pos.y()),
					Quaternion.LookRotation(lookDir)) as GameObject;
		}
	}

	//TODO: Lerp agent rotation
	void UpdateAgentPos()
	{
		for (int i = 0; i < nrOfAgents; i++)
		{
			RVO.Vector2 pos = sim.getAgentPosition(i);
			RVO.Vector2 velocity = sim.getAgentVelocity(i);
			Vector3 temp = new Vector3(velocity.x(), 0f, velocity.y());

			Vector3 lookDir = agents[i].transform.forward;
			if (temp != new Vector3(0f, 0f, 0f))
				lookDir = temp;

			agents[i].transform.position = new Vector3(pos.x(), 0f, pos.y());
			agents[i].transform.rotation = Quaternion.LookRotation(lookDir);


			//	CheckDistance(agents[i].transform.position);
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
		Simulator.Instance.setTimeStep(0.25f);

		/*
 * Specify the default parameters for agents that are subsequently
 * added.
 */
		Simulator.Instance.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 2.5f, 2.0f, new RVO.Vector2(0.0f, 0.0f));

		/*
 * Add agents, specifying their start position, and store their
 * goals on the opposite side of the environment.
 */
		int agentCounter = 0;
		for (int i = 0; i < formationGroups.Length; ++i)
		{
			GameObject currentGroup = formationGroups[i];
			Vector3[] startPostions = currentGroup.GetComponent<Spawner>().StartPositions;
			for (int j = 0; j < currentGroup.transform.childCount; j++)
			{
				RVO.Vector2 pos = new RVO.Vector2(startPostions[j].x, startPostions[j].z);
				sim.addAgent(pos);
				agents[agentCounter++] = currentGroup.transform.GetChild(j).gameObject;
			}
		}
		for (int i = 0; i < agents.Length; i++)
		{

		}
	}
	void SetPreferredVelocities()
	{
		/*
 * Set the preferred velocity to be a vector of unit magnitude
 * (speed) in the direction of the goal.
 */
		int currentGroupIndex = 0;
		for (int i = 0; i < formations.Length; i++)
		{
			RVO.Vector2[] targetPositions = formationGroups[i].GetComponent<Formation>().TagetPositionsRVO;
			for (int j = 0; j < targetPositions.Length; j++)
				goals[currentGroupIndex++] = targetPositions[j];
		}
		for (int i = 0; i < Simulator.Instance.getNumAgents(); i++)
		{
			RVO.Vector2 goalVector = goals[i] - Simulator.Instance.getAgentPosition(i);

			if (RVOMath.absSq(goalVector) > 1.0f)
			{
				goalVector = RVOMath.normalize(goalVector);
			}

			Simulator.Instance.setAgentPrefVelocity(i, goalVector);
		}
	}

	bool ReachedGoal()
	{
		/* Check if all agents have reached their goals. */
		for (int i = 0; i < goals.Length; i++)
		{
			if (RVOMath.absSq(sim.getAgentPosition(i) - goals[i]) > sim.getAgentRadius(i))
			{
				return false;
			}
		}

		return true;
	}

}
