using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskboardManager : MonoBehaviour {
    Layout layout = new Layout();           // Layout Class holds the information of the taskboard 
    string fileName = "taskboard.json";     // File name of the taskboard layout [ need to add option to load more ]
    string path;                            // Directory of where the JSON files are located

    List<GameObject> objs = new List<GameObject>();
    Step currentStep;

    List<string> files;
    List<string> taskboardNames;
    public bool loaded = false;

    // For taskboard placing

    Color modColor;

    private static TaskboardManager _Instance;
    public static TaskboardManager Instance {
        get {
            if (_Instance == null) {
                _Instance = FindObjectOfType<TaskboardManager>();
            }
            return _Instance;
        }
    }

    void Start() {
        transform.position = Camera.main.transform.position + new Vector3(0.0f, -0.2f, 0.3f);

        Instance.path = Application.streamingAssetsPath + "/TaskboardLayouts/";
        files = new List<string>();
        taskboardNames = new List<string>();
        LoadFileNames("/TaskboardLayouts/");
        //ChooseTaskboard(); 
        ProcedureManager.OnStepChanged += OnStepChanged;

        modColor = new Color(1.0f, 1.0f, 1.0f, 1.0f / 4.0f);
    }

    // Update is called once per frame
    void Update () {
    }

    private void OnStepChanged(Step step) {
        // Remove the prompts from the previous step
        if (currentStep != null)
        {
            foreach (Prompt prompt in currentStep.prompts)
            {
                switch (prompt.type)
                {
                    case "arrow":
                        {
                            Transform module1 = transform.Find(prompt.moduleID);
                            Transform module2 = transform.Find(prompt.misc);

                            if (module1 != null && module2 != null)
                            {
                                module1.gameObject.SetActive(false);
                                module2.gameObject.SetActive(false);
                            }
                            Transform arrow = transform.Find("Arrow " + prompt.moduleID + " to " + prompt.misc);
                            if (arrow != null)
                            {
                                Destroy(arrow.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find arrow for deletion");
                            }
                            break;
                        }
                    case "push":
                        {
                            Transform arrow = transform.Find("Push " + prompt.moduleID);

                            Transform module = transform.Find(prompt.moduleID);
                            if (module != null)
                            {
                                module.gameObject.SetActive(false);
                            }

                            if (arrow != null)
                            {
                                Destroy(arrow.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find push arrow for deletion");
                            }
                            break;
                        }
                    case "pull":
                        {
                            Transform arrow = transform.Find("Pull " + prompt.moduleID);
                            
                            Transform module = transform.Find(prompt.moduleID);
                            if (module != null)
                            {
                                module.gameObject.SetActive(false);
                            }

                            if (arrow != null)
                            {
                                Destroy(arrow.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find pull arrow for deletion");
                            }
                            break;
                        }
                    case "highlight":
                        {
                            Transform module = transform.Find(prompt.moduleID);
                            if (module != null)
                            {
                                module.gameObject.GetComponent<Renderer>().material.color = modColor;
                                module.gameObject.SetActive(false);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find module by ID for unhighlighting");
                            }
                            break;
                        }
                    case "circle":
                        {
                            Transform circle = transform.Find("Circle " + prompt.moduleID);
                            Transform module = transform.Find(prompt.moduleID);

                            if (module != null)
                            {
                                module.gameObject.SetActive(false);
                            }

                            if (circle != null)
                            {
                                Destroy(circle.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + step.number + ": Couldn't find circle for deletion");
                            }
                            break;
                        }
                }
            }
        }

        // Create the prompts for the current step
        foreach (Prompt prompt in step.prompts)
        {
            switch (prompt.type)
            {
                case "arrow":
                    {
                        Transform module1 = transform.Find(prompt.moduleID);
                        Transform module2 = transform.Find(prompt.misc);

                        if (module1 != null && module2 != null)
                        {
                            module1.gameObject.SetActive(true);
                            module2.gameObject.SetActive(true);
                            GameObject arrowObj = new GameObject();
                            arrowObj.name = "Arrow " + prompt.moduleID + " to " + prompt.misc;
                            arrowObj.transform.SetParent(transform);
                            Arrow arrow = arrowObj.AddComponent<Arrow>();
                            arrow.beg = module1.position;
                            arrow.end = module2.position;
                            arrow.transform.position = arrow.transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }

                case "push":
                    {
                        Transform module = transform.Find(prompt.moduleID);

                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            GameObject arrowObj = new GameObject();
                            arrowObj.name = "Push " + prompt.moduleID;
                            arrowObj.transform.SetParent(transform);
                            arrowObj.transform.position = module.position;
                            PushArrow arrow = arrowObj.AddComponent<PushArrow>();
                            arrow.end = 0.01f;
                            arrow.beg = 0.05f;
                            arrow.transform.position = arrow.transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
                case "pull":
                    {
                        Transform module = transform.Find(prompt.moduleID);

                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            GameObject arrowObj = new GameObject();
                            arrowObj.name = "Pull " + prompt.moduleID;
                            arrowObj.transform.SetParent(transform);
                            arrowObj.transform.position = module.position;
                            PushArrow arrow = arrowObj.AddComponent<PushArrow>();
                            arrow.end = 0.05f;
                            arrow.beg = 0.01f;
                            arrow.transform.position = arrow.transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
                case "highlight":
                    {
                        Transform module = transform.Find(prompt.moduleID);
                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            Color propCol = Color.green;
                            propCol.a = 1.0f / 4.0f;
                            module.gameObject.GetComponent<Renderer>().material.color = propCol;
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
                case "circle":
                    {
                        Transform module = transform.Find(prompt.moduleID);

                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            float dirMod = 1.0f;
                            if (prompt.misc == "clockwise") dirMod = 1.0f;
                            if (prompt.misc == "counterclockwise") dirMod = -1.0f;

                            GameObject circleObj = new GameObject();
                            circleObj.name = "Circle " + prompt.moduleID;
                            circleObj.transform.SetParent(transform);
                            circleObj.AddComponent<Circle>().speed = dirMod * 45.0f;

                            Mesh mesh = Resources.Load("Circle", typeof(Mesh)) as Mesh;
                            Material mat = Resources.Load("PromptMat", typeof(Material)) as Material;
                            circleObj.AddComponent<MeshRenderer>().material = mat;
                            circleObj.AddComponent<MeshFilter>().mesh = mesh;


                            // Find the smallest dimension of the module
                            float smallestDim = module.localScale.x;
                            if (module.localScale.z < smallestDim) smallestDim = module.localScale.z;


                            circleObj.transform.localScale = new Vector3(dirMod * smallestDim, smallestDim, smallestDim);
                            circleObj.transform.position = module.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
            }
        }
        currentStep = step;
    }


    private void BoxMode(GameObject obj) {
        LineRenderer lr = obj.GetComponent<LineRenderer>();
        lr.enabled = true;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    private void SolidMode(GameObject obj) {
        LineRenderer lr = obj.GetComponent<LineRenderer>();
        lr.enabled = false;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        mr.enabled = true;
    }

    void LoadFileNames(string dir) {
        string location = Application.streamingAssetsPath;
        try {
            string temp = location + dir;
            foreach (string file in System.IO.Directory.GetFiles(temp))
            {
                string label = file.Replace(temp, "");
                if (!label.Contains(".meta"))
                {
                    string contents = System.IO.File.ReadAllText(temp + label);
                    TaskNames taskname = JsonUtility.FromJson<TaskNames>(contents);
                    //Debug.Log(procedureNames.title);
                    taskboardNames.Add(taskname.name);
                    files.Add(label);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: JSON input. " + ex.Message);
        }
    }

    void LoadTaskboard(int x) {
        try
        {
            //Debug.Log(path + files[x]);
            string temp = path + files[x];
            if (System.IO.File.Exists(temp))
            {
                string contents = System.IO.File.ReadAllText(temp);
                Instance.layout = JsonUtility.FromJson<Layout>(contents);
                //Debug.Log(layout.name);
                //foreach(Modules m in layout.modules)
                //Debug.Log(m.id + " " + m.position.x + " " + m.position.y + " " + m.rotation + " " + m.size.x + " " + m.type);
                CreateTaskboard();
            }
            else
            {
                Debug.Log("Error: Unable to read " + fileName + " file, at " + temp);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: Taskboard JSON input. " + ex.Message);
        }
    }

    public void ChooseTaskboard() {
        foreach (Modules m in layout.modules)
        {
            Transform trans = transform.Find(m.id);
            if (trans) {
                SolidMode(trans.gameObject);
            }
        }

        OptionsMenu opts = OptionsMenu.Instance("Choose A Taskboard", true);
        opts.OnSelection += LoadTaskboard;
        if (taskboardNames.Count > 0) {
            for (int i = 0; i < taskboardNames.Count; i++) {
                opts.AddItem(taskboardNames[i], i);
            }
            //opts.ChangeListHeight(taskboardNames.Count);
        } else {
            Debug.Log("Error: No taskboard layouts loaded");
        }
    }

    void CreateTaskboard() {
        Vector3 topLeft = new Vector3(-layout.width / 2.0f, 0, layout.length / 2.0f);
        GameObject cube;
        Material mat = Resources.Load("ModuleMat", typeof(Material)) as Material;

        foreach (Modules m in layout.modules)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.name = m.id;
            cube.transform.localScale = new Vector3((float)m.size.x, 0.001F, (float)m.size.y);
            Vector3 halfScle = new Vector3((float)m.size.x / 2, 0, -(float)m.size.y / 2);
            cube.transform.localPosition = topLeft + new Vector3((float)m.position.x, .001f, -(float)m.position.y) + halfScle;

            
            cube.GetComponent<Renderer>().material = mat;

            LineRenderer lr = cube.AddComponent<LineRenderer>();
            Vector3[] pts = new Vector3[6];
            
            pts[0] = new Vector3(0.25f, 0.0f, 0.5f);
            pts[1] = new Vector3(0.5f, 0.0f, 0.5f);
            pts[2] = new Vector3(0.5f, 0.0f, -0.5f);
            pts[3] = new Vector3(-0.5f, 0.0f, -0.5f);
            pts[4] = new Vector3(-0.5f, 0.0f, 0.5f);
            pts[5] = new Vector3(0.25f, 0.0f, 0.5f);

            lr.positionCount = 6;
            lr.SetPositions(pts);
            lr.widthMultiplier = 0.005f;
            lr.enabled = false;
            lr.useWorldSpace = false;
            lr.numCornerVertices = 3;

            objs.Add(cube);
        }
        loaded = true;

        Transform panel = transform.Find("ProcedurePanel");
        panel.localPosition = new Vector3(0.0f, 0.11f, layout.length / 2.0f + 0.00f);
        moveTaskboard();
    }
    public void moveProcedure()
    {
        /*IGameObject prompt = Instantiate(Resources.Load("PlacingPrompt")) as GameObject;
        prompt.GetComponent<FaceUser>().taskboard = gameObject;
        prompt.GetComponent<PlaceScript>().obj = gameObject;*/
        moveTaskboard();

    }
    public void moveTaskboard()
    {
        GameObject prompt = Instantiate(Resources.Load("PlacingPrompt")) as GameObject;
        prompt.GetComponent<FaceUser>().taskboard = gameObject;
        prompt.GetComponent<PlaceScript>().obj = gameObject;
        prompt.GetComponent<PlaceScript>().OnPlacingComplete += TaskboardPlaced;
        prompt.name = "PlacingPrompt";
    }
    void TaskboardPlaced() {
        
        ProcedureManager.Instance.ChooseProcedure();

        foreach (Modules m in layout.modules)
        {
            Transform trans = transform.Find(m.id);
            if (trans) {
                BoxMode(trans.gameObject);
                trans.gameObject.SetActive(false);
            }
        }
    }
}

//TASKBOARD LAYOUT
[System.Serializable]
public class Layout
{
    public string name;
    public float length;
    public float width;
    public List<Modules> modules = new List<Modules>();
}

[System.Serializable]
public class Modules
{
    public string type;
    public string id;
    public Vec2 size;
    public Vec2 position;
    public int rotation;
}

[System.Serializable]
public class Vec2
{
    public double x;
    public double y;
}

[System.Serializable]
public class TaskNames
{
    public string name;
}
