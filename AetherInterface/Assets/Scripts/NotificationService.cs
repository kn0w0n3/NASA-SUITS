using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct NotifcationData {
    public string header;
    public string msg;
    public int lvl;
}

public class NotificationService : MonoBehaviour {

    private static NotificationService _Instance;
    public static NotificationService Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<NotificationService>();
            }
            return _Instance;
        }
    }

    Queue<NotifcationData> queue;
    GameObject current = null;

    // Use this for initialization
    void Start () {
        queue = new Queue<NotifcationData>();
        NotificationService._Instance = this;
    }
	
	// Update is called once per frame
	void Update () {
        if (current == null && queue.Count > 0) {
            NotifcationData data = queue.Dequeue();
            GameObject noti = (GameObject)Instantiate(Resources.Load("Notification"));

            noti.transform.Find("Canvas/Notification Pane/Header").GetComponent<Text>().text = data.header;
            noti.transform.Find("Canvas/Notification Pane/Body").GetComponent<Text>().text = data.msg;

            Quaternion rot = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.Cross(Camera.main.transform.forward, Vector3.right));
            noti.GetComponent<Notification>().direction = rot * Vector3.up;
            noti.transform.position = Camera.main.transform.position + rot * new Vector3(0.0f, -0.25f, 0.25f);
            noti.transform.rotation = rot * Quaternion.Euler(30.0f, 0.0f, 0.0f);
            current = noti;
        }
        
    }

    public static void Issue(string header, string msg, int lvl)
    {
        NotifcationData data = new NotifcationData();
        data.header = header;
        data.msg = msg;
        data.lvl = lvl;

        Instance.queue.Enqueue(data);
    }
}
