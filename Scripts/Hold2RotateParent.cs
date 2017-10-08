using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;

public class Hold2RotateParent : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable, IManipulationHandler
{
    public float Speed = 2;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }







    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulation Started\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);
        log("started");
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulation Updated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);

        float multiplier = 1.0f;
        float cameraLocalYRotation = Camera.main.transform.localRotation.eulerAngles.y;

        if (cameraLocalYRotation > 270 || cameraLocalYRotation < 90)
            multiplier = -1.0f;

        var rotation = new Vector3(eventData.CumulativeDelta.y * -multiplier, eventData.CumulativeDelta.x * multiplier);
        transform.Rotate(rotation * Speed, Space.World);
        log("rotate updated" + eventData.CumulativeDelta.x.ToString());
        /*
        log("rotate updated " + eventData.InputSource + " " +
            eventData.SourceId + " " +
            eventData.CumulativeDelta.x + " " +
            eventData.CumulativeDelta.y+ " " +
            eventData.CumulativeDelta.z);
            */

    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulation Completed\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);
        log("completed");
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        Debug.LogFormat("OnManipulation Canceled\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
            eventData.InputSource,
            eventData.SourceId,
            eventData.CumulativeDelta.x,
            eventData.CumulativeDelta.y,
            eventData.CumulativeDelta.z);
        log("canceled");
    }







    ////////////////////////////////////////////////////////////////////////////////////////////////////////////








    public void OnInputUp(InputEventData eventData)
    {
        Debug.LogFormat("OnInputUp Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.LogFormat("OnInputDown Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }


    public void OnFocusEnter()
    {
        Debug.Log("OnFocusEnter");
    }

    public void OnFocusExit()
    {
        Debug.Log("OnFocusExit");
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.LogFormat("OnSourceDetected Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        Debug.LogFormat("OnSourceLost Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnHoldStarted(HoldEventData eventData)
    {
        Debug.LogFormat("OnHoldStarted n Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnHoldCompleted(HoldEventData eventData)
    {
        Debug.LogFormat("OnHoldCompleted n Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnHoldCanceled(HoldEventData eventData)
    {
        Debug.LogFormat("OnHoldCanceled n Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        Debug.Log(eventData.RecognizedText.ToLower());
    }

    public void OnInputClicked(InputEventData eventData)
    {
        //throw new NotImplementedException();
        log("clicked");
    }



    public void log(string str)
    {
        GameObject.Find("StatusTxt").GetComponent<Text>().text += str.ToString() + " \n ";
    }

    public void log2(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text = str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text = str.ToString() + " \n ";
    }

}
