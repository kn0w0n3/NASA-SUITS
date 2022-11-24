using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSphere : MonoBehaviour {

    public GameObject menu;
    private ValueAnim<Vector3, Vector3Animable> anim;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (anim != null)
        {
            transform.localScale = anim.Update(Time.deltaTime);
        }
        if(HololensInput.Instance.current.hit && HololensInput.Instance.current.hitInfo.gameObject == transform.gameObject)
        {

            Debug.Log("hit");
            if(HololensInput.Instance.previous.hitInfo.gameObject != transform.gameObject || !HololensInput.Instance.previous.hit)
            {
                Vector3 newScale = new Vector3(.1F, .1F, .1F);
                anim = new ValueAnim<Vector3, Vector3Animable>(transform.localScale, newScale, 0.05f, Easing.easeInQuad);
            }
            if(HololensInput.Instance.current.pressed && !HololensInput.Instance.previous.pressed)
            {
                Debug.Log("clicked");
                menu.GetComponent<MenuManager>().Maximize();
                Destroy(transform.gameObject);
            }

        }
        else
        {
            if (HololensInput.Instance.previous.hitInfo.gameObject == transform.gameObject && HololensInput.Instance.previous.hit)
            {
                float xx = 0.0834F;
                Vector3 newScle = new Vector3(xx, xx, xx);
                anim = new ValueAnim<Vector3, Vector3Animable>(transform.localScale, newScle, 0.05f, Easing.easeInQuad);
            }
        }
	}
}
