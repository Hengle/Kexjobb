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
	[SerializeField]private int nrOfAgents = 0;
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
        bool hold = false;
        if (Input.GetMouseButton(0))
            hold = true;
        else
            hold = false;

        if (hold)
        {
            SetPreferredVelocities();
            sim.doStep();
            UpdateAgentPos();
        }
    }

    public void AddGroupToSim(GameObject group)
    {
        int groupSize = group.transform.childCount;

        formationGroups.Add(group); // Add group to formationGroups
        nrOfAgents += groupSize; // Increase total number of agents
        formations.Add(group.GetComponent<Formation_Test>()); // Add the formation

        // Add the individual agents to RVO Simulator
        for (int i = 0; i < groupSize; i++)
        {
            Agent_Test curAgent = group.transform.GetChild(i).transform.GetComponent<Agent_Test>();
            sim.addAgent(curAgent.CurrentPosRVO);
            agents.Add(group.transform.GetChild(i).gameObject);
            goals.Add(curAgent.TargetPosRVO); // Do we need this?
        }
    }

    /* Update agent positions and direction */
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
        int currentAgentIndex = 0;
        // go through all agent groups
        for (int i = 0; i < formationGroups.Count; i++)
        {
            GameObject currFormGroup = formationGroups[i];

            // Go through each agent in group
            for (int j = 0; j < currFormGroup.transform.childCount; j++)
            {
                Agent_Test currAgent = currFormGroup.transform.GetChild(j).GetComponent<Agent_Test>();

                // get direction vector for reaching goal
                RVO.Vector2 goalVector = currAgent.TargetPosRVO - sim.getAgentPosition(currentAgentIndex);

                // Förstod inte hur denna fungerade med index i == 2
                //// get direction vector for reaching goal
                //if (i == 2)
                //{
                //    Debug.Log(goalVector);
                //}
                //if (RVOMath.absSq(goalVector) > 1.0f)
                //{
                //    goalVector = RVOMath.normalize(goalVector);
                //}

                // set the goalvector to be prefered velocity
                sim.setAgentPrefVelocity(currentAgentIndex, goalVector);

                currentAgentIndex++;
            }
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
}
