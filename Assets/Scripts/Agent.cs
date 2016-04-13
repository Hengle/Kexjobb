using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour
{

	public float maxSpeed = 5;
	public float maxForce = 10;

	private Animator anim;

	void Awake()
	{
		anim = GetComponent<Animator>();
	}
	void Animating()
	{
		anim.SetBool("IsWalking", true);
	}
	void Start()
	{
	}

	void Update()
	{
	}

}
