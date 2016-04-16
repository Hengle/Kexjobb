using UnityEngine;
using UnitySteer.Behaviors;
using System.Collections;
using System;

public class VerticalRowFormation : Formation {

	private float distanceBetweenAgents = 10f;

	protected override void CreateTemplate()
	{
		float zPos = 0;
		for(int i = 0; i < templatePositions.Length; i++)
		{
			templatePositions[i] = new Vector3(0f, 0f, zPos);
			zPos -= distanceBetweenAgents;
		}

/*
		templatePositions[0] = new Vector3(0f, 0f, 0f);
		templatePositions[1] = new Vector3(0f, 0f, -10f);
		templatePositions[2] = new Vector3(0f, 0f, -20f);
*/
	}

}
