using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

public class ColorShift : MonoBehaviour {

    public Color colorA;
    public Color colorB;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<ProceduralImage>().color = Color.Lerp(colorA, colorB, Mathf.Sin(Time.realtimeSinceStartup * (2 * Mathf.PI)));
	}
}
