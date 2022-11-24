using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {
    public Vector3 direction;
    public float speed = 0.5f;
    public float duration = 0.25f;
    public float hold = 2.0f;

    float timer;

	// Use this for initialization
	void Start () {
        timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer <= duration) {
            transform.position = transform.position + direction * speed * Time.deltaTime;
        } else if (timer > duration + hold) {
            Destroy(gameObject);
        }

        timer += Time.deltaTime;
	}
}
