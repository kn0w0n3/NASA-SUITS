using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TelemetryUI : MonoBehaviour 
{
	public Button button;

	// Use this for initialization
	void Start () 
	{
		Button bttn = button.GetComponent<Button>();
		bttn.onClick.AddListener(foo);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void foo() 
	{
		Debug.Log("runnning");
	}
}
