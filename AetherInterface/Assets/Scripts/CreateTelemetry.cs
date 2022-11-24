using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Reflection;

public class GenerateSwitchData
{
    [SerializeField]
    private int battery_amp_high;  // > 4 amp
    [SerializeField]
    private int battery_vdc_low;   // < 15 V
    [SerializeField]
    private int suit_press_low;    // < 2 psid
    [SerializeField]
    private bool sop_on;          // true, false
    [SerializeField]
    private bool suit_press_emerg; // true, false
    [SerializeField]
    private int suit_press_high;   // > 5 psid
    [SerializeField]
    private int o2_high_use;       // > 1 psi/min
    [SerializeField]
    private int sop_press_low;     // < 700 psia
    [SerializeField]
    private bool fan_fail;        // true, false
    [SerializeField]
    private bool no_vent_flow;     // true, false
    [SerializeField]
    private int co2_high;         // > 500 ppm 
    [SerializeField]
    private bool vehicle_power_present;    // true, false
    [SerializeField]
    private bool h2o_off;         // true, false
    [SerializeField]
    private bool o2_off;          // true, false

    /* 
        the value variable in each set function is the implicit parameter,
        whose type is the same as the property its setting
     */
    public int batteryAmpHigh
    {
        get {return battery_amp_high;}
        set {battery_amp_high = value;}
    }
    public int batteryVdcLow
    {
        get {return battery_vdc_low;}
        set {battery_vdc_low = value;}
    }
    public int suitPressLow
    {
        get {return suit_press_low;}
        set {suit_press_low = value;}
    }
    public bool sopOn
    {
        get {return sop_on;}
        set {sop_on = value;}
    }
    public bool suitPressEmerg
    {
        get {return suit_press_emerg;}
        set {suit_press_emerg = value;}
    }
    public int suitPressHigh
    {
        get {return suit_press_high;}
        set {suit_press_high = value;}
    }
    public int o2HighUse
    {
        get {return o2_high_use;}
        set {o2_high_use = value;}
    }
    public int sopPressLow
    {
        get {return sop_press_low;}
        set {sop_press_low = value;}
    }
    public bool fanFail
    {   
        get {return fan_fail;}
        set {fan_fail = value;}
    }
    public bool noVentFlow
    {
        get {return no_vent_flow ;}
        set {no_vent_flow = value;}
    }
    public int co2High
    {
        get {return co2_high;}
        set {co2_high = value;}
    }
    public bool vehiclePowerPresent
    {
        get {return vehicle_power_present ;}
        set {vehicle_power_present = value;}
    }
    public bool h2oOff
    {
        get {return h2o_off;}
        set {h2o_off = value;}
    }
    public bool o2Off
    {
        get {return o2_off;}
        set {o2_off = value;}
    }
}

public class DataPoint 
{
    //private string name; 
    private int pass_min;
    private int pass_max;
    private int fail_min;
    private int fail_max;
    private object value;

    public DataPoint(string name, int pass_min, int pass_max, int fail_min, int fail_max)
    {
        //this.name = name;
        this.pass_min = pass_min;
        this.pass_max = pass_max;
        this.fail_min = fail_min;
        this.fail_max = fail_max;
    }

    public object getValue()
    {
        return this.value;
    }

    public void setValue(bool status, object dataPointType) 
    {
        //random number generator object 
        System.Random rng = new System.Random();

        if(dataPointType == typeof(int))
        {
            //Debug.Log("name: " + this.name);
            //if data point has a passing value
            if(status)
            {
                this.value = (int)rng.Next(this.pass_min, this.pass_max);
                //Debug.Log("value: " + this.value + " -> passes");
            }
            else
            {
                this.value = (int)rng.Next(this.fail_min, this.fail_max);
                //Debug.Log("value: " + this.value + " -> fails");
            }
            
        }
        if(dataPointType == typeof(bool))
        {
            //Debug.Log("name: " + this.name);
            if(status)
            {
                this.value = Convert.ToBoolean(rng.Next(this.pass_min, this.pass_max));
                //Debug.Log("value: " + this.value + " -> passes");
            }
            else
            {
                this.value = Convert.ToBoolean(rng.Next(this.fail_min, this.fail_max));
                //Debug.Log("value: " + this.value + " -> fails");
            }
        }
    }
}

public class CreateTelemetry : MonoBehaviour 
{
    void Start() 
    {
        //collection of all pass/fail states for each data point
        bool [] passFail = generateValues();

        //collection of data points with passing/failing values
        DataPoint [] dataPoints = getDataPoints();

        GenerateSwitchData switchData = new GenerateSwitchData();

        Type type = switchData.GetType();
        PropertyInfo [] props = type.GetProperties();

        if((props.Length == passFail.Length) && (props.Length == dataPoints.Length))
        {
            int i = 0;
            foreach(var prop in props)
            {
                bool status = passFail[i];
                var property_type = prop.PropertyType;

                dataPoints[i].setValue(status, property_type);
                object switchValue = dataPoints[i].getValue();
                prop.SetValue(switchData, switchValue, null);
                i++;
            }
        }

        //json test file path

#if UNITY_EDITOR
        string path = Application.streamingAssetsPath + "/switchTest.json";

        string jsonString = JsonUtility.ToJson(switchData);

        StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Open));
        writer.Write(jsonString);
        writer.Dispose();
#endif

        /*foreach(var prop in props)
        {
            Debug.Log("Name: " + prop.Name + "\nValue: " + prop.GetValue(switchData, null));
        }*/
        
        //debug
    }

    void Update() 
    {

    }

    public bool [] generateValues()
    {   
        //number of data points 
        const int numDataPoints = 14;

        //minRng represents the lowest value returned by rng
        const int minRng = 0;

        //maxRng - 1 represents the highest value returned by rng
        const int maxRng = 1000;

        //fail rate for any given variable 
        float failRate = .01f; 

        //determines cutoff for failure within data point
        //any values between 0-threshold will be considered failure 
        float threshold = failRate * maxRng;

        //random number generator object 
        System.Random rng = new System.Random();
        
        //will store the pass/fail status for each data point
        //true indicates data point is within range, false exceeds range
        bool [] results = new bool [numDataPoints];

        for(int i=0; i<numDataPoints; i++)
        {
            int randNum = rng.Next(minRng, maxRng);

            if(randNum > threshold) 
            {
                results[i] = true;
                //Debug.Log("success!");
            }  
            else 
            {
                results[i] = false;
                //Debug.Log("fail");
            }

            //Debug.Log("Fail Rate: " + failRate);
            //Debug.Log("Threshold: " + threshold);
            //Debug.Log("Random Number: " + randNum);
            //Debug.Log("\n");
        }
        return results; 
    }

    public DataPoint [] getDataPoints()
    {
        int numDataPoints = 14;

        DataPoint [] dataPoints = new DataPoint [numDataPoints];
        dataPoints[0] = new DataPoint("battery_amp_high", 0, 4, 5, 10);
        dataPoints[1] = new DataPoint("battery_vdc_low", 15, 20, 0, 14);
        dataPoints[2] = new DataPoint("suit_press_low", 2, 5, 0, 1);
        dataPoints[3] = new DataPoint("sop_on", 1, 1, 0, 0);
        dataPoints[4] = new DataPoint("suit_press_emerg", 0, 0, 1, 1);
        dataPoints[5] = new DataPoint("suit_press_high", 0, 5, 6, 10);
        dataPoints[6] = new DataPoint("o2_high_use", 0, 1, 2, 5);
        dataPoints[7] = new DataPoint("sop_press_low", 700, 1000, 0, 699);
        dataPoints[8] = new DataPoint("fain_fail", 0, 0, 1, 1);
        dataPoints[9] = new DataPoint("no_vent_flow", 0, 0, 1, 1);
        dataPoints[10] = new DataPoint("co2_high", 0, 500, 501, 1000);
        dataPoints[11] = new DataPoint("vehicle_power_present", 1, 1, 0, 0);
        dataPoints[12] = new DataPoint("h2o_off", 0, 0, 1, 1);
        dataPoints[13] = new DataPoint("o2_off", 0, 0, 1, 1);

        return dataPoints;
    }
}