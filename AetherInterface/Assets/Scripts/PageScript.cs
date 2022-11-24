using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using UnityEngine.Events;

public class PageScript : MonoBehaviour {

    //public GameObject cam;
    public GameObject menu;
    public Button leftButton;
    public Button rightButton;
    //public GameObject canvas;
    public float time = 0f;

    private ValueAnim<Vector3, Vector3Animable> anim;

	// Use this for initialization
	void Start () {
        menu = GameObject.Find("MenuObject");
    }
	
	// Update is called once per frame
	void Update () {
		if (anim != null)
        {
            menu.transform.position = anim.Update(Time.deltaTime);
            if (!anim.isPlaying())
            {
                leftButton.interactable = true;
                rightButton.interactable = true;
            }
        }
	}

    public void MoveRight() {
        Vector3 end = menu.transform.position;
        //end.z += -0.5f;
        //end.x += Random.Range(-0.5f, 0.5f);
        end.x += -0.725f;
        anim = new ValueAnim<Vector3, Vector3Animable>(menu.transform.position, end, 0.3f, Easing.easeInQuad);
        leftButton.interactable = false;
        rightButton.interactable = false;
    }

    public void MoveLeft() {
        
        Vector3 end = menu.transform.position;
        //end.z += -0.5f;
        //end.x += Random.Range(-0.5f, 0.5f);
        end.x += 0.725f;
        anim = new ValueAnim<Vector3, Vector3Animable>(menu.transform.position, end, 0.3f, Easing.easeInQuad);
        leftButton.interactable = false;
        rightButton.interactable = false;
    }
}
