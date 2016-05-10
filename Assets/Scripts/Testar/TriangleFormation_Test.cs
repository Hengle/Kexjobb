using UnityEngine;
using System.Collections;
using System;

public class TriangleFormation_Test : Formation_Test {

	private float rowDistance = 7.5f;    //Distance between rows of agents
	private float columnDistance = 7.5f;	//Distance between agents on each row

	protected override void CreateTemplate()
	{
		float xPos = 0;
		float zPos = 0;
		int agentCounter = 0;
		for(int i = 0; i < templatePositions.Length; i++)		//For each row
		{
			for(int j = 0; j <= i; j++)		//For each column
			{
				if (agentCounter >= templatePositions.Length)
					return;
				templatePositions[agentCounter++] = new Vector3(xPos, 0f, zPos);
				xPos += columnDistance;
			}
			xPos = -((i + 1) * 5);
			zPos -= rowDistance;
		}
	}
}
