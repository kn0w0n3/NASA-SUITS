using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PlaceScript : MonoBehaviour {

    public delegate void PlacingCompleteEvent();
    public event PlacingCompleteEvent OnPlacingComplete;

    public GameObject obj; // The object to place in 3D
    public float gestSensitivity = 1.0f;
    public float anSensitivity = 1.0f;
    GestureRecognizer manipRecog;
    Vector3 lastPos;
    bool manipulating = false;
    int direction = 1;

    // Use this for initialization
    void Start () {
        manipRecog = new GestureRecognizer();

        // Add the ManipulationTranslate GestureSetting to the ManipulationRecognizer's RecognizableGestures.
        manipRecog.SetRecognizableGestures(GestureSettings.ManipulationTranslate);

        // Register for the Manipulation events on the ManipulationRecognizer.
        manipRecog.ManipulationStarted += manipStarted;
        manipRecog.ManipulationUpdated += manipUpdated;
        manipRecog.ManipulationCompleted += manipComplete;
        manipRecog.ManipulationCanceled += manipEnd;

        manipRecog.StartCapturingGestures();
    }

#region Manipulation
    private void manipStarted(ManipulationStartedEventArgs data) {
        lastPos = Vector3.zero;
        manipulating = true;
    }

    private void manipUpdated(ManipulationUpdatedEventArgs data) {

        obj.transform.position = obj.transform.position + gestSensitivity * (data.cumulativeDelta - lastPos);
        transform.position = obj.transform.position;
        lastPos = data.cumulativeDelta;
    }

    private void manipComplete(ManipulationCompletedEventArgs data) {
        manipulating = false;
    }

    private void manipEnd(ManipulationCanceledEventArgs data) {
        manipulating = false;
    }
#endregion

    // Update is called once per frame
    void Update () {
        Quaternion movePlane = Quaternion.LookRotation(Vector3.Cross(Camera.main.transform.right, Vector3.up), Vector3.up);

        if (HololensInput.ButtonDown(ButtonType.BButton) || Input.GetKeyDown(KeyCode.R)) {
            Completed();
        } else if (HololensInput.ButtonDown(ButtonType.AButton)) {
            direction++;
            transform.Find("PlaneDirection").gameObject.SetActive(false);
            transform.Find("RotDirection").gameObject.SetActive(false);
            transform.Find("UpDownDirection").gameObject.SetActive(false);


            if (direction == 1) {
                transform.Find("PlaneDirection").gameObject.SetActive(true);;
            } else if (direction == 2) {
                transform.Find("RotDirection").gameObject.SetActive(true);
            } else if (direction > 2) {
                transform.Find("UpDownDirection").gameObject.SetActive(true);
                direction = 0;
            }
        } else {
            Vector2 an = HololensInput.GetJoystick();
            Vector3 move;
            if (direction == 0) {
                // Vertical movement
                move = new Vector3(0.0f, an.y, 0.0f);
            } else if (direction == 1) {
                // Horizontal plane
                move = movePlane * new Vector3(an.x, 0.0f, an.y);
            } else {
                // Rotation
                move = Vector3.zero;
                obj.transform.rotation = obj.transform.rotation * Quaternion.Euler(0.0f, an.x * 45.0f * Time.deltaTime, 0.0f);
            }

            transform.position += move * anSensitivity * Time.deltaTime;
        }

        obj.transform.position = transform.position;
        transform.Find("PlaneDirection").rotation = movePlane;
	}

    public void Completed() {
        manipRecog.StopCapturingGestures();
        if (gameObject) {
            Destroy(gameObject);

            if (OnPlacingComplete != null)
                OnPlacingComplete();
        }
            
    }
}
