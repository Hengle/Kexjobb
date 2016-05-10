using UnityEngine;
using System.Collections;

public class Agent_Test : MonoBehaviour
{

	[SerializeField]
	private Vector3 startPos;
	[SerializeField]
	private Vector3 targetPos;
	[SerializeField]
	private RVO.Vector2 targetPosRVO;


	void Start()
	{
		startPos = new Vector3();
		targetPos = new Vector3();
		targetPosRVO = new RVO.Vector2();
	}


	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(targetPos, 0.25f);
	}

	public Vector3 StartPos
	{
		get { return startPos; }
		set { startPos = value; }
	}

	public Vector3 TargetPos
	{
		get { return targetPos; }
		set { targetPos = value; }
	}

	public RVO.Vector2 TargetPosRVO
	{
		get { return targetPosRVO; }
		set { targetPosRVO = value; }
	}

	public RVO.Vector2 CurrentPosRVO
	{
		get { return new RVO.Vector2(transform.position.x, transform.position.z); }
	}

}
