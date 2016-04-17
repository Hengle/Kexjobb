using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour
{
	public float maxSpeed = 1;
	public float maxForce = 10;
	public float rotSpeed = 1;
	public float radius = 2.5f;
	public float force = .5f;
	public float arrivalRadius = 2;
	public Vector3 targetPos;

	private Rigidbody rb;
	private float speed;
	private bool isArriving;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}
	void Animating()
	{
	}

	void Update()
	{
		if (!isArriving)
		{
			Vector3 targetDir = (targetPos - transform.position).normalized;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotSpeed * Time.deltaTime, 0.0f);
			transform.rotation = Quaternion.LookRotation(newDir);
		}
		float distanceToTarget = (targetPos - transform.position).magnitude;
		if (distanceToTarget < arrivalRadius)
		{
			speed = maxSpeed * distanceToTarget;
			isArriving = true;
		}
		else
		{
			speed = maxSpeed;
			isArriving = false;
		}
	}
	void FixedUpdate()
	{
		//		rb.AddForce(transform.forward * force);
		rb.velocity = speed * transform.forward;
	}
	public Vector3 TargetPoint
	{
		get { return targetPos; }
		set { targetPos = value; }
	}
	
	public bool IsArriving
	{
		get { return isArriving; }
	}
}
