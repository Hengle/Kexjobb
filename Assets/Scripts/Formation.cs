using UnityEngine;
using System.Collections;
using UnitySteer.Behaviors;

public class Formation : MonoBehaviour {
	public int nrOfAgents;
	public GameObject leader;

	float speedScaling = 0.5f;
	Vector3[] templatePositions;
//	GameObject[] otherAgents; 
	
	// Use this for initialization
	void Start () {
		templatePositions = new Vector3[nrOfAgents];
		templatePositions[0] = new Vector3(0f, 0f, 0f);
		templatePositions[1] = new Vector3(10f, 0f, 0f);
		templatePositions[2] = new Vector3(20f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 leadersTarget = leader.GetComponent<SteerForPoint>().TargetPoint;
		for (int i = 1; i < templatePositions.Length; i++)
		{
			//		Transform agentPosition = transform.GetChild(i).transform;
			Vector3 target = leadersTarget + templatePositions[i];
			Transform neighbor = transform.GetChild(i - 1).transform; 
			SteerForPoint pointScript = transform.GetChild(i).GetComponent<SteerForPoint>();
			AutonomousVehicle vehicleScript = transform.GetChild(i).GetComponent<AutonomousVehicle>();
			pointScript.TargetPoint = target;
			pointScript.enabled = true;
			float distanceToNeighbor = (neighbor.position - transform.GetChild(i).transform.position).magnitude;
			vehicleScript.MaxSpeed = distanceToNeighbor * speedScaling;

		}
	}
}
