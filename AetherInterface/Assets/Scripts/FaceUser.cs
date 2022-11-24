using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceUser : MonoBehaviour {

    public GameObject taskboard;
    GameObject dir;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (taskboard != null) {
            transform.position = taskboard.transform.position;
        }

        Vector3 direction = (transform.position - Camera.main.transform.position);
        direction.y = 0;
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        /*direction = Camera.main.transform.forward;
        direction.y = 0;
        direction.Normalize();
        dir.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);*/
	}
}
