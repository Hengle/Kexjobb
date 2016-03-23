﻿using UnityEngine;
using System.Collections;

public class RowFormation : Formation {

	protected override void CreateTemplate()
	{
		templatePositions[0] = new Vector3(0f, 0f, 0f);
		templatePositions[1] = new Vector3(10f, 0f, 0f);
		templatePositions[2] = new Vector3(20f, 0f, 0f);
	}
}
