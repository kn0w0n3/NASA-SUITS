using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GazeCursor : MonoBehaviour
{

    MeshRenderer meshRenderer;

    // Use this for initialization
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Awake()
    {
    }

    // Update is called once per frame
    void Update() {
        if (HololensInput.Instance.current.hit) {
            transform.position = HololensInput.Instance.current.hitInfo.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, HololensInput.Instance.current.hitInfo.normal);
            meshRenderer.enabled = true;
        } else {

#if UNITY_EDITOR
            //meshRenderer.enabled = false;
            meshRenderer.enabled = true;
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.75f;
            transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
#else
            meshRenderer.enabled = true;
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.75f;
            transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
#endif
        }

        if (!HololensInput.Instance.current.detected) {
            GetComponent<MeshRenderer>().material.color = Color.white;
        } else if (HololensInput.Instance.current.pressed) {
            GetComponent<MeshRenderer>().material.color = Color.green;
        } else {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }


    }
}
