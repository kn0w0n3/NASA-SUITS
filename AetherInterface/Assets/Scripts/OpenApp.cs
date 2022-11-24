using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;

public class OpenApp : MonoBehaviour, IPointerClickHandler
{
    public GameObject MainMenu;
    public GameObject Application;
    public void OnPointerClick(PointerEventData eventData)
    {
        MainMenu.SetActive(false);
        Application.SetActive(true);
    }
}