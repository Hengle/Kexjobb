#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using NUnit.Framework;

public class AnimationHelper : EditorWindow
{
	public GameObject target;
	public AnimationClip walkAnim;

	[MenuItem ("Window/Animation Helper")]
	static void OpenWindow ()
	{
		// Get existing open window or if none, make a new one
		GetWindow<AnimationHelper>();
	}
	void OnGUI()
	{
		target = EditorGUILayout.ObjectField("Target Object", target, typeof(GameObject), true) as GameObject;
		walkAnim = EditorGUILayout.ObjectField("Walk", walkAnim, typeof(AnimationClip), false) as AnimationClip;

		if(GUILayout.Button("Create"))
		{
			if (target == null)
			{
				Debug.LogError("No target for animator controller set.");
				return;
			}
			Create();
		}

	}
	void Create()
	{

	}
	[Test]
	public void EditorTest()
	{
		//Arrange
		var gameObject = new GameObject();

		//Act
		//Try to rename the GameObject
		var newGameObjectName = "My game object";
		gameObject.name = newGameObjectName;

		//Assert
		//The object has a new name
		Assert.AreEqual(newGameObjectName, gameObject.name);
	}
}
#endif