using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using UnityEngine.UI.ProceduralImage;

#if !UNITY_EDITOR
using GloveController;
#endif

public enum ButtonType {
    Tap, AButton, BButton
}

public struct RayHit {
    public float distance;
    public Vector3 point;
    public Vector3 normal;
    public GameObject gameObject;
}

public struct InputState {
    public bool detected;
    public bool tapping;
    public bool manipulating;
    public bool navigating;

    public bool pressed;

    public bool btnA;
    public bool btnB;
    public Vector2 analog;

    public bool hit;
    public RayHit hitInfo;
}

public class HololensInput : MonoBehaviour {

    private static HololensInput _Instance;
    public static HololensInput Instance
    {
        get
        {
            if (_Instance == null) {
                _Instance = FindObjectOfType<HololensInput>();
            }
            return _Instance;
        }
    }

    public InputState current;
    public InputState previous;

    private AetherMenu currentMenu;

    private GameObject oldSelected;
    private GameObject border;

    private GestureRecognizer ManipulationRecognizer;
    private GestureRecognizer NavigationRecognizer;

    private float deadzone = 0.2f;

    public Text text;

#if !UNITY_EDITOR
    private GloveController.GloveController gc;
#endif

    // Use this for initialization
    void Start() {
        current.detected = false;
        current.pressed = false;
        previous.detected = false;
        previous.pressed = false;

        ManipulationRecognizer = new GestureRecognizer();

        // Add the ManipulationTranslate GestureSetting to the ManipulationRecognizer's RecognizableGestures.
        ManipulationRecognizer.SetRecognizableGestures(
            GestureSettings.ManipulationTranslate);

        // Register for the Manipulation events on the ManipulationRecognizer.
        ManipulationRecognizer.ManipulationStarted += manipStarted;
        ManipulationRecognizer.ManipulationUpdated += manipUpdated;
        ManipulationRecognizer.ManipulationCompleted += manipComplete;
        ManipulationRecognizer.ManipulationCanceled += manipEnd;

        ManipulationRecognizer.StartCapturingGestures();

        InteractionManager.InteractionSourceDetected += handDetected;
        InteractionManager.InteractionSourceLost += handLost;
        InteractionManager.InteractionSourcePressed += handPressed;
        InteractionManager.InteractionSourceReleased += handReleased;

#if !UNITY_EDITOR
        gc = new GloveController.GloveController();
        gc.OnConnected += GC_OnConnected;
        gc.OnDisconnected += GC_OnDisconnected;
        gc.OnDeviceFound += GC_OnDeviceFound;
        gc.StartConnection();
#endif
    }

#region Hololens Events
    private void manipStarted(ManipulationStartedEventArgs obj) {
        current.manipulating = true;
    }

    private void manipUpdated(ManipulationUpdatedEventArgs obj) {
    }

    private void manipComplete(ManipulationCompletedEventArgs obj) {
        current.manipulating = false;
    }

    private void manipEnd(ManipulationCanceledEventArgs obj) {
        current.manipulating = false;
    }

    private void handDetected(InteractionSourceDetectedEventArgs obj) {
        current.detected = true;
    }

    private void handLost(InteractionSourceLostEventArgs obj) {
        current.detected = false;
    }

    private void handPressed(InteractionSourcePressedEventArgs obj) {
        current.pressed = true;
    }

    private void handReleased(InteractionSourceReleasedEventArgs obj) {
        current.pressed = false;
    }
    #endregion

    #region Glove Events
#if !UNITY_EDITOR
    void GC_OnConnected() {
        Debug.Log("Connected");
        NotificationService.Issue("Controller Conection", "The controller has successfully connected", 0);
    }

    void GC_OnDisconnected() {
        Debug.Log("Disconnected");
        NotificationService.Issue("Controller Conection", "The controller has disconnected", 0);
    }


    void GC_OnDeviceFound() {
        Debug.Log("Device Found");
        UnityEngine.WSA.Application.InvokeOnUIThread(() => {
            gc.Connect();
        }, true);
    }
#endif
    #endregion

    #region Input Functions

    public static bool GetPressed(ButtonType btnType) {
        switch (btnType) {
            case ButtonType.Tap: return Instance.current.pressed && !Instance.previous.pressed;
            case ButtonType.AButton: return Instance.current.btnA && !Instance.previous.btnA;
            case ButtonType.BButton: return Instance.current.btnB && !Instance.previous.btnB;
            default: return false;
        }
    }

    public static bool ButtonDown(ButtonType btnType) {
        switch (btnType) {
            case ButtonType.Tap: return Instance.current.pressed && !Instance.previous.pressed;
            case ButtonType.AButton: return Instance.current.btnA && !Instance.previous.btnA;
            case ButtonType.BButton: return Instance.current.btnB && !Instance.previous.btnB;
            default: return false;
        }
    }

    public static bool ButtonUp(ButtonType btnType)
    {
        switch (btnType)
        {
            case ButtonType.Tap: return !Instance.current.pressed && Instance.previous.pressed;
            case ButtonType.AButton: return !Instance.current.btnA && Instance.previous.btnA;
            case ButtonType.BButton: return !Instance.current.btnB && Instance.previous.btnB;
            default: return false;
        }
    }

    public static Vector2 GetJoystick() {
        return Instance.current.analog;
    }

    public static Ray GazeRay() {
#if UNITY_EDITOR
        return Camera.main.ScreenPointToRay(Input.mousePosition);
#else
        return new Ray(Camera.main.transform.position, Camera.main.transform.forward);
#endif
    }

#endregion

#region Utilities Functions

    Vector2 GazePosition() {
#if UNITY_EDITOR
        return Input.mousePosition; // use the position from controller as start of raycast instead of mousePosition.
#else
        return new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
#endif
    }

    RaycastResult UICast(Vector2 gazePos) {
        // Raycast on UI elements
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = gazePos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // Find the closest raycast result
        RaycastResult closest = new RaycastResult();
        closest.distance = -1;
        closest.depth = -1;
        foreach (RaycastResult res in results) {
            if (closest.distance >= 0 && res.distance == closest.distance) {
                if (res.depth > closest.depth) {
                    closest = res;
                }
            }
            if (closest.distance < 0 || res.distance < closest.distance) {
                closest = res;
            }
        }

        return closest;
    }

    #endregion

    #region Controller

#if !UNITY_EDITOR
    async void UpdateController() {
        if (gc.isConnected) {
            InputPacket? result = await gc.GetInputAsync();

            if (result.HasValue) {
                InputPacket pack = result.Value;
                current.btnA = pack.button1;
                current.btnB = pack.button2;

                /*
                y center: 544.53
                y max: 807.07
                y min: 286.35
                
                x center: 465.14
                x max: 739.90
                x min: 183.90
                */
                float y = FilterAnalog(ToFloatAn(pack.y), ToFloatAn(286.35f), ToFloatAn(807.07f));
                float x = FilterAnalog(ToFloatAn(pack.x), ToFloatAn(183.90f), ToFloatAn(739.90f));

                Vector3 analog = new Vector2(x, -y);
                //text.text = "X: " + analog.x + " Y: " + analog.y + " Mag: " + analog.magnitude;

                if (analog.magnitude < deadzone)
                    current.analog = Vector2.zero;
                else current.analog = analog.normalized * ((analog.magnitude - deadzone) / (1.0f - deadzone));
            }
        }
    }

    public static void Pulse(ushort strength, ushort duration) {
        if (Instance.gc.isConnected) {
            Instance.gc.Pulse(strength, duration);
        }
    }

#else
    public static void Pulse(ushort strength, ushort duration) {
        Handheld.Vibrate();
    }

#endif

    void EmulateController() {
        current.pressed = Input.GetMouseButton(0);
        current.detected = (Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width &&
                            Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height);

        current.btnA = Input.GetButton("Fire1");
        current.btnB = Input.GetButton("Fire2");
        current.analog = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    #endregion

    float ToFloatAn(float value)
    {
        return value / 512.0f - 1.0f;
    }

    float FilterAnalog(float value, float min, float max)
    {
        float result = 2 * (value - min) / (max - min) - 1.0f;
        if (result > 1.0f) result = 1.0f;
        if (result < -1.0f) result = -1.0f;

        return result;
    }

    void UpdateSelection() {
        if (EventSystem.current.currentSelectedGameObject != oldSelected && EventSystem.current.currentSelectedGameObject != null) {
            oldSelected = EventSystem.current.currentSelectedGameObject;

            RectTransform rect = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
            AetherButton btn = EventSystem.current.currentSelectedGameObject.GetComponent<AetherButton>();

            if (rect != null && currentMenu != null) {
                if (border == null) {
                    // Instance new border
                    border = (GameObject)Instantiate(Resources.Load("BorderHighlight"));
                }
                border.SetActive(true);

                UniformModifier selMod = EventSystem.current.currentSelectedGameObject.GetComponent<UniformModifier>();
                UniformModifier bordMod = border.GetComponent<UniformModifier>();

                RectTransform bordRect = border.GetComponent<RectTransform>();
                //Debug.Log(rect.gameObject.name);
                border.transform.SetParent(rect);

                Vector2 size = rect.sizeDelta + new Vector2(20, 20);
                bordRect.sizeDelta = size;
                bordRect.position = rect.position;
                bordRect.rotation = rect.rotation;
                bordRect.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                if (selMod) {
                    bordMod.Radius = selMod.Radius / rect.sizeDelta.y * size.y;
                } else {
                    bordMod.Radius = 0.0f;
                }
            }

            if (btn != null && !btn.highlight)
            {
                border.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    void Update() {
#else
    async void Update() {
#endif
        previous = current;

#if UNITY_EDITOR
        EmulateController();
#else
        UpdateController();
#endif

        // Raycast on world objects
        Ray ray = GazeRay();
        RaycastHit hitInfo;
        int layer = (1 << 0) | (1 << 5); // Layers 0 (Default) and 5 (UI)
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layer);

        Vector2 gazePos = GazePosition();
        RaycastResult closest = UICast(gazePos);

        if (hit && (closest.distance < 0.0f || hitInfo.distance < closest.distance)) {
            current.hit = true;
            current.hitInfo = new RayHit();
            current.hitInfo.distance = hitInfo.distance;
            current.hitInfo.point = hitInfo.point;
            current.hitInfo.normal = hitInfo.normal;
            current.hitInfo.gameObject = hitInfo.transform.gameObject;
        } else if (closest.distance >= 0) {
            //if (closest.gameObject.name != results[0].gameObject.name)
            //    Debug.Log(closest.gameObject.name + " " + results[0].gameObject.name);
            Ray UIRay = Camera.main.ScreenPointToRay(gazePos);
            Vector3 UIHit;
            float distance;

            if (RayRectIntersect(closest.gameObject.GetComponent<RectTransform>(), UIRay, out UIHit, out distance)) {
                current.hit = true;
                current.hitInfo = new RayHit();
                current.hitInfo.gameObject = closest.gameObject;
                current.hitInfo.point = UIHit;
                current.hitInfo.distance = distance;
                current.hitInfo.normal = closest.gameObject.transform.forward;

                // Find if there is a menu in the parents
                AetherMenu[] menus = closest.gameObject.GetComponentsInParent<AetherMenu>();
                if (menus.Length > 0 && menus[0] != currentMenu) {
                    if (currentMenu != null) {
                        currentMenu.focused = false;
                        currentMenu.onDeselected();
                    }
                    currentMenu = menus[0];
                    currentMenu.focused = true;
                    currentMenu.onSelected();
                    EventSystem.current.SetSelectedGameObject(currentMenu.defaultObject);

                    //ProceduralImage pi = currentMenu.mainPanel.GetComponent<ProceduralImage>();
                }


            } else {
                current.hit = false;
            }
        } else {
            current.hit = false;
        }

        UpdateSelection();

        if (Input.GetKeyDown(KeyCode.F))
        {

            NotificationService.Issue("Controller Conection", "The controller has disconnected", 0);
        }

#if !UNITY_EDITOR
        if (gc.isConnected) {

            
        }
#endif
    }

    public bool RayRectIntersect(RectTransform rectTransform, Ray ray, out Vector3 worldPos, out float distance) {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Plane plane = new Plane(corners[0], corners[1], corners[2]);
    
        float enter;
        if (!plane.Raycast(ray, out enter))
        {
            worldPos = Vector3.zero;
            distance = 0.0f;
            return false;
        }
    
        Vector3 intersection = ray.GetPoint(enter);
    
        Vector3 BottomEdge = corners[3] - corners[0];
        Vector3 LeftEdge = corners[1] - corners[0];
        float BottomDot = Vector3.Dot(intersection - corners[0], BottomEdge);
        float LeftDot = Vector3.Dot(intersection - corners[0], LeftEdge);
        if (BottomDot < BottomEdge.sqrMagnitude && // Can use sqrMag because BottomEdge is not normalized
            LeftDot < LeftEdge.sqrMagnitude &&
                BottomDot >= 0 &&
                LeftDot >= 0)
        {
            worldPos = corners[0] + LeftDot * LeftEdge / LeftEdge.sqrMagnitude + BottomDot * BottomEdge / BottomEdge.sqrMagnitude;


            /*UnityEngine.UI.ProceduralImage.ProceduralImage pi = rectTransform.GetComponent<UnityEngine.UI.ProceduralImage.ProceduralImage>();

            if (pi) {
                Sprite sp = pi.sprite;
                Vector3 rel = worldPos - rectTransform.position;
                rel = Quaternion.Inverse(rectTransform.rotation) * rel;

                int width = sp.texture.width;
                int height = sp.texture.height;
                Canvas can = rectTransform.GetComponentsInParent<Canvas>()[0];
                float x = Mathf.Round(width * rel.x / rectTransform.sizeDelta.x * 2.0f);
                float y = Mathf.Round(height * rel.y / rectTransform.sizeDelta.y * 2.0f);

                //x = Mathf.Round(rel.x * can.referencePixelsPerUnit);
                //x = Mathf.Round(rel.y * can.referencePixelsPerUnit);
                x = rel.x;
                y = rel.y;
                Debug.Log("Transparent: " + x + ", " + y);
                if (sp.texture.GetPixel((int)x, (int)y).a > 0.999f)
                {
                    //Debug.Log("Transparent: " + x + ", " + y);
                    worldPos = Vector3.zero;
                    return false;
                }
            }*/
            distance = Vector3.Distance(ray.origin, worldPos);
            return true;
        } else {
            worldPos = Vector3.zero;
            distance = 0.0f;
            return false;
        }
    }
}
