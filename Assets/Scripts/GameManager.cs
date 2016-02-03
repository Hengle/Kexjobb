using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject agent;
	public int spawnTime = 3;

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
			yield return new WaitForSeconds(spawnTime);
			//Destroy(agent);
		}
	}
}
