using UnityEngine;
using System.Collections;
using UnityEditor;

public class AssignRandomColor : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Color randCol2 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        TrailRenderer trail = GetComponent<TrailRenderer>();

        SerializedObject so = new SerializedObject(trail);

        for (int j = 0; j < 5; j++)
        {
            so.FindProperty("m_Colors.m_Color[" + j + "]").colorValue = randCol2;
        }
        so.ApplyModifiedProperties();
    }
}
