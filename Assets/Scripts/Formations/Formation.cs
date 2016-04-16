using UnityEngine;
using UnitySteer.Behaviors;
using System.Collections;
using System;

public abstract class Formation : MonoBehaviour
{
//	public int nrOfAgents;
	private GameObject leader;

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
		CreateTemplate();
		leader = transform.GetChild(0).gameObject;

		float radius = leader.GetComponent<AutonomousVehicle>().Radius;
		float height = 2 * radius + Math.Abs(templatePositions[templatePositions.Length - 1].z);
		float width = 2 * radius + Math.Abs(templatePositions[templatePositions.Length - 1].x);

	}

	// Update is called once per frame
	void Update()
	{
		Vector3 leadersTarget = leader.GetComponent<SteerForPoint>().TargetPoint;
		for (int i = 1; i < templatePositions.Length; i++)
		{
			Transform agentTransform = transform.GetChild(i).transform;
			Vector3 target = leader.transform.position + templatePositions[i];
			Transform neighbor = transform.GetChild(i - 1).transform;
			SteerForPoint pointScript = transform.GetChild(i).GetComponent<SteerForPoint>();
			AutonomousVehicle vehicleScript = transform.GetChild(i).GetComponent<AutonomousVehicle>();
			pointScript.TargetPoint = target;
			pointScript.enabled = true;
			float distanceToTarget = (target - agentTransform.position).magnitude;
			vehicleScript.MaxSpeed = distanceToTarget * speedScaling + leader.GetComponent<AutonomousVehicle>().MaxSpeed;

			if ((agentTransform.position - target).magnitude < arrivalRadius)
			{
				float speed = vehicleScript.TurnTime;
				Vector3 targetDir = leader.transform.forward;
				float step = speed * Time.deltaTime;
				Vector3 newDir = Vector3.RotateTowards(agentTransform.forward, targetDir, step, 0.0f);
				agentTransform.rotation = Quaternion.LookRotation(newDir);
			}
		}
	}
	public Vector3[] TemplatePositions
	{
		get { return templatePositions; }
	}
	protected abstract void CreateTemplate();
}
