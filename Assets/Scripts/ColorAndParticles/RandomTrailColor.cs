using UnityEngine;
using System.Collections;
using UnityEditor;

public class RandomTrailColor : MonoBehaviour {

    public bool is3DAgent = false;

	// Use this for initialization
	void Start()
    {
        Color[] colors = { Color.black, Color.blue, Color.green, Color.red, Color.magenta };
        
        Color randCol = colors[(int)Mathf.Floor(Random.Range(0, colors.Length))];

        Color randCol2 = new Color(Random.Range(0f,1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject theAgent = transform.GetChild(i).gameObject;
            int trailIndex = 1;
            if(is3DAgent) { trailIndex = 2; }
            GameObject trailObject = theAgent.transform.GetChild(trailIndex).gameObject;
            TrailRenderer trail = trailObject.GetComponent<TrailRenderer>();

            SerializedObject so = new SerializedObject(trail);

            for(int j = 0; j < 5; j++)
            {
                so.FindProperty("m_Colors.m_Color["+ j +"]").colorValue = randCol2;
            }
            so.ApplyModifiedProperties();
        }
    	
	}
}
