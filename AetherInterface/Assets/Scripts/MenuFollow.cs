using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFollow : MonoBehaviour {

    public Canvas canvas;
    float hFOV;
    float realWidth;

	// Use this for initialization
	void Start () {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        realWidth = rect.localScale.x * rect.sizeDelta.x;
        float realHeight = rect.localScale.y * rect.sizeDelta.y;

        //Debug.Log("Real Width:" + realWidth);

        // Calculate the preffered distance
        float radAngle = Camera.main.fieldOfView * Mathf.Deg2Rad;
        float radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * Camera.main.aspect);
        hFOV = Mathf.Rad2Deg * radHFOV;

        //Debug.Log("Horizontal FOV: " + hFOV);



        float desDist = 1.2f * realWidth / (2 * Mathf.Tan(radHFOV / 2.0f));
        Quaternion desired = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        transform.position = Camera.main.transform.position + desDist * (desired * Vector3.forward);
        transform.rotation = desired;
    }
	
	// Update is called once per frame
	void Update () {
        float radAngle = Camera.main.fieldOfView * Mathf.Deg2Rad;
        float radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * Camera.main.aspect);
        hFOV = Mathf.Rad2Deg * radHFOV;


        float curDist = Vector3.Distance(transform.position, Camera.main.transform.position);

        float tolerance = 0.01f;
        float desDist = 1.2f * realWidth / (2 * Mathf.Tan(radHFOV / 2.0f));//0.5f;

        float move = desDist - curDist;
        float dVel = move * 1.0f / 1.0f;

        if (move < tolerance && move > -tolerance) dVel = 0.0f;
        //if (dVel * Time.deltaTime > move) dVel = move;

        float newDist = curDist + dVel * Time.deltaTime;
        //newDist = desDist;

        // Rotation
        Quaternion current = Quaternion.LookRotation((transform.position - Camera.main.transform.position).normalized, transform.up);
        Quaternion desired = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        float angle = Quaternion.Angle(current, desired);


        float t = Time.deltaTime * (angle - hFOV / 2.0f);
        if (t < 0) t = 0;
        Quaternion newRot = Quaternion.RotateTowards(current, desired, t); //Quaternion.Slerp(current, desired, Time.deltaTime);
        
        //if (angle < hFOV / 2.0f) newRot = current;


        transform.position = Camera.main.transform.position + newDist * (newRot * Vector3.forward);
        transform.rotation = newRot;
    }
}
