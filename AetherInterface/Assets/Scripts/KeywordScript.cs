
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;

/*
 *  The purpose of this script is to allow for the recognition of verbal commands. It works
 *  by setting up a primary listener that only listens for one command, "Karen", which then
 *  starts a secondarylistener that has more commands. You can see these commands and their 
 *  purpose by saying  "Karen show commands". To add commands you must use the AddCommand function.
 *  This function requires the dictionary you want to add to, the function you want to add, the keyword
 *  to trigger that function and a brief but sufficient explanation on what that command does.
 */

public class KeywordScript : MonoBehaviour
{

    // Dictionaries for each listener that hold the keywords as a string and functions as a System.Action  
    Dictionary<string, System.Action> keywordDict = new Dictionary<string, System.Action>();
    Dictionary<string, System.Action> KarensWords = new Dictionary<string, System.Action>();

    // Commands and their purpose used for ListCommands function
    string[] commands = new string[35]; // Size can be changed later if we add more functions

    // Sets the minimum level of confidence that the recognizer will allow 
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    // Creates the recognizers
    protected PhraseRecognizer recognizer;
    protected PhraseRecognizer Karen;

    private void Start()
    {
        // Adds the keyword and function to the dictionary - Capitalization of the keyword does not matter
        // Must be done before the keys are added to the keyword array!!!

        AddFunction(
            keywordDict,
            KarenCalled,
            "Karen",
            "This phrase is mandatory to start the Karen listener which accepts further commands"
            );

        AddFunction(
            KarensWords,
            ShowCommands,
            "Show commands",
            "The purpose of this command is to show what Karen can do for you"
            );

        AddFunction(
            KarensWords,
            GameObject.Find("menu").GetComponent<Menu>().createTimer,
            "Activate Timer",
            "The purpose of this command is destroy the timer application"
            );

        //These are for the procedure/taskboard manager
        AddFunction(
            KarensWords,
            KarenCallStep,
            "Read Step",
            "The purpose of this command is read the current step on the E.V.A. procedure"
            );
        AddFunction(
            KarensWords,
            KarenMoveTaskboard,
            "Move Taskboard",
            "The purpose of this command is to reposition the taskboard"
            );
        AddFunction(
            KarensWords,
            ProcedureManager.Instance.NextStep,
            "Next",
            "The purpose of this command is to go to the next procedure step"
            );
        AddFunction(
            KarensWords,
            ProcedureManager.Instance.PreviousStep,
            "Previous",
            "The purpose of this command is to go to the previous procedure step"
            );
        /*
        AddFunction(
            KarensWords,
            GameObject.Find("menu").GetComponent<Menu>().createTimer,
            "Activate Timer",
            "The purpose of this command is destroy the timer application"
            );

        //These are for the procedure/taskboard manager
        AddFunction(
            KarensWords,
            KarenCallStep,
            "Read Step",
            "The purpose of this command is read the current step on the E.V.A. procedure"
            );
            */
        // Have to convert dictionary keys into a string array for the recognizer to use them
        string[] keywordArr = keywordDict.Keys.ToArray();
        string[] KarensKeys = KarensWords.Keys.ToArray();

        // If for some reason the array is empty, the listener will not start
        if (keywordArr != null && KarensWords != null)
        {

            // Gives the recognizer its words and its confidence level in those words
            recognizer = new KeywordRecognizer(keywordArr, confidence);
            Karen = new KeywordRecognizer(KarensKeys, confidence);

            // Gives the recognizer something to do when its triggered by a keyword
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            Karen.OnPhraseRecognized += Recognizer_OnPhraseRecognized;

            // Tells the recognizer to start listening
            recognizer.Start();

            // Just to make sure everything is working
            Debug.Log("Recognizer running : " + recognizer.IsRunning);
        }
        else
        {
            Debug.Log("The array is empty!!! Please fix me.");
        }
    }
    public void karenTest()
    {
        Debug.Log("Button Activated");
    }
    public void KarenCallStep()
    {
        GameObject.Find("ProcedureManager").GetComponent<ProcedureManager>().stepSpeak();
    }
    public void KarenMoveTaskboard()
    {
        //TaskboardManager.Instance.moveTaskboard();
    }
    // This is Karen - Everything here is basically the same as in the start function
    // other than the fact that this one has a time limit and a notifier to the user
    public void KarenCalled()
    {
        if (!Karen.IsRunning)
        {
            // Make a notifier
            Karen.Start();
            Debug.Log("Karen is listening : " + Karen.IsRunning);
		    GameObject.Find("Audio Manager/VoiceSound").GetComponent<AudioSource>().Play();            


            //Debug.Log("Karen called ");

            StartCoroutine(KarenTimer(15));// Time in seconds
            // Close the notifier
        }
    }

    // This function is called whenever a phrase is recognized
    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        Debug.Log("You said: " + args.text + ", Confidence level; " + args.confidence);

        // If Karen was the one that invoked the function, stop her from listening until called again
        if (KarensWords.ContainsKey(args.text))
        {
            // Also have to pull from a different dictionary if Karen is called
            if (KarensWords.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
            else
            {
                Debug.Log("Error: " + args.text + " does not have a value in the dictonary");
            }
            StopKaren(Karen);
        }
        else
        {
            if (keywordDict.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
            }
            else
            {
                Debug.Log("Error: " + args.text + " does not have a value in the dictonary");
            }
        }
    }

    private void StopKaren(PhraseRecognizer Karen)
    {
        if (Karen.IsRunning)
        {
            Karen.Stop();
            Debug.Log("Karen stopped : " + !(Karen.IsRunning));
        }
    }

    private void Update()
    {
        if (recognizer == null) {
            AddFunction(
            keywordDict,
            KarenCalled,
            "Karen",
            "This phrase is mandatory to start the Karen listener which accepts further commands"
            );
            string[] keywordArr = keywordDict.Keys.ToArray();
            
            recognizer = new KeywordRecognizer(keywordArr, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
        }

        if (!recognizer.IsRunning)
        {
            recognizer.Start();
        }
    }

    // Clears Phrase actions and stops mic from listening 
    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
        if (Karen != null && Karen.IsRunning)
        {
            Karen.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            Karen.Stop();
        }
    }




    /* Example : 
     *      AddFunction(
     *          KarensWords, 
     *          ShowNextStep(), 
     *          "Show next step", 
     *          "The purpose of this is to show the next step of the current task"
     *      );
     */

    // Used for index in the AddFunction, faster than using a forloop
    int x = 0;

    // Adds functions and keywords to the dictionary and then adds keyword and purpose to a string array for printing later
    private void AddFunction(Dictionary<string, System.Action> dict, System.Action function, string keyword, string purpose)
    {
        if (x > 35)
        {
            Debug.Log("Too many commands! Please change the size of the array; keyword being added - " + keyword);
        }
        else
        {
            dict.Add(keyword, () => { function(); });
            commands[x] = keyword + " : " + purpose;
            x++;
        }
    }

    // Pretty self explanatory, shows commands when asked to
    private void ShowCommands()
    {
        foreach (string i in commands)
        {
            if (i != null) Debug.Log(i);
        }
    }

    // Timer for Karen 
    // MUST BE STARTED AS A COROUTINE TO WORK - as seen in KarenCalled function
    IEnumerator KarenTimer(float x)
    {
        //Debug.Log(Time.time);
        yield return new WaitForSeconds(x);
        StopKaren(Karen);
        //Debug.Log(Time.time);
    }

    // -- Keep Code above here -- // 
}
