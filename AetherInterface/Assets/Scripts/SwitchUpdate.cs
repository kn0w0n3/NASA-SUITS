using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;
using UnityEngine.Networking;

//object that will be created to hold incoming switch data
[System.Serializable]
public class SwitchTelemetry {
	//data received from server
	public bool sop_on;
	public bool sspe;
	public bool fan_error;
	public bool vent_error;
	public bool vehicle_power;
	public bool h2o_off;
	public bool o2_off;	//primary and secondary oxygen tanks are offline when value = true

	//data not received from server
	public bool battery_amp_high;
	public bool battery_vdc_low;
	public bool suit_pressure_low;
	public bool spacesuit_pressure_high;
	public bool o2_high_use;
	public bool sop_pressure_low;
	public bool co2_high;

	public SwitchTelemetry() {
		this.battery_amp_high = false;
		this.battery_vdc_low = false;
		this.suit_pressure_low = false;
		this.spacesuit_pressure_high = false;
		this.o2_high_use = false;
		this.sop_pressure_low = false;
		this.co2_high = false;
	}
}

//contains individual telemetry data points
public class SwitchDataPoints {
	public string name;
	public object status;

	public SwitchDataPoints(string name, object status) {
		this.name = name;
		this.status = status;
	}
}

//Switch UI 
public class SwitchDataUI {
	public GameObject panel;
	public GameObject dataName;
	public GameObject dataValue;

	public SwitchDataUI(string name, SwitchDataPoints data) {
		this.panel = GameObject.Find(name);

		this.dataName = this.panel.transform.Find("name").gameObject;
		this.dataName.GetComponent<Text>().text = data.name;

		this.dataValue = this.panel.transform.Find("status").gameObject;
		string statusString = data.status.ToString();
		this.dataValue.GetComponent<Text>().text = statusString;

	}
}

public class SwitchUpdate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(GetSwitch());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
    public void callShowTop5() {
        GameObject.Find("menu").GetComponent<Menu>().createTelemtry();
    }

	IEnumerator GetSwitch() {
		while(true) {
			//UnityWebRequest www = UnityWebRequest.Get("https://nasa-bhitt.c9users.io/api/suitswitch/recent");
			UnityWebRequest www = UnityWebRequest.Get("https://gemini-program.herokuapp.com/api/suitswitch/recent");
			yield return www.SendWebRequest();

			if(www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			} else {
				//Debug.Log(www.downloadHandler.text);
				string jsonString = www.downloadHandler.text;
				jsonString = jsonString.Replace('[',' ').Replace(']',' ');

				//parse data received from server and store in object
				SwitchTelemetry switchData = JsonUtility.FromJson<SwitchTelemetry>(jsonString);

				//change numSwitchPoints when adding more data to UI
				int numSwitchPoints = 14;
				SwitchDataPoints [] dataPoints = new SwitchDataPoints [numSwitchPoints];

				//create individual data points with names and status 
				setSwitchDataPoints(switchData, dataPoints);

				//place switch data onto ui with corresponding name, status, and color
				makeSwitchDataUI(dataPoints);
			}

			//make a request every 5 seconds
			yield return new WaitForSeconds(5);
		}
	}

	public void setSwitchDataPoints(SwitchTelemetry data, SwitchDataPoints [] dataPoints) {
		//dataPoints[] = new SwitchDataPoints("", data.);
		dataPoints[0] = new SwitchDataPoints("SOP On", data.sop_on);
		dataPoints[1] = new SwitchDataPoints("Spacesuit Pressure Emergency", data.sspe);
		dataPoints[2] = new SwitchDataPoints("Fan Failure", data.fan_error);
		dataPoints[3] = new SwitchDataPoints("No Vent Flow", data.vent_error);
		dataPoints[4] = new SwitchDataPoints("Vehicle Power Present", data.vehicle_power);
		dataPoints[5] = new SwitchDataPoints("H2O is off", data.h2o_off);
		dataPoints[6] = new SwitchDataPoints("O2 is off", data.o2_off);

		//data not coming from server
		dataPoints[7] = new SwitchDataPoints("Battery AMP High", data.battery_amp_high);
		dataPoints[8] = new SwitchDataPoints("Battery VDC Low", data.battery_vdc_low);
		dataPoints[9] = new SwitchDataPoints("Suit Pressure Low", data.suit_pressure_low);
		dataPoints[10] = new SwitchDataPoints("Spacesuit Pressure High", data.spacesuit_pressure_high);
		dataPoints[11] = new SwitchDataPoints("O2 High Use", data.o2_high_use);
		dataPoints[12] = new SwitchDataPoints("SOP Pressure Low", data.sop_pressure_low);
		dataPoints[13] = new SwitchDataPoints("CO2 High", data.co2_high);
	}

	public void makeSwitchDataUI(SwitchDataPoints [] dataPoints) {
		SwitchDataUI [] dataUIArray = new SwitchDataUI[dataPoints.Length];
		
		for(int i=0; i<dataUIArray.Length; i++) {
			dataUIArray[i] = new SwitchDataUI(("switch-" + (i+1)), dataPoints[i]);
		}
	}
}