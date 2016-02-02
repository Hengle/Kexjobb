using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

	public float maxVelocity;
	[HideInInspector] public Vector3 velocity;

	private GameObject target;
	private Vector3 mousePosition;

	void Start ()
	{
		//Creates vector with random values for velocity
		velocity = new Vector3(Random.Range(-maxVelocity, maxVelocity), 
			0f , Random.Range(-maxVelocity, maxVelocity));
		target = GameObject.FindGameObjectWithTag("Target");
		mousePosition = new Vector3();
	}
	
	void Update ()
	{
		//		target.transform.position += velocity;	
		SetTargetPosition();
	}
	//Sets the target position to the current mouse position
	void SetTargetPosition()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit floorHit;
		int floorMask = LayerMask.GetMask("FloorMask");
		if(Physics.Raycast(ray, out floorHit, 100, floorMask))
		{
			mousePosition = floorHit.point;
			target.transform.position = mousePosition;
		}
	}
	void OnTriggerEnter (Collider other)
	{
		if(other.GetType() == typeof(SphereCollider) && other.gameObject.tag.Equals("Agent"))
			Destroy(other.gameObject);
	}
}
