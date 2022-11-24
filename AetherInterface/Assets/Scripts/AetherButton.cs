using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AetherButton : MonoBehaviour, IPointerClickHandler, ISelectHandler {

	public bool useSounds = true;
	public bool highlight = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSelect(BaseEventData eventData)
    {
        HololensInput.Pulse(1023, 200);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
		GameObject.Find("Audio Manager/ClickSound").GetComponent<AudioSource>().Play();
    }
}
