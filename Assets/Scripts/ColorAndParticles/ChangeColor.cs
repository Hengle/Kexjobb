using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour {


	// Use this for initialization
	void Start () {

        // Random color
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject human = transform.GetChild(i).gameObject;
            GameObject theMesh = human.transform.GetChild(1).gameObject;
            SkinnedMeshRenderer renderer = theMesh.GetComponent<SkinnedMeshRenderer>();
            renderer.materials[0].color = color;

            //// Create small variation in animations
            human.GetComponent<Animator>().speed = Random.Range(0.3f,0.6f);
            //Animation anim = human.GetComponent<Animation>();
            //float length = anim["WalkForward"].length;
            //anim["WalkForward"].time = length * 0.5f;
        }
    }
}
