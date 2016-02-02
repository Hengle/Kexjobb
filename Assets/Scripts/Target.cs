using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

	public float maxVelocity;
	[HideInInspector] public Vector3 velocity;

	private GameObject target;

	void Start ()
	{
		//Creates vector with random values for velocity
		velocity = new Vector3(Random.Range(-maxVelocity, maxVelocity), 
			0f , Random.Range(-maxVelocity, maxVelocity));
		target = GameObject.FindGameObjectWithTag("Target");
	}
	
	void Update ()
	{
		target.transform.position += velocity;	
	}
	void OnTriggerEnter (Collider other)
	{
		Destroy(other.gameObject);
	}
}
