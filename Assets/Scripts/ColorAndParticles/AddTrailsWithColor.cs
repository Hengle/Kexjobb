using UnityEngine;
using System.Collections;
using UnityEditor;

public class AddTrailsWithColor : MonoBehaviour
{
    public GameObject theTrail;
    
    // Use this for initialization
    void Start()
    {
        for(int i = 0; i < transform.childCount-1; i++)
        {
            GameObject trail = Instantiate(theTrail, transform.position, Quaternion.identity) as GameObject;
            trail.transform.parent = transform.GetChild(i).transform;

            TrailRenderer rend = trail.GetComponent<TrailRenderer>();

            SerializedObject so = new SerializedObject(trail);
            Color randCol2 = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            for (int j = 0; j < 5; j++)
            {
                so.FindProperty("m_Colors.m_Color[" + j + "]").colorValue = randCol2;
            }
            so.ApplyModifiedProperties();
        }
    }
}
