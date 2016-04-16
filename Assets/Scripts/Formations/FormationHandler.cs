﻿using UnityEngine;
using System.Collections;

//Class for making it easier to change between formations

public class FormationHandler : MonoBehaviour {
	public enum FormationState
	{
		HorizontalRow, Triangle, VerticalRow
	}
	public Formation[] formations;
	public FormationState currentFormation;

	private FormationState oldFormation;

	void Start () {
		Destroy(GetComponent<Formation>());
		switch (currentFormation)
		{
			case FormationState.HorizontalRow:
				gameObject.AddComponent<HorizontalRowFormation>().enabled = true;
				oldFormation = FormationState.HorizontalRow;
				break;
			case FormationState.Triangle:
				gameObject.AddComponent<TriangleFormation>().enabled = true;
				oldFormation = FormationState.Triangle;
				break;
			case FormationState.VerticalRow:
				gameObject.AddComponent<VerticalRowFormation>().enabled = true;
				oldFormation = FormationState.VerticalRow;
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(currentFormation != oldFormation)		//Dont want to remove and create scripts if unnecesary
		{
			Destroy(GetComponent<Formation>());
			switch (currentFormation)
			{
				case FormationState.HorizontalRow:
					gameObject.AddComponent<HorizontalRowFormation>().enabled = true;
					oldFormation = FormationState.HorizontalRow;
					break;
				case FormationState.Triangle:
					gameObject.AddComponent<TriangleFormation>().enabled = true;
					oldFormation = FormationState.Triangle;
					break;
				case FormationState.VerticalRow:
					gameObject.AddComponent<VerticalRowFormation>().enabled = true;
					oldFormation = FormationState.VerticalRow;
					break;
			}
		}
	}

}