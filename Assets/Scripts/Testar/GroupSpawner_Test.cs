using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GroupSpawner_Test : MonoBehaviour {

	public float width; // Width of spawn area
	public float height; // Height of spawn area
	public float spawnTime = 3; // Delay between spawns
	public GameObject group2;
	public GameObject group3;

	private List<GameObject> groups; // List of groups that have spawned
	private System.Random random; // For easier access to random
	private RVOController_Test rvo; // For access to RVOController Script
    private Vector3 randomStart; // For random star position

    // Reference RVOController script
	void Awake()
	{
		rvo = GetComponent<RVOController_Test>();
	}

	void Start ()
    {
		groups = new List<GameObject>();
		random = new System.Random();
        InvokeRepeating("Spawn", 1, spawnTime);
    }


    void Spawn()
	{
		//Randomize start position for group
		float minX = transform.position.x - width / 2;
		float maxX = transform.position.x + width / 2;
		float minZ = transform.position.z - height / 2;
		float maxZ = transform.position.z + height / 2;
		Vector3 startPosition = new Vector3(UnityEngine.Random.Range(minX, maxX), 0f, UnityEngine.Random.Range(minZ, maxZ));
        randomStart = startPosition; // Use this in formation

		//Randomize nr of agents in group (2 or 3)
		int nrOfAgents = UnityEngine.Random.Range(2, 3);

		//Randomize group formation
		Array values = Enum.GetValues(typeof(FormationState));
		FormationState formation = (FormationState)values.GetValue(random.Next(values.Length));

		//Instantiate the new agents
		InstantiateGroup(nrOfAgents, startPosition, formation);

    }

    // Spawns a group ini the scene
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

        // Add the right formation script to the group
		switch (formation)
		{
			case FormationState.HorizontalRow:
                groups.Last().AddComponent<HorizontalRowFormation_Test>().enabled = true;
                //group.AddComponent<HorizontalRowFormation_Test>().enabled = true;
                break;
			case FormationState.Triangle:
                groups.Last().AddComponent<TriangleFormation_Test>().enabled = true;
                //group.AddComponent<TriangleFormation_Test>().enabled = true;
                break;
			case FormationState.VerticalRow:
                groups.Last().AddComponent<VerticalRowFormation_Test>().enabled = true;
                //group.AddComponent<VerticalRowFormation_Test>().enabled = true;
                break;
		}

        //Update the RVOController with the new agents
        rvo.AddGroupToSim(groups.Last());
    }

    public List<GameObject> Groups
	{
		get { return groups; }
	}
    public Vector3 RandomStartPos
    {
        get { return randomStart; }
    }
}
