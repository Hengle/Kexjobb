﻿using UnityEngine;
using System.Collections;

public class Steering : MonoBehaviour {

	public GameObject agent;
	public float slowingRadius = 10;
	public bool flee = false;

	private Transform target;
	private Vector3 tempTarget;
	private Target targetScript;
	private Vector3 dirToTarget;
	private Agent agentScript;
	private Vector3 currentVelocity;
	private Rigidbody agentRb;
	private SphereCollider agentCollider;
	private int raycastDistance = 10;			//The distance in front of the agent which we want to check for obstacles
	void Awake()
	{
		agentRb = agent.GetComponent<Rigidbody>();
		agentScript = agent.GetComponent<Agent>();
		agentCollider = agent.GetComponent<SphereCollider>();
		target = GameObject.FindGameObjectWithTag("Target").transform;
		targetScript = target.GetComponent<Target>();
	}
	void Start ()
	{
		currentVelocity = new Vector3(0f, 0f, 0f);
		dirToTarget = (target.position - agent.transform.position).normalized;
	}

	void Update ()
	{
		if (flee)
			Flee();
		else
			SeekWithObstacleAvoidance();
		//Pursuit();
	}
	//Moves towards target
	void Seek ()
	{
		Vector3	distanceVector = target.position - agent.transform.position;  //Distance left to target
		dirToTarget = distanceVector.normalized;              //Direction to target as a vector
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
		agentRb.MoveRotation(Quaternion.LookRotation(currentVelocity));
	}
	//Seek towards the target while also avoiding obstacles in the way
	void SeekWithObstacleAvoidance ()
	{
		Vector3 distanceVector = target.position - transform.position;  //Distance left to target
		dirToTarget = distanceVector.normalized;              //Direction to target as a vector

		RaycastHit hit;
		Ray ray = new Ray(transform.position, transform.forward);   //Creates a ray from agents position in the forward direction
		//Check for obstacles
		if (Physics.Raycast(ray, out hit, raycastDistance))
			if (hit.transform != transform && hit.collider.tag == "Obstacle")
			{
//				Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
				dirToTarget += hit.normal * 10f;		//Moves the target direction
			}

		Vector3 left = transform.position;		//Gives the agent some sideway distance
		Vector3 right = transform.position;

		left.x -= 2;  //Vector to the left of agents current position
		right.x += 2; //Vector to the right of agents current position

		if (Physics.Raycast(left, transform.forward, out hit, raycastDistance))
			if (hit.transform != transform && hit.collider.tag == "Obstacle")
			{
//				Debug.DrawRay(left, transform.forward * 10, Color.red);
				dirToTarget += hit.normal * 10f;
			}
		if (Physics.Raycast(right, transform.forward, out hit, raycastDistance))
			if (hit.transform != transform && hit.collider.tag == "Obstacle")
			{
//				Debug.DrawRay(right, transform.forward * 10, Color.red);
				dirToTarget += hit.normal * 10f;
			}
		Quaternion rot = Quaternion.LookRotation(dirToTarget);

		transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
		transform.position += transform.forward * agentScript.maxSpeed * Time.deltaTime;

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
	//Obstacle avoidance using colliders
	void OnTriggerStay(Collider target)
	{
		SphereCollider targetSc = target.gameObject.GetComponent<SphereCollider>();
		//We only want to avoid the obstacle objects
		if(target.gameObject.tag.Equals("Obstacle"))
		{

			Vector3 steering;
			float targetX = target.gameObject.transform.position.x;
			float agentX = agent.transform.position.x;
//			float horizontalDistance = targetX - agentX; //Distance from agent to obstacle on x-axis

			float steeringForce = (targetX - targetSc.radius) - (agentX + agentCollider.radius);
			steering = new Vector3(0f, 0f, steeringForce);

			steering = Truncate(steering, agentScript.maxForce);
			Vector3 steeringAcc = (steering / agentRb.mass);   //Converts force to acceleration
			Vector3 desiredVelocity = (currentVelocity + steeringAcc);
			float theta = Vector3.Angle(desiredVelocity, currentVelocity);
			agentRb.MoveRotation(Quaternion.Euler(0f, theta, 0f));
			currentVelocity = agentRb.transform.forward * agentScript.maxSpeed;
			agentRb.position = agentRb.position + currentVelocity;  //Sets the new agent position		
		}
	}
	//Function for limiting the maximum absolute value of a vector
	Vector3 Truncate(Vector3 vec, float max)
	{
		float i = max / vec.magnitude;
		i = i < 1f ? i : 1f;      //If i is less than 1 then velocity is greater than maxVelocity
		Vector3 truncVec = i * vec;
		return truncVec;
	}
}
