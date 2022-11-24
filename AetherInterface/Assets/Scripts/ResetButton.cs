using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetButton : MonoBehaviour, IPointerClickHandler
{
    public GameObject Timer;
    public void OnPointerClick(PointerEventData eventData)  //Resets Timer and Stops the Invoking
    {
        Timer.GetComponent<Timer>().CancelInvoke();
    }
}
