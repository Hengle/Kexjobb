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
	private bool test = true;
	private float oldLeaderRotation;
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
		oldLeaderRotation = 0;

	}

	void Update()
	{

		if (Math.Abs(oldLeaderRotation - leader.transform.rotation.eulerAngles.y) > .1f)
		{
			UpdateTemplatePositions();
		}
//		test = false;
		UpdateTargetPositions();
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for(int i = 0; i < templatePositions.Length; i++)
		{
			Gizmos.DrawCube(templatePositions[i], new Vector3(5f, 5f, 5f));
		}
	}
	/// <summary>
	/// Rotates template positions to match leaders direction
	/// </summary>
	void UpdateTemplatePositions()
	{
		//Get leaders current rotation
		float rotation = leader.transform.rotation.eulerAngles.y;
		float moveRotation = rotation - oldLeaderRotation;
		oldLeaderRotation = rotation;

		for (int i = 1; i < templatePositions.Length; i++)
		{
			//Rotate each template position around leader position
			Vector3 dir = templatePositions[i] - templatePositions[0];
			Quaternion temp = Quaternion.Euler(new Vector3(0f, moveRotation, 0f));
			dir = temp * dir;
			templatePositions[i] = dir + templatePositions[0];
			
		}
	}

	/// <summary>
	/// Updates target position for all followers
	/// </summary>
	void UpdateTargetPositions()
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
	/// <summary>
	/// Implement this in the subclass to define the relative positions of the agents
	/// </summary>
	protected abstract void CreateTemplate();
}