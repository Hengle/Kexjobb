using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject agent;


	void Start ()
	{
		StartCoroutine(spawnAgents());

	}

	void Update ()
	{
	}
	IEnumerator spawnAgents()
	{
		while (true)
		{
			Instantiate(agent);
			yield return new WaitForSeconds(3);
			Destroy(agent);
		}
	}
}
