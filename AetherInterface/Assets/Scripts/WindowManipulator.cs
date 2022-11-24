using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowManipulator : MonoBehaviour {
    GameObject menu;
    bool isResize = false;
    public bool SecondaryObject;

    GameObject[] corners;
    int isDragging = -1; // If the menu panel is getting resized

    // Inverse aspect ratio of the menu
    const float MENU_ASPECT = 9.0f / 16.0f;

    // Use this for initialization
    void Start () {
        if (!SecondaryObject)
        {
            menu = GameObject.Find("MenuObject");
            spawnCorners();
        }
    }

    // Update is called once per frame
    void Update() {

        if (isResize) {
            Vector2 an = HololensInput.GetJoystick();

            if (HololensInput.GetPressed(ButtonType.BButton)) {
                Debug.Log("Test");
                resizeUI();
            } else if (an.y > 0.1 || an.y < -0.1) {

                float width = menu.transform.localScale.x + an.y * Time.deltaTime;

                // Clamp to min/max size
                if (width < 0.2F) {
                    width = 0.2F;
                } else if (width > 1.0f) {
                    width = 1.0f;
                }

                menu.transform.localScale = new Vector3(width, width, 1.0f); // New scale of the menu
                moveCorners();
            }
        }

        //Debug.Log(HololensInput.Instance.current.pressed + " " + HololensInput.Instance.previous.pressed);
        if (HololensInput.Instance.current.pressed && !HololensInput.Instance.previous.pressed) {
            // Check if the user has selected a corner box
            //Debug.Log("click");
            for (int i = 0; i < corners.Length; i++) {
                if (HololensInput.Instance.current.hitInfo.gameObject == corners[i])
                {
                    //Debug.Log("selected cube " + i);
                    isDragging = i;
                    break;
                }
            }
        } else if (isDragging >= 0 && HololensInput.Instance.current.pressed) {

            // Cast the users selection to the plane
            Ray ray = HololensInput.GazeRay();
            float dist = GetRayDistance(ray);
            if (dist > 0) {
                //Debug.Log("Distance > 0");
                Vector3 hit = ray.origin + dist * ray.direction; // Position the ray hit

                Vector3 lnDir = new Vector3(0,0,0); // Direction of the cross line
                if (isDragging == 0 || isDragging == 1) {
                    lnDir = Vector3.Normalize(menu.transform.rotation * new Vector3(1, 9 / 16.0f, 0));
                } else {
                    lnDir = Vector3.Normalize(menu.transform.rotation * new Vector3(1, -9 / 16.0f, 0));
                }
                //Debug.Log(lnDir);

                // Find the point on the cross line closest to hit
                Vector3 corner = corners[isDragging].transform.position;
                Vector3 closest = lnDir * Vector3.Dot(hit - corner, lnDir) + corner;
                corners[isDragging].transform.position = closest;
                // Find the new width

                closest = Quaternion.Inverse(menu.transform.rotation) * (closest - menu.transform.position);
                //Debug.Log("Dist = " + closest);
                float width;
                if (isDragging == 0 || isDragging == 3) {
                    Vector3 cornerP = Quaternion.Inverse(menu.transform.rotation) * (corners[1].transform.position - menu.transform.position);

                    width = Mathf.Abs(closest.x - cornerP.x);
                } else {
                    Vector3 cornerP = Quaternion.Inverse(menu.transform.rotation) * (corners[0].transform.position - menu.transform.position);

                    width = Mathf.Abs(closest.x - cornerP.x);
                }

                // Clamp to min/max size
                if (width < 0.2F) {
                    width = 0.2F;
                } else if (width > 1.0f) {
                    width = 1.0f;
                }

                Vector3 scale = new Vector3(width, width, 1.0f); // New scale of the menu
                Vector3 scaleDelta = scale - menu.transform.localScale;
                scaleDelta.y *= 9.0f / 16.0f;

                // Set the scale
                menu.transform.localScale = scale;
                
                // Move the menu to compensate for the change in scale
                if (isDragging == 1 || isDragging == 3) {
                    scaleDelta.y = -scaleDelta.y;
                }
                if (isDragging == 1 || isDragging == 2) {
                    scaleDelta.x = -scaleDelta.x;
                }
                Vector3 p = menu.transform.position + menu.transform.rotation * (scaleDelta / 2.0f);
                menu.transform.position = p;

                // Finally set the corners in the right position
                moveCorners();
            } else {
                isDragging = -1;
            }
        }
        if (isDragging >= 0 && !HololensInput.Instance.current.pressed) isDragging = -1;
    }

    float GetRayDistance(Ray ray) {
        float denom = Vector3.Dot(menu.transform.forward, ray.direction);
        if (denom > 1e-6) {
            Vector3 vec = menu.transform.position - ray.origin;
            float d = Vector3.Dot(vec, menu.transform.forward) / denom;
            if (d >= 0.0f) return d;
        }
        return -1;
    }

    // Move the corner boxes to the correct positions
    void moveCorners() {
        float xx = menu.transform.localScale.x;
        xx = xx * 0.035F;
        Vector3 newScle = new Vector3(xx, xx, xx);
        Vector3 scl = corners[0].transform.localScale;
        //Debug.Log(scl);

        Vector3 t;

        // Upper-Right
        t = menu.transform.position + menu.transform.rotation * Vector3.Scale(new Vector3(1 / 2.0F, 9 / 32.0F, 0.0F), menu.transform.localScale);
        corners[0].transform.position = t;
        corners[0].transform.rotation = menu.transform.rotation;
        corners[0].transform.localScale = newScle;

        // Lower-Left
        t = menu.transform.position + menu.transform.rotation * Vector3.Scale(new Vector3(-1 / 2.0F, -9 / 32.0F, 0.0F), menu.transform.localScale);
        corners[1].transform.position = t;
        corners[1].transform.rotation = menu.transform.rotation;
        corners[1].transform.localScale = newScle;

        // Upper-Left
        t = menu.transform.position + menu.transform.rotation * Vector3.Scale(new Vector3(-1 / 2.0F, 9 / 32.0F, 0.0F), menu.transform.localScale);
        corners[2].transform.position = t;
        corners[2].transform.rotation = menu.transform.rotation;
        corners[2].transform.localScale = newScle;

        // Lower-Right
        t = menu.transform.position + menu.transform.rotation * Vector3.Scale(new Vector3(1 / 2.0F, -9 / 32.0F, 0.0F), menu.transform.localScale);
        corners[3].transform.position = t;
        corners[3].transform.rotation = menu.transform.rotation;
        corners[3].transform.localScale = newScle;
    }

    // Enter/Exit resize mode
    public void resizeUI() {
        if (!isResize) {
            isResize = true;
            moveCorners();
            cornersActive(true);
        } else {
            isResize = false;
            cornersActive(false);
        }
    }

    // spawn the corner boxes
    void spawnCorners() {
        float s = 0.035F; // Scale of the corners

        corners = new GameObject[4];

        //Create each corner box
        for(int i = 0; i < 4; i++) {
            GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.name = "CornerBox";
            c.transform.localScale = new Vector3(s,s,s);
            c.transform.rotation = menu.transform.rotation;
            c.SetActive(false); // Set inactive for now

            // TODO: Styling

            // Add the box to the array
            corners[i] = c;
        }
    }

    // Set the active state of the corner boxes
    void cornersActive(bool active) {
        for (int i = 0; i < 4; i++) {
            corners[i].SetActive(active);
        }
    }

    //set to secondary object
    public void setNewObject(GameObject ob)
    {
        menu = ob;
        spawnCorners();
    }
}
