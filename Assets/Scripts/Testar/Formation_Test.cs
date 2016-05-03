using UnityEngine;
using UnitySteer.Behaviors;
using System.Collections;
using System;

public abstract class Formation_Test : MonoBehaviour
{

    [HideInInspector]
    public Vector3 leadersTarget;
	[HideInInspector]
	public int nrOfAgents;
	private GameObject leader;

    //Set in subclass
	protected float maxWidth;
	protected float maxHeight;
	protected Vector3[] templatePositions;

    void Start()
	{
        nrOfAgents = transform.childCount;
        templatePositions = new Vector3[nrOfAgents];
		
		CreateTemplate(); // Implemented in subclasses
		leader = transform.GetChild(0).gameObject;

        // Set things for the leader
        for(int i = 0; i < nrOfAgents; i++)
        {
            Agent_Test curAgent = transform.GetChild(i).GetComponent<Agent_Test>();
            curAgent.StartPos = transform.GetChild(i).position;
            curAgent.TargetPos = transform.GetChild(i).position + new Vector3(0f, 0f, 50f);
            curAgent.TargetPosRVO = new RVO.Vector2(curAgent.TargetPos.x, curAgent.TargetPos.z);
        }
    }

    // Update is called once per frame
    void Update()
	{
        // Draw line to leaders target position
        Debug.DrawLine(transform.GetChild(0).position, transform.GetChild(0).GetComponent<Agent_Test>().TargetPos);
        
        // We don't need to update leaders pos (index 0)
		for (int i = 1; i < templatePositions.Length; i++)
		{
			Vector3 target = leader.transform.position + templatePositions[i];
			RVO.Vector2 targetRVO = new RVO.Vector2(target.x, target.z);

            Agent_Test curAgent = transform.GetChild(i).GetComponent<Agent_Test>();
            curAgent.TargetPos = target;
            curAgent.TargetPosRVO = targetRVO;

            // Draw line to followers target position
            Debug.DrawLine(transform.GetChild(i).position, curAgent.TargetPos, Color.blue);
		}
	}

	public Vector3[] TemplatePositions
	{
		get { return templatePositions; }
	}

	protected abstract void CreateTemplate();
}
