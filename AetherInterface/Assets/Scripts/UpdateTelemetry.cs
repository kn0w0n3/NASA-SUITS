using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.Networking;

public class Container {
	public GameObject container; //contains Panel game object
	public GameObject panel;	 //contains value, unit, info game objects
	public GameObject dataName;  //contains data point name found in variable-name-bar
	public GameObject dataValue; //contains data value found in Panel
	public GameObject dataUnit;	 //containes data unit found in Panel

	public Container(string containerName) {
		this.container = GameObject.Find(containerName);
		this.panel = this.container.transform.Find("Panel").gameObject;
		string dataNamePosition = "var-name-" + containerName[containerName.Length - 1];
		this.dataName = GameObject.Find(dataNamePosition);
		this.dataValue = this.panel.transform.Find("value").gameObject;
		this.dataUnit = this.panel.transform.Find("unit").gameObject;


		//this.container.GetComponent<ProceduralImage>().color = Color.red;
	}

	public void setData(DataPoints data) {
		this.dataName.GetComponent<Text>().text = data.name;
	}

	/*public void setValue(object value){
		int y = (int)value;
		this.dataValue.GetComponent<Text>().text = value.ToString();
		if(y > 50 && y < 100) {
			this.container.GetComponent<ProceduralImage>().color = Color.yellow;
		}
		if(y > 100) {
			this.container.GetComponent<ProceduralImage>().color = Color.red;
		}
	}*/

}


public class UpdateTelemetry : MonoBehaviour {

	Container container1; 
	Container container2; 
	Container container3;
	Container container4;
	Container container5;
	
	// Use this for initialization
	void Start () {
		container1 = new Container("container-1");
		container2 = new Container("container-2");
		container3 = new Container("container-3");
		container4 = new Container("container-4");
		container5 = new Container("container-5");
		StartCoroutine(GetTelemetry());	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void callShowAllTelem()
    {
        GameObject.Find("menu").GetComponent<Menu>().showAllTelemetry();
    }

	IEnumerator GetTelemetry() {
		while(true) {
			UnityWebRequest www = UnityWebRequest.Get("https://gemini-program.herokuapp.com/api/suit/recent");
			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			} else {
				Debug.Log(www.downloadHandler.text);
				string jsonStr = www.downloadHandler.text;
				NumericalTelemetry data = JsonUtility.FromJson<NumericalTelemetry>(jsonStr);

				//if time life battery is erroneous when received from server
				if(data.t_battery[0] == '-') {
					data.t_battery = "00:00:00";
				}

				DataPoints [] dataPoints = new DataPoints[5];

				dataPoints[0] = new DataPoints("Time Life Battery", data.t_battery, "hh:mm:ss", 0, 36000);
				dataPoints[1] = new DataPoints("Time Life Oxygen", data.t_oxygen, "hh:mm:ss", 0, 36000);
				dataPoints[2] = new DataPoints("Time Life Water", data.t_water, "hh:mm:ss", 0, 36000);
				dataPoints[3] = new DataPoints("Oxygen Pressure", data.p_o2, "psia", 750f, 950f);
				dataPoints[4] = new DataPoints("Oxygen Rate", data.rate_o2, "psi/min", 0.5f, 1.0f);

				container1.setData(dataPoints[0]);
		
			}
			//make a request every 5 seconds
			yield return new WaitForSeconds(5);
		}
	}
}
//critical variables will be received from numerical and switch update scripts 
//the data displayed onto the simple data ui should be consistent with the requests that are made
//red values shown first, yellow second, green values have least priority
//if all variables are green, then display priority data (time data)
//

/* 
	-get telemetry data (local or server)
	-from telemetry object create sub-objects(individual datapoints) that holds name, value, unit, pass/fail state
	-pass sub-objects to container, update name, value, unit, and color of container
*/


/* 
	Canvas
		Panel
			variable-name-bar
				var-name-*
			container-*
				Panel
					value
					unit
					info
 */

 /*
 		Color myColor;
		ColorUtility.TryParseHtmlString("#1E90FF", out myColor);
		this.container.GetComponent<ProceduralImage>().color = myColor;
		//OR
		this.container.GetComponent<ProceduralImage>().color = Color.white;
*/
