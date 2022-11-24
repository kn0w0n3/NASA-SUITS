using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
//needed for JSON
using UnityEngine.Networking;


//blueprint for object parsed from JSON
//  this is used with NASA's given format
[System.Serializable]
public class teleObj    //telemetry data
{
    public string create_date;	//date/time the packet of data was created
	public float heart_bpm;		//heart beats per minute
    //public float i_suit_p;    	//Internal Suit Pressure (range:2-4 psid)
    public float p_sub;       	//Sub pressure      (range: 2-4 psia)
    public float t_sub;       	//Sub temperature   (degrees Fahrenheit)
    public int v_fan;       	//Fan tachometer    (range: 10000 - 40000 RPM)
    //public string t_eva;    	//EVA time          (stopwatch for current EVA)
    public float p_o2;        	//Oxygen pressure   (range: 750 - 950 psia)
    public float rate_o2;   	//Oxygen rate       (range: 0.5 - 1 psi/min)
    public float cap_battery; 	//Battery Capacity  (range: 0 - 30 amp-hr)
    public float p_h2o_g;     	//H2O gas pressure  (range: 14 - 16 psia)
    public float p_h2o_l;     	//H20 liquid pressure            (range: 14 - 16 psia)
    public float p_sop;       	//Secondary Oxygen Pack pressure (range: 750 - 950)
    public float rate_sop;  	//Oxygen rate for Secondary      (range: 0.5 - 1 psi/min)
	public string t_battery;    //time life battery (range: 0-10 hours) ("hh:mm:ss")
    public string t_oxygen;    	//time life oxygen (range: 0-10 hours) ("hh:mm:ss")
    public string t_water;    	//time life water (range: 0-10 hours) ("hh:mm:ss")

    public static teleObj crFmJsn(string jsonString) {
    	string temp = jsonString.Replace('[',' ').Replace(']',' ');
        return JsonUtility.FromJson<teleObj>(temp);
    }

}


public class JSON_input : MonoBehaviour {
	//public variables
    float timer = 0;
	int dataPC = 15; //data point count in JSON string
    public teleObj latestT;
    public List<teleObj> tDataS;   //telemetry data set
    public List<float> meanSet;
    public float[] deviats;
	
    // Use this for initialization
    void Start() {
        tDataS = new List<teleObj>();
        deviats = new float[dataPC];
        for (int i = 0; i < dataPC; i++) deviats[i] = 0;
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (timer >= 10)
        {
            StartCoroutine(GetJson());
            timer -= 10;
        }
    }

    //
    IEnumerator GetJson() {

        //Follows NASA format, only works if Branden's server is up and running
        using (UnityWebRequest www = UnityWebRequest.Get("https://telemetry-stream-bhitt.c9users.io/numerical")) {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                //string jsonData = www.text;
                //parse json
                //Debug.Log(www.downloadHandler.text);

                teleObj teleData = teleObj.crFmJsn(www.downloadHandler.text); //create from JSON

                //print out properties  (older version of debug... needs updating to be used)
                //Debug.Log("Internal Suit Pressure: " + teleData.i_suit_p);
                //Debug.Log("Time Life Battery: " + teleData.t_life_battery);
                //Debug.Log("Time Life Oxygen: " + teleData.t_life_oxygen);
                //Debug.Log("Time Life Water: " + teleData.t_life_water);
                //Debug.Log("Sub pressure: " + teleData.p_sub);
                //Debug.Log("Sub temperature: " + teleData.t_sub);
                //Debug.Log("Fan tachometer: " + teleData.v_fan);
                //Debug.Log("EVA time: " + teleData.t_eva);
                //Debug.Log("Oxygen pressure: " + teleData.p_o2);
                //Debug.Log("Oxygen rate: " + teleData.rate_o2);
                //Debug.Log("Battery Capacity: " + teleData.cap_battery);
                //Debug.Log("H2O gas pressure: " + teleData.p_h2o_g);
                //Debug.Log("H20 liquid pressure: " + teleData.p_h2o_l);
                //Debug.Log("Secondary Oxygen Pack pressure: " + teleData.p_sop);
                //Debug.Log("Oxygen rate for Secondary: " + teleData.rate_sop);

                //updata latest
                latestT = teleData;
                //insert into list
                tDataS.Add(teleData);
                //update deviations
                stdDev(tDataS);
                //print deviations
                //printD();
            }

        }

    }
    //find the standard deviation for a set of data
    void stdDev(List<teleObj> tDataS)
    {
        //find the mean, starting with adding all values
        float[] means = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
		
		
        for(var dSet = 0; dSet < tDataS.Count; dSet++) // loop through different data sets
        {
            //for(var setEl = 0; setEl < 15; setEl) // loop through different data points in a set
            //{
            //    if (setEl == 1 || setEl == 2 || setEl == 3 || setEl == 7) means[setEl] += 1;
            //    else means[setEl] += tDataS[dSet].
            //}
            //means[0] += tDataS[dSet].i_suit_p;
            means[0] += 1;					//string value
			means[1] += tDataS[dSet].heart_bpm;
            means[2] += tDataS[dSet].p_sub;
            means[3] += tDataS[dSet].t_sub;
            means[4] += tDataS[dSet].v_fan;
            //means[7] += 1;                  //string value
            means[5] += tDataS[dSet].p_o2;
            means[6] += tDataS[dSet].rate_o2;
            means[7] += tDataS[dSet].cap_battery;
            means[8] += tDataS[dSet].p_h2o_g;
            means[9] += tDataS[dSet].p_h2o_l;
            means[10] += tDataS[dSet].p_sop;
            means[11] += tDataS[dSet].rate_sop;
			means[12] += 1;                  //string value
            means[13] += 1;                  //string value
            means[14] += 1;                  //string value
        }
        //finalize mean values
        for(int i = 0; i < dataPC; i++)
        {
            means[i] /= tDataS.Count;
        }

        //Debug.Log("Mean: " + means[0]);

        //the sum of each difference with the mean
        float[] variances = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        //find the variances, first by summing up the differences squared
        for (var dSet = 0; dSet < tDataS.Count; dSet++) // loop through different data sets
        {
            //variances[0] += ((tDataS[dSet].i_suit_p - means[0])* (tDataS[dSet].i_suit_p - means[0]));
			variances[0] += 1;                  //string value
			variances[1] += ((tDataS[dSet].heart_bpm - means[1]) * (tDataS[dSet].heart_bpm - means[1]));                  //string value
            variances[2] += ((tDataS[dSet].p_sub - means[2]) * (tDataS[dSet].p_sub - means[2]));
            variances[3] += ((tDataS[dSet].t_sub - means[3]) * (tDataS[dSet].t_sub - means[3]));
            variances[4] += ((tDataS[dSet].v_fan - means[4]) * (tDataS[dSet].v_fan - means[4]));
            variances[5] += 1;                  //string value
            variances[5] += ((tDataS[dSet].p_o2 - means[5]) * (tDataS[dSet].p_o2 - means[5]));
            variances[6] += ((tDataS[dSet].rate_o2 - means[6]) * (tDataS[dSet].rate_o2 - means[6]));
            variances[7] += ((tDataS[dSet].cap_battery - means[7]) * (tDataS[dSet].cap_battery - means[7]));
            variances[8] += ((tDataS[dSet].p_h2o_g - means[8]) * (tDataS[dSet].p_h2o_g - means[8]));
            variances[9] += ((tDataS[dSet].p_h2o_l - means[9]) * (tDataS[dSet].p_h2o_l - means[9]));
            variances[10] += ((tDataS[dSet].p_sop - means[10]) * (tDataS[dSet].p_sop - means[10]));
            variances[11] += ((tDataS[dSet].rate_sop - means[11]) * (tDataS[dSet].rate_sop - means[11]));
			variances[12] += 1;                  //string value
            variances[13] += 1;                  //string value
            variances[14] += 1;                  //string value
        }
        //finalize variances by dividing by N (where N is number of data sets)
        for(int i = 0; i < dataPC; i++)
        {
            variances[i] /= tDataS.Count;
        }

        //take the square root of values held in variances to get the final deviations
        for(int i = 0; i < dataPC; i++)
        {
            deviats[i] = Mathf.Sqrt(variances[i]);
        }
    }

    void printD()   //print deviations
    {
        Debug.Log("STANDARD DEVIATIONS");
        Debug.Log("--------------------");
        //Debug.Log("Internal Suit Pressure: " + deviats[0]);
		Debug.Log("Heart Beats Per Minute:"+ deviats[1]);
        Debug.Log("Sub pressure: " + deviats[2]);
        Debug.Log("Sub temperature: " + deviats[3]);
        Debug.Log("Fan tachometer: " + deviats[4]);
        //Debug.Log("EVA time: " + deviats[0]);
        Debug.Log("Oxygen pressure: " + deviats[5]);
        Debug.Log("Oxygen rate: " + deviats[6]);
        Debug.Log("Battery Capacity: " + deviats[7]);
        Debug.Log("H2O gas pressure: " + deviats[8]);
        Debug.Log("H20 liquid pressure: " + deviats[9]);
        Debug.Log("Secondary Oxygen Pack pressure: " + deviats[10]);
        Debug.Log("Oxygen rate for Secondary: " + deviats[11]);
		//Debug.Log("Time Life Battery: " + deviats[12]);
        //Debug.Log("Time Life Oxygen: " + deviats[13]);
        //Debug.Log("Time Life Water: " + deviats[14]);
    }
}
