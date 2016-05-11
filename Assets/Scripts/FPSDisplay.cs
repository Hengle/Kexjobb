using UnityEngine;
using System.Collections.Generic;
 
public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	float fps;
	RVOController_Test rvo;
	List<string> data;
	int oldNrOfAgents;
 
	void Awake()
	{
		rvo = GetComponent<RVOController_Test>();
		oldNrOfAgents = 0;
		data = new List<string>();
	}

	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

		int nrOfAgents = rvo.NrOfAgents;

		if (oldNrOfAgents != nrOfAgents)
		{
			string newData = nrOfAgents + ", " + (int)fps;
			data.Add(newData);
			oldNrOfAgents = nrOfAgents;
		}
		if(nrOfAgents >= 50)
		{
			System.IO.File.WriteAllLines(@"C:\Users\jonas\Documents\GitHub\Kexjobb\Assets\Data\fpsData.txt", data.ToArray());
		}
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);



		GUI.Label(rect, text, style);
	}
}