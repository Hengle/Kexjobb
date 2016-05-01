using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour
{
	public float maxSpeed = 5;
	public float maxForce = 10;
	public float rotSpeed = 1;
	public float radius = 2.5f;
	public float force = .5f;
	public float arrivalRadius = .1f;
	public float deltaSpeed = 1;
	public Vector3 targetPos;

//	private Material m;
	private Rigidbody rb;
	private float speed;
	private bool isArriving;
	private bool hasArrived;
	void Awake()
	{
		rb = GetComponent<Rigidbody>();
//		m = GetComponent<Material>();
		isArriving = false;
		hasArrived = false;
	}
	void Animating()
	{
	}
	/*
	void Update()
	{

//		if (IsArriving) m.color = Color.blue;
//		else m.color = Color.red;
		float distanceToTarget = (targetPos - transform.position).magnitude;

		if (!hasArrived)
		{
			Vector3 targetDir = (targetPos - transform.position).normalized;
			Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotSpeed * Time.deltaTime, 0.0f);
			transform.rotation = Quaternion.LookRotation(newDir);

			if (distanceToTarget < arrivalRadius)
			{
				speed = maxSpeed * distanceToTarget * distanceToTarget;
				isArriving = true;
			}
			else
			{
				if (speed < maxSpeed)
					speed += deltaSpeed * Time.deltaTime;
				isArriving = false;
			}
		}

		if (distanceToTarget < 0.01f)
		{
			hasArrived = true;
			speed = 0;
		}

		rb.velocity = speed * transform.forward;

	}
	*/
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(targetPos, 0.25f);
	}

	void FixedUpdate()
	{
		//		rb.AddForce(transform.forward * force);
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
