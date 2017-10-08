using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer = null;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    // Use this for initialization
    void Start()
    {
        keywords.Add("Reset world", () =>
        {
            // Call the OnReset method on every descendant object.
            this.BroadcastMessage("OnReset");
            //Console.Write("hello");
        });
        /*
        keywords.Add("Drop Sphere", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the OnDrop method on just the focused object.
                focusObject.SendMessage("OnDrop");
            }
        });
        */
        keywords.Add("bigger", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnEnlarge");
            }

            //System.Diagnostics.Debug.WriteLine("hi");
            //System.Console.Write("OnEnlarge");
        });

        keywords.Add("smaller", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnShrink");
            }
        });

        keywords.Add("move left", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnMoveIt", "left");
            }
        });

        keywords.Add("move right", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnMoveIt", "right");
            }
        });

        keywords.Add("move forward", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnMoveIt", "forward");
            }
        });

        keywords.Add("move backward", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnMoveIt", "backward");
            }
        });

        keywords.Add("move up", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnMoveIt", "up");
            }
        });

        keywords.Add("move down", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnMoveIt", "down");
            }
        });

        keywords.Add("turn left", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnRotate", "left");
            }
        });

        keywords.Add("turn right", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null && !focusObject.tag.ToString().Contains("Controller"))
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("OnRotate", "right");
            }
        });


        keywords.Add("hide human", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("ToggleGameObject", "human");
            }
        });

        keywords.Add("hide mouse", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("ToggleGameObject", "mouse");
            }
        });

        keywords.Add("display human", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("ToggleGameObject", "human");
            }
        });

        keywords.Add("display mouse", () =>
        {
            var focusObject = GazeGestureManager.Instance.FocusedObject;
            if (focusObject != null)
            {
                // Call the Enlarge method on just the focused object.
                focusObject.SendMessage("ToggleGameObject", "mouse");
            }
        });


        string[] commands1 = new string[] { "open", "separate", "apart", "separation" };
        string[] commands2 = new string[] { "close", "joint", "joined", "together", "join", "assembly", "group", "regroup" };
        string[] commands3 = new string[] { "reset", "reset world", "restart" };
        string[] commands4 = new string[] { "next", "next world", "next scene", "next demo", "next model" };
        string[] commands5 = new string[] { "help", "show help", "display help", "what can I do", "how do i do this", "I need help", "open manual" };
        string[] commands6 = new string[] { "close help", "hide help","hide", "close manual" };


        for (int i = 0; i < commands1.Length; i++)
        {
            keywords.Add(commands1[i].ToString(), () =>
            {
                //this.BroadcastMessage("ToggleModel", "open");
                var focusObject = GazeGestureManager.Instance.FocusedObject;
                if (focusObject != null && focusObject.tag.ToString().Contains("Controller"))
                {
                    // Call the Enlarge method on just the focused object.
                    focusObject.SendMessage("ToggleModel", "open");
                }
            });
        }

        for (int i = 0; i < commands2.Length; i++)
        {
            keywords.Add(commands2[i].ToString(), () =>
            {
                //this.BroadcastMessage("ToggleModel", "close");
                var focusObject = GazeGestureManager.Instance.FocusedObject;
                if (focusObject != null && focusObject.tag.ToString().Contains("Controller"))
                {
                    // Call the Enlarge method on just the focused object.
                    focusObject.SendMessage("ToggleModel", "close");
                }
            });
        }


        for (int i = 0; i < commands3.Length; i++)
        {
            keywords.Add(commands3[i].ToString(), () =>
            {
                this.BroadcastMessage("ResetWorld");
            });
        }


        for (int i = 0; i < commands4.Length; i++)
        {
            keywords.Add(commands4[i].ToString(), () =>
            {
                this.BroadcastMessage("NextScene");
            });
        }

        for (int i = 0; i < commands5.Length; i++)
        {
            keywords.Add(commands5[i].ToString(), () =>
            {
                GameObject[] list = GameObject.FindGameObjectsWithTag("help_screen");
                foreach (GameObject go in list)
                {
                    go.GetComponent<Renderer>().enabled = true;
                }
            });
        }

        for (int i = 0; i < commands6.Length; i++)
        {
            keywords.Add(commands6[i].ToString(), () =>
            {
                GameObject[] list = GameObject.FindGameObjectsWithTag("help_screen");
                foreach (GameObject go in list)
                {
                    go.GetComponent<Renderer>().enabled = false;
                }
            });
        }
        


        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}