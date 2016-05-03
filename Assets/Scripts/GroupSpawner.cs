using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GroupSpawner : MonoBehaviour {

	public float width;
	public float height;
	public float spawnTime = 3;
	public GameObject group2;
	public GameObject group3;

	private List<GameObject> groups;
	private System.Random random;
	private RVOController rvo;

	void Awake()
	{
		rvo = GetComponent<RVOController>();
	}
	// Use this for initialization
	void Start () {
		groups = new List<GameObject>();
		random = new System.Random();
		InvokeRepeating("Spawn", spawnTime, spawnTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//TODO: Implement this 
	void Spawn()
	{
		//Randomize start position for group
		float minX = transform.position.x - width / 2;
		float maxX = transform.position.x + width / 2;
		float minZ = transform.position.z - height / 2;
		float maxZ = transform.position.z + height / 2;
		Vector3 startPosition = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(minZ, maxZ));

		//Randomize nr of agents in group (2 or 3)
		int nrOfAgents = UnityEngine.Random.Range(2, 3);

		//Randomize group formation
		Array values = Enum.GetValues(typeof(FormationState));
		FormationState formation = (FormationState)values.GetValue(random.Next(values.Length));

		//Instantiate the new agents
		InstantiateGroup(nrOfAgents, startPosition, formation);

		
		//Update the RVOController with the new agents

	}
	void InstantiateGroup(int nrOfAgents, Vector3 startPosition, FormationState formation)
	{
		//Spawn group at startPosition
		if(nrOfAgents == 2)
		{
			GameObject group = Instantiate(group2, startPosition, Quaternion.identity) as GameObject;
			group.transform.parent = transform;
			groups.Add(group);
			
		}
		else if(nrOfAgents == 3)
		{
			GameObject group = Instantiate(group3, startPosition, Quaternion.identity) as GameObject;
			group.transform.parent = transform;
			groups.Add(group);
		}
		switch (formation)
		{
			case FormationState.HorizontalRow:
				groups.Last().AddComponent<HorizontalRowFormation>().enabled = true;
				break;
			case FormationState.Triangle:
				groups.Last().AddComponent<TriangleFormation>().enabled = true;
				break;
			case FormationState.VerticalRow:
				groups.Last().AddComponent<VerticalRowFormation>().enabled = true;
				break;
		}
//		groups.Last().GetComponent<Formation>().enabled = true;
		groups.Last().GetComponent<Formation>().UpdateLeadersTarget(startPosition + new Vector3(0f, 0f, 50f));
		rvo.UpdateController();
	}

	public List<GameObject> Groups
	{
		get { return groups; }
	}
}
