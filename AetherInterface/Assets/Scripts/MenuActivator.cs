using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivator : MonoBehaviour {

    public GameObject menu;
    public GameObject progress;
    float timer;

    const float STARE_TIME = 1.0f;

	// Use this for initialization
	void Start () {
        timer = 0.0f;
	}

    void Awake()
    {
        timer = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
		if (HololensInput.Instance.current.hit && HololensInput.Instance.current.hitInfo.gameObject == transform.gameObject)
        {
            progress.GetComponent<MeshRenderer>().material.color = Color.red;
            timer += Time.deltaTime;

            Vector3 scale = progress.transform.localScale;
            scale.x = timer / STARE_TIME;
            progress.transform.localScale = scale;

            Vector3 pos = transform.position;
            pos = pos + transform.rotation * new Vector3((scale.x - 1) * 0.0225f, 0.0f, 0.0f);
            progress.transform.position = pos;

            if (timer >= STARE_TIME)
            {
                Quaternion rot = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
                menu.transform.position = transform.position + rot * new Vector3(0, 0, 0.3f);
                menu.transform.rotation = rot;
                transform.gameObject.SetActive(false);
            }

        } else
        {
            timer = 0.0f;
            progress.GetComponent<MeshRenderer>().material.color = Color.white;


            Vector3 scale = progress.transform.localScale;
            scale.x = timer / STARE_TIME;
            progress.transform.localScale = scale;

            Vector3 pos = transform.position;
            pos = pos + transform.rotation * new Vector3((scale.x - 1) * 0.0225f, 0.0f, 0.0f);
            progress.transform.position = pos;
        }
	}

    void OnEnable()
    {
        timer = 0.0f;
    }
}
