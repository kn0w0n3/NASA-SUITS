using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARDebug : MonoBehaviour
{
    public Text console3d;                              //The 3d console text
    private static ARDebug _Instance;
    public static ARDebug Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<ARDebug>();
            }
            return _Instance;
        }
    }
    public void Log(string DebugText)           //Passing in Debug text then adding Debug text to console 3d
    {
        //Displays that object was pressed in the 3d console
        console3d.text = console3d.text+DebugText+"\n";
    }
}
