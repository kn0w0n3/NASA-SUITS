using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    private ValueAnim<Vector3, Vector3Animable> anim;
    Vector3 lastScale;

    bool active;

    // Use this for initialization
    void Start () {
        active = true;
	}
	
	// Update is called once per frame
	void Update () {
	    if(anim != null)
        {
            if (anim.isPlaying())
            {
                transform.localScale = anim.Update(Time.deltaTime);
                if(!anim.isPlaying())
                {
                    if(active) {
                        float xx = 0.0834F;
                        Vector3 newScle = new Vector3(xx, xx, xx);
                        GameObject c = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        c.transform.position = transform.position;
                        c.transform.localScale = newScle;
                        c.transform.rotation = transform.rotation;
                        MenuSphere sph = c.AddComponent<MenuSphere>();
                        sph.menu = transform.gameObject;
                    }
                    active = !active;
                    transform.gameObject.SetActive(active);
                }
            }
        }
        
	}
    public void Minimize()
    {
        //                                            Begining position,End position, duration, animation style
        anim = new ValueAnim<Vector3, Vector3Animable>(transform.localScale, Vector3.zero, 0.15827f, Easing.easeInQuad);
        lastScale = transform.localScale;

    }
    public void Maximize()
    {
        Vector3 end = lastScale;
        if (active) end = transform.localScale;
        anim = new ValueAnim<Vector3, Vector3Animable>(Vector3.zero, end, 0.15827f, Easing.easeInQuad);
        transform.gameObject.SetActive(true);
    }

}
