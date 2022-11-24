using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideScript : MonoBehaviour {

    public GameObject menu;
    bool active;

	// Use this for initialization
	void Start () {
        active = true;
	}

    // Update is called once per frame
    void Update()
    {
        if (HololensInput.Instance.current.pressed && !HololensInput.Instance.previous.pressed)
        {
            active = !active;
            menu.SetActive(active);
        }
    }
}
