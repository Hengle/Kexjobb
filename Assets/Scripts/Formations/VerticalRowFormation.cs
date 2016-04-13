using UnityEngine;
using UnitySteer.Behaviors;
using System.Collections;
using System;

public class VerticalRowFormation : Formation {
	protected override void CreateTemplate()
	{
		templatePositions[0] = new Vector3(0f, 0f, 0f);
		templatePositions[1] = new Vector3(0f, 0f, -10f);
		templatePositions[2] = new Vector3(0f, 0f, -20f);
	}

}
