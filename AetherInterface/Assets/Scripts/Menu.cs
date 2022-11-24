using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    //Prefabs
    public GameObject timer;
    public GameObject timerApp;
    public GameObject telemertyApp;
    public GameObject allTelemetryApp;
    public GameObject menuButtons;
    public GameObject micButton;

    //Reference
    public GameObject drops;
    public GameObject inner;

    GameObject TelemTemp;
    GameObject TelemAllTemp;
    List<string> apps;
    List<System.Action> actions;

    bool isDrops = false;
    bool isTimer = false;
    bool isTelem = false;
    bool isAllTelem = false;
    // Use this for initialization
    void Start () {
        apps = new List<string>();
        actions = new List<System.Action>();

        apps.Add("Taskboard");
        actions.Add(GameObject.Find("TaskboardOverlay").GetComponent<TaskboardManager>().ChooseTaskboard);
        apps.Add("Timer");
        actions.Add(createTimer);
        apps.Add("Show Telemetry");
        actions.Add(createTelemtry);
    }
	
	// Update is called once per frame
	void Update () {

    }
    public void moveMenu()
    {
        GameObject prompt = Instantiate(Resources.Load("PlacingPrompt")) as GameObject;
        prompt.GetComponent<FaceUser>().taskboard = gameObject;
        prompt.GetComponent<PlaceScript>().obj = gameObject;

    }
    public void showAllTelemetry()
    {
        if (!isAllTelem)
        {
            deleteTelem();
            TelemAllTemp = (GameObject)Instantiate(allTelemetryApp);
            TelemAllTemp.name = "Telemetry-All";
            Vector3 t = this.transform.position;
            //Debug.Log(TelemAllTemp.transform.GetComponent<RectTransform>().rect.width);
            t.y += 0.17f;
            //this.transform.GetComponent<RectTransform>().rect.width + 50;
            TelemAllTemp.transform.position = t;
            isAllTelem = true;
        }
        else
        {
            deleteTelemAll();
        }
    }
    void deleteTelemAll() {
        if (TelemAllTemp != null)
        { 
            if (isAllTelem)
            {
                Destroy(TelemAllTemp);
                isAllTelem = false;
            }
        }
    }
    void deleteTelem()
    {
        if (TelemTemp != null)
        {
            if (isTelem)
            {
                Destroy(TelemTemp);
                isTelem = false;
            }
        }
    }
    public void createTelemtry()
    {
        if (!isTelem)
        {
            deleteTelemAll();
            TelemTemp = (GameObject)Instantiate(telemertyApp);
            TelemTemp.name = "Telemetry-Top";
            Vector3 t = this.transform.position;
            //t.x -= 0.7f;
            t.y += 0.17f;
            TelemTemp.transform.position = t;
            isTelem = true;
        }
        else
        {
            deleteTelem();
        }
    }

    public void createTimer()
    {
        if (!isTimer)
        {
            GameObject timerr = (GameObject)Instantiate(timer);
            timerr.name = "menuTimer";
            timerr.transform.SetParent(inner.transform, false);
            isTimer = true;
        }
        else
        {
            //Delete
            isTimer = false;
        }
    }
    void deleteDrops() { 
        foreach (Transform child in drops.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void expandTimer()
    {
        inner.transform.Find("menuTimer").gameObject.SetActive(false);
        GameObject app = (GameObject)Instantiate(timerApp);
        app.name = "TimerApp";
        Vector3 temp = this.transform.position;
        temp.x -= 0.7f;
        app.transform.position = temp;
    }
    public void closeTimer()
    {
        if (GameObject.Find("TimerApp"))
        {
            Destroy(GameObject.Find("TimerApp"));
        }
    }
    public void createDrops()
    {
        //Debug.Log("creating drops");
        if (isDrops)
        {
            //Delete Drops
            deleteDrops();
        }
        else
        {
            float y = this.transform.position.y - drops.GetComponent<RectTransform>().rect.height - 25;
            for (int i = 0; i < apps.Count; i++)
            {
                GameObject butt = (GameObject)Instantiate(menuButtons);
                butt.name = "menubutton";
                Vector3 temp = drops.transform.position;
                temp.y = y;
                butt.transform.position = temp;
                butt.transform.SetParent(drops.transform, false);
                butt.GetComponentInChildren<Text>().text = apps[i];
                butt.GetComponentInChildren<Button>().onClick.AddListener(actions[i].Invoke);
                butt.GetComponentInChildren<Button>().onClick.AddListener(deleteDrops);
                y += -butt.GetComponent<RectTransform>().rect.height;
            }
        }
        isDrops = !isDrops;
    }
    public void test()
    {
        Debug.Log("test");
    }
}
