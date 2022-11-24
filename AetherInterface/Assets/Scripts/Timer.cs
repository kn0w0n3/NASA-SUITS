using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Timer : MonoBehaviour
{
    public Button ResetButton;
    public Button StartButton;
    public Button StopButton;
    public Text Milliseconds;
    public Text Seconds;
    public Text Seconds2;
    public Text Minutes;
    public Text Minutes2;
    public Text Hours;
    public Text Hours2;
    float millise = 0;

    public GameObject houru;
    public GameObject hourd;
    public GameObject minu;
    public GameObject mind;
    public GameObject secu;
    public GameObject secd;

    void Start()
    {
        Button ResButton = ResetButton.GetComponent<Button>();
        ResButton.onClick.AddListener(ResetTimer);
        Button Startbtn = StartButton.GetComponent<Button>();
        Startbtn.onClick.AddListener(StartTimer);
        Button Stopbtn = StopButton.GetComponent<Button>();
        Stopbtn.onClick.AddListener(StopTimer);
    }
    public void callExpand()
    {
        GameObject.Find("menu").GetComponent<Menu>().expandTimer();
    }
    void hideStuff()
    {
        houru.SetActive(!houru.activeSelf);
        hourd.SetActive(!hourd.activeSelf);
        minu.SetActive(!minu.activeSelf);
        mind.SetActive(!mind.activeSelf);
        secu.SetActive(!secu.activeSelf);
        secd.SetActive(!secd.activeSelf);
}
    void StopTimer()
    {
        hideStuff();
        StopButton.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(true);
        CancelInvoke();

    }
    public void incSec()
    {
        int x = Convert.ToInt32(Seconds.text);
        int y = Convert.ToInt32(Seconds2.text);
        if (x >= 9 && y >= 5) return;
        else
        {
            x++;
            if(x>9)
            {
                y++;
                x = 0;
            }
            Seconds.text = Convert.ToString(x);
            Seconds2.text = Convert.ToString(y);
        }
    }
    public void decSec()
    {
        int x = Convert.ToInt32(Seconds.text);
        int y = Convert.ToInt32(Seconds2.text);
        if (x <= 0 && y <=0) return;
        else
        {
            
            if (x > 0)
            {
                x--;
                if (y > 0)
                {
                    y--;
                    x = 9;
                }
            }
            Seconds.text = Convert.ToString(x);
            Seconds2.text = Convert.ToString(y);
        }
    }
    public void incMin()
    {
        int x = Convert.ToInt32(Minutes.text);
        int y = Convert.ToInt32(Minutes2.text);
        if (x >= 9 && y >= 5) return;
        else
        {
            x++;
            if (x > 9)
            {
                y++;
                x = 0;
            }
            Minutes.text = Convert.ToString(x);
            Minutes2.text = Convert.ToString(y);
        }
    }
    public void decMin()
    {
        int x = Convert.ToInt32(Minutes.text);
        int y = Convert.ToInt32(Minutes2.text);
        if (x <= 0 && y <= 0) return;
        else
        {

            if (x > 0)
            {
                x--;
                if (y > 0)
                {
                    y--;
                    x = 9;
                }
            }
            Minutes.text = Convert.ToString(x);
            Minutes2.text = Convert.ToString(y);
        }
    }
    public void incHour()
    {
        int x = Convert.ToInt32(Hours.text);
        int y = Convert.ToInt32(Hours2.text);
        if (x >= 9 && y >= 9) return;
        else
        {
            x++;
            if (x > 9)
            {
                y++;
                x = 0;
            }
            Hours.text = Convert.ToString(x);
            Hours2.text = Convert.ToString(y);
        }
    }
    public void decHour()
    {
        int x = Convert.ToInt32(Hours.text);
        int y = Convert.ToInt32(Hours2.text);
        if (x <= 0 && y <= 0) return;
        else
        {

            if (x > 0)
            {
                x--;
                if (y > 0)
                {
                    y--;
                    x = 9;
                }
            }
            Hours.text = Convert.ToString(x);
            Hours2.text = Convert.ToString(y);
        }
    }
    void ResetTimer()
    {
        if(!StartButton.gameObject.activeSelf)
            hideStuff();
        float sec = float.Parse(Seconds.text);
        int min = int.Parse(Minutes.text);
        int hrs = int.Parse(Hours.text);
        millise = 0;
        sec = 0;
        min = 0;
        hrs = 0;
        CancelInvoke();
        Milliseconds.text = "" + millise.ToString("00");
        Seconds.text = "" + sec.ToString("0");
        Seconds2.text = "" + sec.ToString("0");
        Minutes.text = "" + min.ToString("0");
        Minutes2.text = "" + min.ToString("0");
        Hours.text = "" + hrs.ToString("0");
        Hours2.text = "" + hrs.ToString("0");
    }
    void StartTimer()
    {
        hideStuff();
        StopButton.gameObject.SetActive(true);
        StartButton.gameObject.SetActive(false);
        float sec = float.Parse(Seconds.text);
        float sec2 = float.Parse(Seconds2.text);
        int min = int.Parse(Minutes.text);
        int min2 = int.Parse(Minutes2.text);
        int hrs = int.Parse(Hours.text);
        int hrs2 = int.Parse(Hours2.text);
        if (hrs == 0 && min == 0 && sec==0&&hrs2==0&&min2==0&&sec2==0)
        {
            CancelInvoke();
            Milliseconds.text = "" + millise.ToString("00");
            Seconds.text = "" + sec.ToString("0");
            Seconds2.text = "" + sec.ToString("0");
            Minutes.text = "" + min.ToString("0");
            Minutes2.text = "" + min.ToString("0");
            Hours.text = "" + hrs.ToString("0");
            Hours2.text = "" + hrs.ToString("0");
        }
        else
        {
            InvokeRepeating("CountDown", .01f, .01f);
        }
    }
    public void CountDown()
    {
        float sec = float.Parse(Seconds.text);
        float sec2 = float.Parse(Seconds2.text);
        int min = int.Parse(Minutes.text);
        int min2 = int.Parse(Minutes2.text);
        int hrs = int.Parse(Hours.text);
        int hrs2 = int.Parse(Hours2.text);
        millise--;
        //Decrements minute/hours/seconds
        if (millise <= 0)
        {
            millise = 100;
            sec--;
        }
        if (sec <= -.9)                           //Decrements minute
        {
            sec = 9;
            sec2--;
        }
        if(sec2<=-.9)
        {
            sec2 = 5;
            min--;
        }
        if (min <=-.9)                              //Decrements Hours
        {
            min = 9;
            min2--;
        }
        if(min2<=-.9)
        {
            min2 = 5;
            hrs--;
        }
        if(hrs<0)
        {
            hrs = 9;
            hrs2--;
        }
        //Outputs the time to the user
        Milliseconds.text = "" + millise.ToString("00");
        Seconds.text = "" + sec.ToString("0");
        Seconds2.text = "" + sec2.ToString("0");
        Minutes.text = "" + min.ToString("0");
        Minutes2.text = "" + min2.ToString("0");
        Hours.text = "" + hrs.ToString("0");
        Hours2.text = "" + hrs2.ToString("0");
        //if (min < 10)
        //{
        //    Minutes.text = "" + min.ToString("D2");
        //}
        //else
        //{
        //    Minutes.text = "" + min.ToString();
        //}
        //if (hrs < 10)
        //{
        //    Hours.text = "" + hrs.ToString("D2");
        //}
        //else
        //{
        //    Hours.text = "" + hrs.ToString();
        //}
        if (hrs == 0 && min == 0 && sec == 0 && hrs2 == 0 && min2 == 0 && sec2 == 0&& millise <= 1)
        {
            millise = 0;
            sec = 0;
            sec2 = 0;
            min = 0;
            min2 = 0;
            hrs = 0;
            hrs2 = 0;
            Milliseconds.text = "" + millise.ToString("00");
            Seconds.text = "" + sec.ToString("0");
            Seconds2.text = "" + sec2.ToString("0");
            Minutes.text = "" + min.ToString("0");
            Minutes2.text = "" + min2.ToString("0");
            Hours.text = "" + hrs.ToString("0");
            Hours2.text = "" + hrs2.ToString("0");
            CancelInvoke();
        }

    }
}
