using UnityEngine;
using UnitySteer.Behaviors;
using System.Collections;
using System;

public abstract class Formation : MonoBehaviour
{
//	public int nrOfAgents;
	private GameObject leader;
	private Vector3[] targetPositions;

	protected float arrivalRadius = 5;
	protected float speedScaling = 0.5f;
	//Set in subclass
	protected float maxWidth;
	protected float maxHeight;
	protected Vector3[] templatePositions;
	//	GameObject[] otherAgents; 

	// Use this for initialization
	void Start()
	{
		templatePositions = new Vector3[transform.childCount];
		targetPositions = new Vector3[transform.childCount];
		CreateTemplate();
		leader = transform.GetChild(0).gameObject;

		float radius = leader.GetComponent<Agent>().radius;
		float height = 2 * radius + Math.Abs(templatePositions[templatePositions.Length - 1].z);
		float width = 2 * radius + Math.Abs(templatePositions[templatePositions.Length - 1].x);

	}

	// Update is called once per frame
	void Update()
	{
		for (int i = 1; i < templatePositions.Length; i++)
		{
			Transform agentTransform = transform.GetChild(i).transform;
			Vector3 target = leader.transform.position + templatePositions[i];
			Agent agentScript = transform.GetChild(i).GetComponent<Agent>();
			agentScript.TargetPoint = target;
			agentScript.enabled = true;
		}
	}
	public Vector3[] TemplatePositions
	{
		get { return templatePositions; }
	}
	public Vector3[] TargetPostions
	{
		get { return targetPositions; }
	}
	protected abstract void CreateTemplate();
}
