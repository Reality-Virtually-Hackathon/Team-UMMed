//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License. See LICENSE in the project root for license information.

//using UnityEngine;

//namespace HoloToolkit.Unity.InputModule.Tests
//{
//    [RequireComponent(typeof(Renderer))]
//    public class ObjectKeywords : MonoBehaviour, ISpeechHandler, IFocusable
//    {
//        private Material cachedMaterial;
//        private bool gazing = false;
//        private void Awake()
//        {
//            cachedMaterial = GetComponent<Renderer>().material;
//        }

//        public void OnFocusEnter()
//        {
//            gazing = true;
//        }

//        public void OnFocusExit()
//        {
//            gazing = false;
//        }

//        public void ChangeColor(string color)
//        {
//            switch (color.ToLower())
//            {
//                case "red":
//                    cachedMaterial.SetColor("_Color", Color.red);
//                    break;
//                case "blue":
//                    cachedMaterial.SetColor("_Color", Color.blue);
//                    break;
//                case "green":
//                    cachedMaterial.SetColor("_Color", Color.green);
//                    break;
//            }
//        }

//        public void ChangeObject(string command)
//        {
//            if (gazing)
//            {
//                if(this.tag == "KneeExpandController")
//                {
//                    switch (command.ToLower())
//                    {
//                        case "open": case "separate": case "apart": case "separation": break;
//                        case "close": case "joint": case "joined": case "together": case "join": case "assembly": case "group": break;

//                    }
//                }else if (this.tag == "knee")
//                {
//                    switch (command.ToLower())
//                    {
//                        case "bigger": break;
//                        case "smaller": break;
//                        case "turn left": break;
//                        case "turn right": break;
//                        case "move left" : break;
//                        case "move right": break;
//                        case "move up":  break;
//                        case "move down":  break;
//                        case "move forward": break;
//                        case "move backward": break;

//                    }
//                } else
//                {
//                    switch (command.ToLower())
//                    {
//                        case "reset": case "reset world": case "restart":  break;
//                        case "next": break;

//                    }

//                }


//            }
//        }

//        public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
//        {
//            //ChangeColor(eventData.RecognizedText);
//            ChangeObject(eventData.RecognizedText);
//        }

//        private void OnDestroy()
//        {
//            DestroyImmediate(cachedMaterial);
//        }
//    }
//}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;


public class ObjectKeywords : MonoBehaviour
{
    Vector3 originalPosition;
    private const float DefaultSizeFactor = 2.0f;

    [Tooltip("Size multiplier to use when scaling the object up and down.")]
    public float SizeFactor = DefaultSizeFactor;

    // Use this for initialization
    void Start()
    {
        // Grab the original local position of the sphere when the app starts.
        originalPosition = this.transform.localPosition;
    }

    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // If the sphere has no Rigidbody component, add one to enable physics.
        if (!this.GetComponent<Rigidbody>())
        {
            var rigidbody = this.gameObject.AddComponent<Rigidbody>();
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.useGravity = false;
        }
    }

    // Called by SpeechManager when the user says the "Reset world" command
    void OnReset()
    {
        // If the sphere has a Rigidbody component, remove it to disable physics.
        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            DestroyImmediate(rigidbody);
        }

        // Put the sphere back into its original local position.
        this.transform.localPosition = originalPosition;
    }

    // Called by SpeechManager when the user says the "Drop sphere" command
    void OnDrop()
    {
        // Just do the same logic as a Select gesture.
        OnSelect();
    }

    // make it bigger
    void OnEnlarge()
    {
        Vector3 scale = this.transform.localScale;
        //scale *= SizeFactor;
        scale /= 0.9f;
        this.transform.localScale = scale;
        //System.Console.Write("change scale bigger");
        //this.transform.Rotate(Vector3.forward, -50f * Time.deltaTime);
        //transform.localRotation = Quaternion.Euler(0f, 45.0f, 0f);
        //this.transform.localRotation = Quaternion.Euler(0f, 45.0f, 0f);
    }

    // make it smaller
    void OnShrink()
    {
        Vector3 scale = this.transform.localScale;
        //scale *= SizeFactor;
        scale *= 0.9f;
        //scale.x += 0.1f;
        this.transform.localScale = scale;
    }

    // move it to left right forward up down


    void OnMoveIt(string input)
    {
        Vector3 pos = this.transform.position;

        switch (input)
        {
            case "left":
                pos.z -= 0.05f;
                this.transform.position = pos;
                break;
            case "right":
                pos.z += 0.05f;
                this.transform.position = pos;
                break;
            case "forward":
                pos.x -= 0.05f;
                this.transform.position = pos;
                break;
            case "backward":
                pos.x += 0.05f;
                this.transform.position = pos;
                break;
            case "down":
                pos.y -= 0.05f;
                this.transform.position = pos;
                break;
            case "up":
                pos.y += 0.05f;
                this.transform.position = pos;
                break;
            default:

                break;
        }
    }


    //transform.Rotate(0,45,0);
    void OnRotate(string input)
    {
        //Vector3 pos = this.transform.position;
        //float turnSpeed = 50f;
        //float turny = this.transform.localEulerAngles.y + 10f;
        //transform.parent.transform.Rotate(0, transform.parent.transform.rotation.y+90, 0);
        switch (input)
        {
            case "left":
                if (this.tag == "mouse_box") { this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x + 10f, 0f, 90f); }
                else { this.transform.localRotation = Quaternion.Euler(0f, this.transform.localEulerAngles.y + 45f, 0f); }
                break;
            case "right":
                if (this.tag == "mouse_box") { this.transform.localRotation = Quaternion.Euler(this.transform.localEulerAngles.x - 10f, 0f, 90f); }
                else { this.transform.localRotation = Quaternion.Euler(0f, this.transform.localEulerAngles.y - 45f, 0f); }
                break;

            default:
                break;
        }
    }


    void ToggleModel(string input)
    {
        SelectObject.KneeExpend();
    }

    void ResetWorld()
    {

    }


    void NextScene()
    {
        
    }

    void ToggleGameObject(string input)
    {
        switch (input)
        {
            case "human":
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.tag == "human")
                    {
                        go.GetComponent<Renderer>().enabled = !go.GetComponent<Renderer>().enabled;
                    }
                }
                break;
            case "mouse":
                foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
                {
                    if (go.tag == "mouse")
                    {
                        go.GetComponent<Renderer>().enabled = !go.GetComponent<Renderer>().enabled;
                    }
                }
                break;
            default:
                break;
        }
    }


}

