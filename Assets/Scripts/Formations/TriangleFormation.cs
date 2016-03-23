using UnityEngine;
using System.Collections;
using System;

public class TriangleFormation : Formation {

	protected override void CreateTemplate()
	{
		templatePositions[0] = new Vector3(0f, 0f, 0f);
		templatePositions[1] = new Vector3(-5f, 0f, -10f);
		templatePositions[2] = new Vector3(5f, 0f, -10f);

	}
}
