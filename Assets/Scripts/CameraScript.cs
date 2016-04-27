using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
	private RVOControllerCircle controller;
//	private float minSize;
	private float padding;
	void Awake()
	{
		controller = GetComponent<RVOControllerCircle>();
		
	}
	// Use this for initialization
	void Start()
	{
		//		minSize = 100f;
		padding = 20f;
		Camera.main.orthographicSize = controller.circleRadius + padding;
	}

	// Update is called once per frame
	void Update()
	{
		float size = controller.distanceToMid + padding;
		Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, Time.deltaTime);
	}
}
