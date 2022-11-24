using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ReturnMenu : MonoBehaviour, IPointerClickHandler
{
    public GameObject MainMenu;
    public void OnPointerClick(PointerEventData eventData)
    {
        MainMenu.SetActive(true);
        GameObject[] Apps = GameObject.FindGameObjectsWithTag("Application");
        Debug.Log(Apps.Length);
        foreach (GameObject go in Apps)
        {
            go.SetActive(false);
        }
    }
 }