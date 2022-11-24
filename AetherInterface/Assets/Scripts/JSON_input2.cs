using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//needed for JSON
using UnityEngine.Networking;


//blueprint for object parsed from JSON
//  this is used with NASA's given format
[System.Serializable]
public class switObj    //telemetry data
{
    public string create_date;	//date/time that packet was created
    //public bool batterAmpHigh;  //battery amp high          ***key name not yet given
    //public bool batteryVdcLow;  //battery voltage dc low    ***key name not yet given
    //public bool suitPressLow;   //suit pressure low         ***key name not yet given
    public bool sop_on;         //Secondary Oxygen pack active
    public bool sspe;           //Spacesuit Pressure Emergency
    //public bool suitPressHigh;  //Spacesuit Pressure High   ***key name not yet given
    //public bool o2HighUse;      //Oxygen use high           ***key name not yet given
    //public bool sopPressLow;    //Secondary Oxygen Pack Low ***key name not yet given
    public bool fan_error;      //Fan failure
    public bool vent_error;     //no vent flow
    //public bool co2High;        //Co2 levels high           ***key name not yet given
    public bool vehicle_power;  //vehicle power present
    public bool h2o_off;        //h20 system offline
    public bool o2_off;         //o2 system offline

    public static switObj crFmJsn(string jsonString)
    {
        string temp = jsonString.Replace('[',' ').Replace(']',' ');
        return JsonUtility.FromJson<switObj>(temp);
    }

}


public class JSON_input2 : MonoBehaviour
{
    float timer = 0;
    switObj latestS;
    List<switObj> sDataS;   //switch data set

    // Use this for initialization
    void Start()
    {
        sDataS = new List<switObj>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 10)
        {
            StartCoroutine(GetJson());
            timer -= 10;
        }
    }

    //
    IEnumerator GetJson()
    {

        
        //Follows NASA format, only works if Branden's server is up and running
        using (UnityWebRequest www = UnityWebRequest.Get("https://telemetry-stream-bhitt.c9users.io/switch"))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //string jsonData = www.text;
                //parse json
                //Debug.Log(www.downloadHandler.text);
                //
                switObj switData = switObj.crFmJsn(www.downloadHandler.text); //create from JSON

                //print out properties   (this is dated, needs to be changed since values have changed)
                //Debug.Log("Battery Amp High: " + switData.batterAmpHigh);
                //Debug.Log("Battery Voltage Low: " + switData.batteryVdcLow);
                //Debug.Log("Suit Press Low: " + switData.suitPressLow);
                //Debug.Log("Secondary Oxygen Pack Active: " + switData.sop_on);
                //Debug.Log("Spacesuit Pressure Emergency: " + switData.sspe);
                //Debug.Log("Spacesuit Pressure High: " + switData.suitPressHigh);
                //Debug.Log("Oxygen Use High: " + switData.o2HighUse);
                //Debug.Log("Secondary Oxygen Pack Low: " + switData.sopPressLow);
                //Debug.Log("Fan Failure: " + switData.fan_error);
                //Debug.Log("Vent_Error (No Vent Flow): " + switData.vent_error);
                //Debug.Log("Co2 High: " + switData.co2High);
                //Debug.Log("Vehicle Power Present: " + switData.vehicle_power);
                //Debug.Log("H2o System Offline: " + switData.h2o_off);
                //Debug.Log("o2 System Offline: " + switData.o2_off);

                //updata latest
                latestS = switData;
                //insert into list
                sDataS.Add(switData);
            }
        }
    }
}
