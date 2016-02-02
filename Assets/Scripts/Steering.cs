using UnityEngine;
using System.Collections;

public class Steering : MonoBehaviour {

	public GameObject agent;
	public float slowingRadius = 10;
	public bool flee = false;
	private Transform target;
//	private GameObject target2;
	private Target targetScript;

	private Agent agentScript;
	private Vector3 currentVelocity;
	private Rigidbody agentRb;
	private SphereCollider agentCollider;

	void Awake()
	{
		agentRb = agent.GetComponent<Rigidbody>();
		agentScript = agent.GetComponent<Agent>();
		agentCollider = agent.GetComponent<SphereCollider>();
		target = GameObject.FindGameObjectWithTag("Target").transform;
//		target2 = GameObject.FindGameObjectWithTag("Target");
		targetScript = target.GetComponent<Target>();
	}
	void Start ()
	{
		currentVelocity = new Vector3(0f, 0f, 0f);

	}

	void Update ()
	{
		if(flee)
			Flee();
		else
			Seek();
		agentRb.MoveRotation(Quaternion.LookRotation(currentVelocity));
		//Pursuit();
	}
	//Moves towards target
	void Seek ()
	{
		Vector3 distanceVector = target.position - agent.transform.position;  //Distance left to target
		Vector3 dirToTarget = distanceVector.normalized;              //Direction to target as a vector
		Vector3 desiredVelocity = dirToTarget * agentScript.maxSpeed;   //The desired velocity pointing towards the target
		float distance = distanceVector.magnitude;
		if (distance < slowingRadius)
		{
			desiredVelocity *= (distance / slowingRadius);
		}
		Vector3 steeringForce = desiredVelocity - currentVelocity;    //The steering force is the vector which we want to change the current direction with				
		steeringForce = Truncate(steeringForce, agentScript.maxForce);
		Vector3 steeringAcc = steeringForce / agentRb.mass;   //Converts force to acceleration
		currentVelocity = Truncate(steeringAcc + currentVelocity, agentScript.maxSpeed);
//		agent.transform.right = currentVelocity.normalized;
		agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position
	}
	//Flees from target
	void Flee ()
	{
		Vector3 distanceVector = agent.transform.position - target.position;  //Distance left to target
		Vector3 dirToTarget = distanceVector.normalized;              //Direction to target as a vector
		Vector3 desiredVelocity = dirToTarget * agentScript.maxSpeed;   //The desired velocity pointing towards the target
		float distance = distanceVector.magnitude;
		if (distance < slowingRadius)
		{
			desiredVelocity *= (distance / slowingRadius);
		}
		Vector3 steeringForce = desiredVelocity - currentVelocity;    //The steering force is the vector which we want to change the current direction with				
		steeringForce = Truncate(steeringForce, agentScript.maxForce);
		Vector3 steeringAcc = steeringForce / agentRb.mass;   //Converts force to acceleration
		currentVelocity = Truncate(steeringAcc + currentVelocity, agentScript.maxSpeed);
		agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position

	}
	//Moves towards the target by predicting its future position (OBS Funkar inte!)
	void Pursuit()
	{
		Vector3 distanceVector = target.position - agent.transform.position;  //Distance left to target
		float distance = distanceVector.magnitude;
		float T = distance / agentScript.maxSpeed;		//Calculate the dynamic timestep
		Vector3 futurePosition = target.position + targetScript.velocity * T; // Estimates the future position of the target

		Vector3 dirToTarget = futurePosition.normalized;              //Direction to targets future position as a vector
		Vector3 desiredVelocity = dirToTarget * agentScript.maxSpeed;   //The desired velocity pointing towards the target
		
		//Start to slow down if target is nearby
		if (distance < slowingRadius)
		{
			desiredVelocity *= (distance / slowingRadius);
		}

		Vector3 steeringForce = desiredVelocity - currentVelocity;    //The steering force is the vector which we want to change the current direction with				
		steeringForce = Truncate(steeringForce, agentScript.maxForce);
		Vector3 steeringAcc = steeringForce / agentRb.mass;   //Converts force to acceleration
		currentVelocity = Truncate(steeringAcc + currentVelocity, agentScript.maxSpeed);
		agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position

	}

	void OnTriggerStay(Collider target)
	{
		SphereCollider targetSc = target.gameObject.GetComponent<SphereCollider>();
		//We only want to avoid the obstacle objects
		if(target.gameObject.tag.Equals("Obstacle"))
		{
	//		Debug.Log("Collition");
			//First we need to check if it is possible for a collision to occur
			Vector3 steering;
			float targetX = target.gameObject.transform.position.x;
			float agentX = agent.transform.position.x;
			float horizontalDistance = targetX - agentX; //Distance from agent to obstacle on z-axis
/*
			if (horizontalDistance < 0)		//Then the obstacle is on the right side of the agent, thus we want to steer it in the left direction
			{
				float steeringForce = (targetX - targetSc.radius) - (agentX + agentCollider.radius); 
				steering = new Vector3(0f, 0f, steeringForce);
			}
			else
			{
				float steeringForce = (targetX + targetSc.radius) - (agentX - agentCollider.radius);
				steering = new Vector3(0f, 0f, steeringForce);
			}
*/
			float steeringForce = (targetX - targetSc.radius) - (agentX + agentCollider.radius);
			steering = new Vector3(0f, 0f, steeringForce);

			steering = Truncate(steering, agentScript.maxForce);
//			Debug.Log(steering.z);
			Vector3 steeringAcc = (steering / agentRb.mass);   //Converts force to acceleration
//			steeringAcc.Normalize();
			Vector3 desiredVelocity = (currentVelocity + steeringAcc);
//			desiredVelocity.Normalize();
			float theta = Vector3.Angle(desiredVelocity, currentVelocity);
			agentRb.MoveRotation(Quaternion.Euler(0f, theta, 0f));
			//			Debug.Log(theta);
			currentVelocity = agentRb.transform.forward * agentScript.maxSpeed;
			agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position

			Debug.DrawRay(agent.transform.position, steeringAcc);
			Debug.DrawRay(agent.transform.position, desiredVelocity*20);
			Debug.DrawRay(agent.transform.position, currentVelocity*20);

			/*
						currentVelocity = Truncate(steeringAcc + currentVelocity, agentScript.maxSpeed);
						agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position
						Debug.Log(currentVelocity);
			*/
		}
	}
	/*
		void FixedUpdate ()
		{
			Vector3 distanceVector = target.position - agent.transform.position;  //Distance left to target
			Vector3 dirToTarget = distanceVector.normalized;              //Direction to target as a vector
			Vector3 desiredVelocity = dirToTarget * agentScript.maxSpeed;   //The desired velocity pointing towards the target
			float distance = distanceVector.magnitude;
			if(distance < slowingRadius)
			{
				desiredVelocity *= (distance / slowingRadius);
			}
			Vector3 steeringForce = desiredVelocity - currentVelocity;    //The steering force is the vector which we want to change the current direction with				
			steeringForce = Truncate(steeringForce, agentScript.maxForce);
			agentRb.AddRelativeForce(steeringForce);
			Debug.DrawLine(agentRb.position, steeringForce, Color.cyan);
			Debug.DrawLine(agentRb.position, desiredVelocity, Color.green);
			/*
			Vector3 steeringAcc = steeringForce / agentRb.mass;   //Converts force to acceleration
			currentVelocity = Truncate(steeringAcc + currentVelocity, agentScript.maxSpeed);
			agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position		
		}
	*/
	//Function for limiting the maximum absolute value of a vector
	Vector3 Truncate(Vector3 vec, float max)
	{
		float i = max / vec.magnitude;
		i = i < 1f ? i : 1f;      //If i is less than 1 then velocity is greater than maxVelocity
		Vector3 truncVec = i * vec;
		return truncVec;
	}
}
