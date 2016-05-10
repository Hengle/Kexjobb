using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RVO;

public class RVOController_Test : MonoBehaviour
{
	public float agentRootSpeed;

	private List<RVO.Vector2> goals;
	[SerializeField]
	private int totNrOfAgents = 0;
	private Simulator sim;
	private List<GameObject> agents;
	private List<GameObject> formationGroups;
	private List<Formation_Test> formations;


	void Start()
	{
		agentRootSpeed = 2f;
		goals = new List<RVO.Vector2>();
		sim = Simulator.Instance;
		agents = new List<GameObject>();
		formationGroups = new List<GameObject>();
		formations = new List<Formation_Test>();

		// Old setupScenario code
		sim.setTimeStep(0.05f);
		sim.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 2.5f, 2.0f, new RVO.Vector2(0.0f, 0.0f));
	}

	// Update is called once per frame
	void Update()
	{
			SetPreferredVelocities();
			sim.doStep();
			UpdateAgentPos();
	}

	public void AddGroupToSim(GameObject group)
	{
		Agent_Test leader = group.transform.GetChild(0).gameObject.GetComponent<Agent_Test>();
		int groupSize = group.transform.childCount;

		formationGroups.Add(group); // Add group to formationGroups
		totNrOfAgents += groupSize; // Increase total number of agents
		formations.Add(group.GetComponent<Formation_Test>()); // Add the formation

		// Add agents to list
		for (int i = 0; i < groupSize; i++)
			agents.Add(group.transform.GetChild(i).gameObject);

		// This has to be done when we spawn in a new agent while the simulation is running!!
		sim.Clear(); // Default Agents has to be set after this!
		sim.setTimeStep(0.05f);
		sim.setAgentDefaults(15.0f, 10, 10.0f, 10.0f, 2.5f, 2.0f, new RVO.Vector2(0.0f, 0.0f));

		// Add the individual agents to RVO Simulator
		for (int i = 0; i < agents.Count; i++)
		{
			Agent_Test curAgent = agents[i].GetComponent<Agent_Test>();
			sim.addAgent(curAgent.CurrentPosRVO);
		}

		//Debug.Log(sim.getNumAgents());
	}

	/* Update agent positions and direction */
	void UpdateAgentPos()
	{

		for (int i = 0; i < totNrOfAgents; i++)
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

			var newDir = Quaternion.LookRotation(lookDir).eulerAngles;
			agents[i].transform.rotation = Quaternion.Slerp(agents[i].transform.rotation,
					Quaternion.Euler(newDir), Time.deltaTime * agentRootSpeed);
			agents[i].transform.position = new Vector3(pos.x(), 0f, pos.y());

		}
	}

	void SetPreferredVelocities()
	{
		/* set the preferred velocity to be a vector of unit magnitude
		 * (speed) in the direction of the goal. */
		for (int i = 0; i < agents.Count; i++)
		{
			Agent_Test curAgent = agents[i].GetComponent<Agent_Test>();
			RVO.Vector2 goalVector = curAgent.TargetPosRVO - sim.getAgentPosition(i);
			sim.setAgentPrefVelocity(i, goalVector);
		}
	}

	/* Check if all agents have reached their goals. */
	bool ReachedGoal()
	{
		for (int i = 0; i < goals.Count; i++)
		{
			if (RVOMath.absSq(sim.getAgentPosition(i) - goals[i]) > sim.getAgentRadius(i) * sim.getAgentRadius(i))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Checks if the leader has reached its goal
	/// </summary>
	/// <returns></returns>
	bool LeaderReachedGoal(GameObject group)
	{
		GameObject leader = group.transform.GetChild(0).gameObject;
		return (Math.Pow(((leader.GetComponent<Agent_Test>().TargetPos - leader.transform.position).magnitude), 2) < Math.Pow(sim.getAgentRadius(1), 2)) ? true : false;
	}
}
