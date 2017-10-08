using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;
public class Hold2Rotate : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable, IManipulationHandler
{
    public float Speed = 3;

    public int rotate_counter = 0;
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
        InputManager.Instance.PushModalInputHandler(gameObject);
        this.GetComponent<Renderer>().material.color = Color.green;
        log("started");
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        //Debug.LogFormat("OnManipulation Updated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
        //    eventData.InputSource,
        //    eventData.SourceId,
        //    eventData.CumulativeDelta.x,
        //    eventData.CumulativeDelta.y,
        //    eventData.CumulativeDelta.z);
        
        //float multiplier = 100.0f;
        //float cameraLocalYRotation = Camera.main.transform.localRotation.eulerAngles.y;

        //if (cameraLocalYRotation > 270 || cameraLocalYRotation < 90)
        //    multiplier = -100.0f;

        //var rotation = new Vector3(eventData.CumulativeDelta.y * -multiplier, eventData.CumulativeDelta.x * multiplier);
        //transform.parent.transform.Rotate(rotation * Speed, Space.World);
        //log2("rotate updated" + eventData.CumulativeDelta.x.ToString());

        float multiplier = 3.0f;
        float cameraLocalYRotation = Camera.main.transform.localRotation.eulerAngles.y;

        if (cameraLocalYRotation > 270 || cameraLocalYRotation < 90)
            multiplier = -3.0f;

        var rotation = new Vector3(eventData.CumulativeDelta.y * -multiplier, eventData.CumulativeDelta.x * multiplier);
        //transform.parent.transform.Rotate(rotation * Speed, Space.World);
        Rotate(rotation);
        log2("rotate updated" + eventData.CumulativeDelta.x.ToString());


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
        InputManager.Instance.PopModalInputHandler();
        this.GetComponent<Renderer>().material.color = Color.white;

    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
        this.GetComponent<Renderer>().material.color = Color.white;
    }


    public void Rotate(Vector3 rotation)
    {
        transform.parent.transform.Rotate(rotation * Speed, Space.World);

        //transform.parent.transform.RotateAround(transform.parent.transform.rotation.eulerAngles, rotation, 1 * Time.deltaTime);
        //transform.parent.transform.RotateAround(Vector3.zero, rotation, 1 * Time.deltaTime);
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
        //this.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void OnFocusExit()
    {
        Debug.Log("OnFocusExit");
        this.GetComponent<Renderer>().material.color = Color.white;
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
        log("rotated");
        //rotate_counter++;
        //if (rotate_counter > 3) rotate_counter = 0;

            transform.parent.transform.Rotate(transform.parent.transform.rotation.x + 90, 0, 0);        

        //log(rotate_counter + " - " + (90 * rotate_counter));

    }



    public void log(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text += str.ToString() + " \n ";
    }

    public void log2(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text += str.ToString() + " \n ";
    }

}
