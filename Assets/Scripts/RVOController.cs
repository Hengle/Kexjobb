using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using RVO;

public class RVOController : MonoBehaviour
{

	IList<RVO.Vector2> goals;

	public GameObject agent;
	public int nrOfAgents;
	public float distanceToMid;
	public float circleRadius;		//radius 150 and 100 agents looks cool

	private Simulator sim;
	private GameObject[] agents;
	

	// Use this for initialization
	void Start()
	{
		goals = new List<RVO.Vector2>();
		sim = Simulator.Instance;
		agents = new GameObject[nrOfAgents];
		distanceToMid = 200f;
		SetupScenario();
		SetPreferredVelocities();
		InstantiateAgents();
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
		distanceToMid = 0;
		for(int i = 0; i < nrOfAgents; i++)
		{
			RVO.Vector2 pos = sim.getAgentPosition(i);
			RVO.Vector2 velocity = sim.getAgentVelocity(i);
            Vector3 temp = new Vector3(velocity.x(), 0f, velocity.y());

            Vector3 lookDir = agents[i].transform.forward;
            if (temp != new Vector3(0f,0f,0f))
                lookDir = temp;
            
			agents[i].transform.position = new Vector3(pos.x(), 0f, pos.y());
            agents[i].transform.rotation = Quaternion.LookRotation(lookDir);


            CheckDistance(agents[i].transform.position);
		}
	}
	void CheckDistance(Vector3 pos)
	{
		float dist = (new Vector3(0f, 0f, 0f) - pos).magnitude;
		if (dist > distanceToMid) distanceToMid = dist;
	}
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
		for (int i = 0; i < nrOfAgents; ++i)
		{
			Simulator.Instance.addAgent(circleRadius *
					new RVO.Vector2((float)Math.Cos(i * 2.0f * Math.PI / (float)nrOfAgents),
							(float)Math.Sin(i * 2.0f * Math.PI / (float)nrOfAgents)));
			goals.Add(-Simulator.Instance.getAgentPosition(i));
		}
	}
	void SetPreferredVelocities()
	{
		/*
		 * Set the preferred velocity to be a vector of unit magnitude
		 * (speed) in the direction of the goal.
		 */
		for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
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
		for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
		{
			if (RVOMath.absSq(Simulator.Instance.getAgentPosition(i) - goals[i]) > Simulator.Instance.getAgentRadius(i) * Simulator.Instance.getAgentRadius(i))
			{
				return false;
			}
		}

		return true;
	}

}
