using UnityEngine;
using System.Collections;

public class SpawnAreaScript : MonoBehaviour {

	private Bounds bounds;

	// Use this for initialization
	void Awake()
	{
		bounds = GetComponent<BoxCollider>().bounds;
	}

	public Vector3 RandomizePosition()
	{
		Vector3 max = bounds.max;
		Vector3 min = bounds.min;
		Vector3 pos = new Vector3(Random.Range(min.x, max.x), 0f, Random.Range(min.z, max.z));
		return pos;
	}
}
