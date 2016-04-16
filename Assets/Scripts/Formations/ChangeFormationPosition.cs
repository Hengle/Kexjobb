using UnityEngine;
using System.Collections;

//Changes the formation at a specific point
public class ChangeFormationPosition : MonoBehaviour
{
	public Vector3 positionForChange;   //The position where to change formation

	private Vector3 leaderPosition;
	private Formation currentFormation;
	// Use this for initialization
	void Start()
	{
		leaderPosition = transform.GetChild(0).transform.position;
		foreach(Formation formation in GetComponents<Formation>())
		{
			currentFormation = formation.enabled ? formation : currentFormation;			
		}
	}

	// Update is called once per frame
	void Update()
	{
		leaderPosition = transform.GetChild(0).transform.position;
		if((leaderPosition - positionForChange).magnitude < 1)
		{
			GetComponent<VerticalRowFormation>().enabled = true;
			currentFormation.enabled = false;
		}
	}
}
