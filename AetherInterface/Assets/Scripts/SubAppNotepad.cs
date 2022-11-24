using System.Collections;
using System.Collections.Generic;
using System.IO;//file operations
using System.Text;//byte encoding
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SubAppNotepad : MonoBehaviour {
    //administrative setings
    private string Base_dir;//Notepad operating dir
    public string SubDir;//Directory inside Aether system
    public bool Open_New_Tester;//Assume in testing state, create new dir
    public int TestDir;//current test instance (default increment)
    //tester settings
    public int NoteIndex;//index of note in test
    private int MaxNoteIndex;//max index of last check
    public bool Open_New_Note;//Open new note, no load
    public bool Open_in_loc;//opens from location saved in notes
    public bool Use_onscreen_keyboard;//use on screen keyboard instead of diction
    public Color HeaderColor;//color of header
    //sub_objects and values
    private Vector3 deltamove;
    private Vector3 note_pos;
    public InputField Note_Input;//input with carrot //value attributed to the note
    private bool is_edit;
    private bool is_move;
    //button stats
    private bool BUTTON_new;
    private bool BUTTON_edit;
    
    //notes are saved at Base_dir+SubDir+"/notesaves_"+TestDir+"/note_" +TestDir+'-'+NoteIndex+".txt";
    //Test directory must be set prior to note reading

    void Start ()
    {
        Base_dir = Directory.GetCurrentDirectory();
        if (Open_New_Tester || Update_Max_Index(false) == -1)//create new tester dir if set or non-existant
        {
            TestDir = Update_Max_Index(false)+1;//set most recent tester by default
            New_Tester();
        }
        else//open latest tester
        {
            TestDir = Update_Max_Index(false);//set most recent tester by default
        }
        Initiate();
    }
    private void Initiate()
    {
        is_edit = false;//not currently editing note
        Directory_check();//check for file locations
        deltamove = this.transform.position;
        this.GetComponent<WindowManipulator>().setNewObject(GameObject.Find(this.gameObject.name));
        //begin note initialization
        if (Open_New_Note && File.Exists(Base_dir + SubDir + "/notesaves_" + TestDir + "/note_" + TestDir + "-0.txt"))//new note and one exists
        {
            NoteIndex = Update_Max_Index(true) + 1;
            New_Note();
        }
        else if (File.Exists(Base_dir + SubDir + "/notesaves_" + TestDir + "/note_" + TestDir + "-0.txt"))//open latest note
        {
            NoteIndex =  Update_Max_Index(true);
            Load_Note(Base_dir + SubDir + "/notesaves_" + TestDir + "/note_" + TestDir + '-' + NoteIndex + ".txt");
        }
        else
        {//open new note
            //make new directory
            NoteIndex = 0;
            New_Note();
        }
        //check button values
        Update_sidebar(3, 3);//(should have loaded last note, set to new note)3
        Update_sidebar(4, 2);//set edit button
    }


    // Update is called once per frame
    void Update ()
    {
        if (is_move)//ajust 
        {
            deltamove = this.transform.position;
        }
        if (!is_move&&deltamove!=this.transform.position)//re-center
        {
            Move_Note();
        }
        if (Use_onscreen_keyboard && is_edit)//update field
        {
            if (!this.gameObject.GetComponent<Onscreen_keyboard>().IsKeyboardOpen())//save note value
            {
                Note_Input.text = this.gameObject.GetComponent<Onscreen_keyboard>().GetKeyboardText();
                Save_Note();
            }
        }
	}
    
    private void OnDestroy()
    {
        Edit_Note(false);//close keyboard if one exists
    }

    //administrative operations
    private void New_Tester()
    {
        if (Directory.Exists(Base_dir + SubDir))//base dir to sub dir of notepad
        {
            Debug.Log("SubApp_Note: New_Tester: creating dir");
            Directory.CreateDirectory(Base_dir + SubDir + "/notesaves_" + (Update_Max_Index(false)+1).ToString());//create subdirectory
            //create config
        }
        else
        {
            Debug.Log("SubApp_Note: New_Tester: base dir does not exist " + Base_dir + SubDir);
        }
    }
    public bool Load_Tester(int tester)//load tester config manually
    {

        if (tester<=Update_Max_Index(false)&&Directory.Exists(Base_dir + SubDir + "/notesaves_" + tester + " "))//base dir to sub dir of notepad
        {
            TestDir = tester;
            Initiate();
            return true;
        }
        else
        {
            Debug.Log("SubApp_Note: Load_Tester: incorrect parameter or file does not exist:"+ Base_dir + SubDir + "/notesaves_" + tester);
            return false;
        }
    }
    
    //file operations
    private bool Directory_check()//check for directory errors, and set up if not avaliable
    {
        bool noerror = true;
        Base_dir = Directory.GetCurrentDirectory();//base directory of Aether application at runtime
        if (Directory.Exists(Base_dir))
        {

            if (!Directory.Exists(Base_dir + SubDir))
            {
                Debug.Log("SubApp_Note: Directory_check: Subdirectory does not exist: " + Base_dir + SubDir);
                Directory.CreateDirectory(Base_dir + SubDir);//try and create dir given
                if (!Directory.Exists(Base_dir + SubDir))//if failed, make default
                {
                    Debug.Log("SubApp_Note: Directory_check: unable to create subdirectory, creating default:" + Base_dir + "/Assets/SubApps/NotePad");
                    Directory.CreateDirectory(Base_dir + "/Assets");
                    Directory.CreateDirectory(Base_dir + "/Assets/SubApps");
                    Directory.CreateDirectory(Base_dir + "/Assets/SubApps/NotePad");
                    SubDir = "/Assets/SubApps/NotePad";

                }
                if (!Directory.Exists(Base_dir + SubDir))//resolution failed, abort
                {
                    noerror = false;
                }
                if (!Directory.Exists(Base_dir + SubDir + "/notesaves_0"))
                {
                    New_Tester();
                }
            }
        }
        return noerror;
    }
    private int Update_Max_Index(bool note)//return the largest index of notes saved
    {
        int index = -1;
        string loc;
        if (note)//check largest note in tester file
        {
            do
            {
                index++;
                loc = Base_dir + SubDir + "/notesaves_" + TestDir + "/note_" + TestDir + '-' + index + ".txt";
            } while (File.Exists(loc));
            //Debug.Log(index+"= largest index note");
            return (index - 1);
        }
        else//check for largest tester in 
        {
            do
            {
                index++;
                loc = Base_dir + SubDir + "/notesaves_" + index;
            } while (Directory.Exists(loc));
            //Debug.Log(index+"= largest index dir");
            return (index - 1);
        }
    }
    //Button operations
    public void BUTTON_move_resize()
    {
        is_move = !is_move;
        if (!is_move)
        {
            deltamove = note_pos;
            Move_Note();
        }
    }
    public void Previous_Note()
    {
        //Debug.Log("SubApp_Note: Prev_Note: ");
        if ((NoteIndex - 1) >= 0)//within note bounds, load prev note
        {
            NoteIndex--;//confirmed load, increment index
            if (Load_Note(Base_dir + SubDir + "/notesaves_" + TestDir + "/note_" + TestDir +'-' + (NoteIndex) + ".txt"))
            {
                Update_sidebar(3, 2);//activate next form on next button
            }
            else
            {
                NoteIndex++;
                Debug.Log("SubApp_Note: Prev_Note: could not load prev note");
            }
        }
        else if (NoteIndex!=MaxNoteIndex)
        {
            Debug.Log("SubApp_Note: Prev_Note: NoteIndex!=MaxNoteIndex");
            Debug.Log("SubApp_Note: Prev_Note: else state:index" + NoteIndex + " max " + MaxNoteIndex);
            Update_sidebar(2, 1);//disable prev button
        }
        Update_sidebar(3, 0);//activate next form on next button
    }

    public void BUTTON_new_next()//handle button states
    {
        if (BUTTON_new)
        {
            New_Note();
        }
        else
        {
            Next_Note(true);
        }
    }
    public void Next_Note(bool help)
    {
        MaxNoteIndex = Update_Max_Index(true);//check for changes in max note index
        if ((NoteIndex) < MaxNoteIndex)//within note bounds, load next note (technically off by one but increments so really index+1<=max)
        {
            NoteIndex++;//confirmed load, increment index
            if (Load_Note(Base_dir + SubDir + "/notesaves_" + TestDir + "/note_" + TestDir + '-' + (NoteIndex) + ".txt"))//Load_Note(Base_dir + SubDir + "/notesaves_" + '_' + TestDir + "/note_" + TestDir + '-' + (NoteIndex) + ".txt")
            {
                Update_sidebar(2, 0);// enable "prev" button
            }
            else
            {
                NoteIndex--;//failed, go back
                Debug.Log("SubApp_Note: Next_Note: could not load next note");
            }
        }
        else if (NoteIndex == MaxNoteIndex)//open as new note
        {
            New_Note();//at max already, create new note
        }
    }
    public void New_Note()
    {
        MaxNoteIndex = Update_Max_Index(true);
        NoteIndex = MaxNoteIndex + 1;
        HeaderColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        Note_Input.text = "";
        this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Graphic>().color = HeaderColor;//apply loaded color to header
        this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "New Note #" + (NoteIndex + 1);//apply loaded file name to header
        Note_Input.text = "";
        Update_sidebar(3, 1);//deactivate "next" button, already on new note
        if (NoteIndex > 0)//meaning this isn't the first note given
        {
            Update_sidebar(2, 0);//activate previous
        }
    }
    private void Move_Note()//smoothly transition note position
    {
        Vector3 delta= (deltamove - this.transform.position) / 100;
        this.transform.position+=delta;
        if ((delta.x < 0.0001f&& delta.x > -0.0001f) && (delta.y < 0.0001f && delta.y > -0.0001f) && (delta.z < 0.0001f && delta.z > -0.0001f))//snap at the end to avoid constant calculations
        {
            this.transform.position = deltamove;
        }
    }
    public void BUTTON_edit_save()
    {
        if (BUTTON_edit)
        {
            Edit_Note(true);
        }
        else{
            Edit_Note(false);
            Save_Note();
        }
    }
    public void Edit_Note(bool open)
    {
        is_edit = open;
        if (open)//open edit parameters
        {
            Note_Input.ActivateInputField();
            if (Use_onscreen_keyboard)
            {
                if (!this.gameObject.GetComponent<Onscreen_keyboard>().IsKeyboardOpen())
                {
                    this.gameObject.GetComponent<Onscreen_keyboard>().OpenKeyboard(3);
                    this.gameObject.GetComponent<Onscreen_keyboard>().LoadKeyboardText(Note_Input.text);//load previous value to keyboard
                }
                else
                {
                    Note_Input.text = this.gameObject.GetComponent<Onscreen_keyboard>().GetKeyboardText();
                }
            }
            Update_sidebar(4, 3);
        }
        else//close edit parameters
        {
            if (Use_onscreen_keyboard)
            {
                if (!this.gameObject.GetComponent<Onscreen_keyboard>().IsKeyboardOpen())
                {
                    Note_Input.text = this.gameObject.GetComponent<Onscreen_keyboard>().GetKeyboardText();
                }
            }
            else
            {
                Note_Input.DeactivateInputField();
            }
            //close voice
        }
        //starting onscreen keyboard;
    }
    public void Save_Note()
    {
        string loc = Base_dir + SubDir + "/notesaves_" + TestDir;
        if (Directory.Exists(loc))//check for directory
        {
            loc+="/note_" + TestDir + '-' + NoteIndex + ".txt";//add note information
            string save_data = this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Graphic>().color.ToString() + "POS" + this.transform.position.ToString() + Note_Input.text;
            FileStream File_save = new FileStream(loc, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            File_save.Write(Encoding.ASCII.GetBytes(save_data), 0, Encoding.ASCII.GetBytes(save_data).Length);
            File_save.Close();
            Debug.Log("SubApp_Note: Save_Note: Saved note to:" + loc);
            is_edit = false;
            if(Load_Note(loc))//confirm save
            {
                Debug.Log("SubApp_Note: Save_Note: Save confirmed");
                Update_sidebar(3, 0);
                BUTTON_new = true;
            }
            else//save or reload failure
            {
                Debug.Log("SubApp_Note: Save_Note: Save/reload failed");
            }
        }
        else
        {
            Debug.Log("SubApp_Note: Save_Note: failed to find directory:" + loc);
        }
        MaxNoteIndex = Update_Max_Index(true);//check for changes in max note index
        Update_sidebar(4,2);//saved note
    }

    private bool Load_Note(string notedir)//Base_dir+SubDir+"/notesaves_"+TestDir+"/note_"+NoteIndex+".txt";
    {
        Debug.Log("SubApp_Note: Load_Note: attempting to load:"+notedir);
        if (File.Exists(notedir))
        {
            string load_dat = System.IO.File.ReadAllText(@notedir);
            string a, b, c;//strings to hold loaded variable information
            string[] temp;//parsing string array

            a = load_dat.Substring(load_dat.IndexOf('(') + 1);
            b = a.Substring(a.IndexOf('(') + 1);
            c = b.Substring(b.IndexOf(')') + 1);

            temp = a.Split(',', ')');
            //pick_color(NoteIndex);
            //Debug.Log("split1="+temp[0] + temp[1] + temp[2] + temp[3]);
            HeaderColor = new Color(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), float.Parse(temp[3]));//update color
            this.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Graphic>().color = HeaderColor;//apply to header visually

            temp = b.Split(',', ')');
            //Debug.Log("split2=" + temp[0] + temp[1] + temp[2]);
            if (Open_in_loc)//move to saved location
            {
                //this.transform.position
                deltamove= new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
                note_pos = deltamove;
            }
            this.gameObject.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text= "Note #" + (NoteIndex+1);
            Note_Input.text = c;
            return true;
        }
        else
        {
            Debug.Log("SubApp_Note: Load_Note: file does not exist:"+notedir);
            return false;
        }
        
    }
    public void Exit_Note()
    {
        Debug.Log("SubApp_Note: Exit_Note: Removing instance");
        //update button
        Destroy(this.gameObject);
        //open keyboard or start voice
    }
    private void Update_sidebar(int button,int option)//update functions and button labels
    {
        switch (button)
        {
            case 0://move
                switch (option)
                {
                    case 0://enable move
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetComponent<Button>().enabled = true;
                        break;
                    case 1://disable move
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetComponent<Button>().enabled = false;
                        break;
                    default:
                        break;
                }
                break;
            case 1://exit
                switch (option)
                {
                    case 0://enable move
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetComponent<Button>().enabled = true;
                        break;
                    case 1://disable move
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetComponent<Button>().enabled = false;
                        break;
                    default:
                        break;
                }
                break;
            case 2://prev
                switch (option)
                {
                    case 0://enable prev note
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).transform.GetComponent<Button>().enabled = true;
                        break;
                    case 1://disable prev note
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).transform.GetComponent<Button>().enabled = false;
                        break;
                    default:
                        break;
                }
                break;
            case 3://next
                switch (option)
                {
                    case 0://enable next note
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).transform.GetComponent<Button>().enabled = true;
                        break;
                    case 1://disable next note
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).transform.GetComponent<Button>().enabled = false;
                        break;
                    case 2://next form
                        BUTTON_new = false;
                        break;
                    case 3://new form
                        BUTTON_new = true;
                        break;
                    default:
                        break;
                }
                break;
            case 4://edit
                switch (option)
                {
                    case 0://enable edit button
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetComponent<Button>().enabled = true;
                        break;
                    case 1://disable edit button
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetComponent<Button>().enabled = false;
                        break;
                    case 2://edit form
                        BUTTON_edit = true;
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = "Edit";
                        Edit_Note(false);
                        break;
                    case 3://save form
                        BUTTON_edit = false;
                        this.gameObject.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = "Save";
                        break;
                    default:
                        break;
                }
                break;
            default:
                switch (option)
                {
                    default:
                        break;
                }
                break;
        }
    }
}
