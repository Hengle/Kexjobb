using UnityEngine;
using System.Collections;

public class SpawnAreasScript : MonoBehaviour {

	private GameObject[] spawnAreas;
	private int nrOfAreas;
	private int areaCounter = 0;

	void Start()
	{
		spawnAreas = new GameObject[transform.childCount];
		nrOfAreas = spawnAreas.Length;
		for(int i = 0; i < nrOfAreas; i++)
		{
			spawnAreas[i] = transform.GetChild(i).gameObject;
		}
	}
	public Vector3 GenerateRandomStartPos()
	{
		SpawnAreaScript currentArea = spawnAreas[areaCounter].GetComponent<SpawnAreaScript>();
		Vector3 pos = currentArea.RandomizePosition();
		areaCounter++;
		areaCounter %= nrOfAreas;
		return pos;
	}
	public Vector3[] GenerateRandomStartPosAndGoal()
	{
		SpawnAreaScript startArea = spawnAreas[areaCounter].GetComponent<SpawnAreaScript>();
		SpawnAreaScript goalArea = spawnAreas[(areaCounter + 1) % nrOfAreas].GetComponent<SpawnAreaScript>();
		Vector3 startPos = startArea.RandomizePosition();
		Vector3 goalPos = goalArea.RandomizePosition();
		Vector3[] pos = { startPos, goalPos };
		areaCounter++;
		areaCounter %= nrOfAreas;
		return pos;

	}
}
