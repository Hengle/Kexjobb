﻿using UnityEngine;
using System.Collections;

public class HorizontalRowFormation : Formation
{
	private float distanceBetweenAgents = 10f;
	protected override void CreateTemplate()
	{
		float xPos = 0;
		for (int i = 0; i < templatePositions.Length; i++)
		{
			if (i % 2 != 0)
			{
				xPos += distanceBetweenAgents;
			}
			templatePositions[i] = new Vector3(xPos, 0f, 0f);
			xPos *= -1;
		}
	}
}
