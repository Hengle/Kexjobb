using UnityEngine;
using UnitySteer.Behaviors;
using System.Collections;
using System;

public abstract class Formation : MonoBehaviour
{
	[HideInInspector] public Vector3 leadersTarget;
	[HideInInspector]
	public int nrOfAgents;
//	public int nrOfAgents;
	private GameObject leader;
	private Vector3[] targetPositions;
	private RVO.Vector2[] targetPositionsRVO;
	private Transform[] agentTransforms;

	protected float arrivalRadius = 5;
	protected float speedScaling = 0.5f;
	//Set in subclass
	protected float maxWidth;
	protected float maxHeight;
	protected Vector3[] templatePositions;
	//	GameObject[] otherAgents; 

	// Use this for initialization
	void Awake()
	{
		nrOfAgents = transform.childCount;
		templatePositions = new Vector3[transform.childCount];
		targetPositions = new Vector3[transform.childCount];
		targetPositionsRVO = new RVO.Vector2[transform.childCount];
		agentTransforms = new Transform[transform.childCount];
		CreateTemplate();
		leader = transform.GetChild(0).gameObject;
		//Target positions for leader, needs only to be set once
		targetPositions[0] = leadersTarget;
		targetPositionsRVO[0] = new RVO.Vector2(leadersTarget.x, leadersTarget.z);

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
			RVO.Vector2 targetRVO = new RVO.Vector2(target.x, target.z);
			targetPositionsRVO[i] = targetRVO;
			//			Agent agentScript = transform.GetChild(i).GetComponent<Agent>();
			//agentScript.TargetPoint = target;
			//agentScript.enabled = true;
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
	public RVO.Vector2[] TargetPositionsRVO
	{
		get { return targetPositionsRVO; }
	}
	protected abstract void CreateTemplate();
}
