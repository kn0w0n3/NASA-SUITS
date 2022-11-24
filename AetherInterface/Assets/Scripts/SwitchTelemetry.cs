/*using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SwitchData
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

    public void printSet() 
    {
        Debug.Log("Battery AMP High: " + this.battery_amp_high);
        Debug.Log("Battery VDC Low: " + this.battery_vdc_low);
        Debug.Log("Spacesuit Pressure Low: " + this.suit_press_low);
        Debug.Log("Second oxygen pack active? " + this.sop_on);
        Debug.Log("Spacesuit pressure emergency: " + this.suit_press_emerg);
        Debug.Log("Spacesuit pressure high: " + this.suit_press_high);
        Debug.Log("O2 high use " + this.o2_high_use);
        Debug.Log("SOP pressure low " + this.sop_press_low);
        Debug.Log("Fan failure? " + this.fan_fail);
        Debug.Log("No vent flow " + this.no_vent_flow);
        Debug.Log("CO2 high " + this.co2_high);
        Debug.Log("Vehicle power present " + this.vehicle_power_present);
        Debug.Log("H2O off? " + this.h2o_off);
        Debug.Log("O2 off? " + this.o2_off);
    }

}

public class SwitchTelemetry : MonoBehaviour
{
    // Use this for initialization
    void Start() 
    {
#if UNITY_EDITOR
        //json test file path
        string path = Application.streamingAssetsPath + "/switchTest.json";

        //read in file and store json text 
        StreamReader jsonString = new StreamReader(new FileStream(path, FileMode.Open));

        //deserialize jsonString and store key-values in new instance of SwitchTelemtry 
        SwitchData switchData = new SwitchData();
        
        switchData = JsonUtility.FromJson<SwitchData>(jsonString.ReadToEnd());

        switchData.printSet();

        jsonString.Dispose();
#endif

    }

    // Update is called once per frame
    void Update() 
    {

    }
}
*/