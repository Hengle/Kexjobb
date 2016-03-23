using UnityEngine;
using System.Collections;
using UnitySteer.Behaviors;
public class AssignTargetScript2 : MonoBehaviour {
	public Transform targetPoint;
	// Use this for initialization
	void Start () {
		foreach(Transform child in transform)
		{
			child.GetComponent<SteerToFollow>()._target = targetPoint;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
