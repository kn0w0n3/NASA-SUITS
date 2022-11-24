using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Onscreen_keyboard : MonoBehaviour {

    /// <summary>
    /// Requirements:
    /// The user performs an action causing app code to call TouchScreenKeyboard
    /// The app is responsible for pausing app state before calling TouchScreenKeyboard
    /// The app may terminate before ever switching back to the volumetric view
    /// Unity switches to a 2D XAML view which is auto-placed in the world
    /// The user enters text using the system keyboard and submits or cancels
    /// Unity switches back to the volumetric view
    /// The app is responsible for resuming app state when the TouchScreenKeyboard is done
    /// Submitted text is available in the TouchScreenKeyboard
    /// 
    /// The HoloLens system keyboard is only available to Unity applications that are exported with the "UWP Build Type" set to "XAML". There are tradeoffs you make 
    /// when you choose "XAML" as the "UWP Build Type" over "D3D". If you aren't comfortable with those tradeoffs, you may wish to explore an alternative input solution to the system keyboard.
    /// 
    /// </summary>

    /*Additional notes:
     * Information from https://developer.microsoft.com/en-us/windows/mixed-reality/keyboard_input_in_unity
     * 
     * 
    */

    UnityEngine.TouchScreenKeyboard keyboard;
    public static string keyboardText = "";
    public int Keyboard_views;//0-6

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (TouchScreenKeyboard.visible == false && keyboard != null)//additionally TouchScreenKeyboard.area
        {
            if (keyboard.done == true)
            {
                keyboardText = keyboard.text;
                keyboard = null;
            }
        }
    }
    public void OpenKeyboard(int view)
    {
        Keyboard_views = view;
        //2D XAML view
        switch (Keyboard_views)//open keyboard in correct mode
        {
            case 0:
                Debug.Log("No keyboard method set");
                break;
            case 1:
            // Single-line textbox
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
                break;
            case 2:
            // Single-line textbox with title
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false, "Single-line title");
                break;
            case 3:
            // Multi-line textbox
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, true, false, false);
                break;
            case 4:
            // Multi-line textbox with title
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, true, false, false, "Multi-line Title");
                break;
            case 5:
            // Single-line password box
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, false);
                break;
            case 6:
            // Single-line password box with title
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, false, "Secure Single-line Title");
                break;
            default:
                Debug.Log("Incorrect Keyboard mode val=" + Keyboard_views);
                break;
        }
    }
    public bool IsKeyboardOpen()
    {
        if(TouchScreenKeyboard.visible == false && keyboard != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public string GetKeyboardText()//probably not useful
    {
        return keyboardText;
    }
    public void LoadKeyboardText(string val)//probably not useful
    {
        keyboard.text=val;
    }
}
