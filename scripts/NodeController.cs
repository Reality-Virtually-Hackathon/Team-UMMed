using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;

public class NodeController : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable, IManipulationHandler, IHoldHandler, ISourceStateHandler
{

    private float UPPER_LIMIT = 0.4f, LOWER_LIMIT = -0.4f, LOW_MAX = -0.05f, UP_MIN = 0.05f; 

    public float Speed = 10;
    public int size_counter = 3;

    public Loader dicom_cube;

    enum Axis
    {
        X,
        Y,
        Z,
        T
    }
    [SerializeField]
    Axis axis;
    [SerializeField]
    bool min;
    [Tooltip("Speed at which the object is resized.")]
    [SerializeField]
    float ResizeSpeedFactor = 1.5f;

    [SerializeField]
    float ResizeScaleFactor = 1.5f;

    [Tooltip("When warp is checked, we allow resizing of all three scale axes - otherwise we scale each axis by the same amount.")]
    [SerializeField]
    bool AllowResizeWarp = false;

    [Tooltip("Minimum resize scale allowed.")]
    [SerializeField]
    float MinScale = 0.3f;

    [Tooltip("Maximum resize scale allowed.")]
    [SerializeField]
    float MaxScale = 4f;

    [SerializeField]
    bool resizingEnabled = true;

    Vector3 lastScale;

    Vector3 lastPosition;


    [SerializeField]
    float DragSpeed = 1.5f;

    [SerializeField]
    float DragScale = 1.5f;

    [SerializeField]
    float MaxDragDistance = 2f;

    public void SetResizing(bool enabled)
    {
        resizingEnabled = enabled;
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
        this.GetComponent<Renderer>().material.color = Color.green;
        InputManager.Instance.PushModalInputHandler(gameObject);
        //lastScale = transform.parent.transform.localScale;
        //InputManager.Instance.PushModalInputHandler(gameObject);
        //lastPosition = transform.position;


    }

    public void UpdateDragging(Vector3 input)
    {
        return;
        Vector3 eventData = input;
        //print(eventData);
        Vector3 pos = transform.localPosition;
        float posV;
        float deltaValue;
        if (axis == Axis.Y)
            deltaValue = eventData.y * 0.1f;
        else
            deltaValue = eventData.x * 0.1f;
        posV = pos.z + deltaValue;
        //print(posV);
        if (Mathf.Abs(posV) >= 0.05 && Mathf.Abs(posV) <= 0.4)
        {
            //print("Updated: " + posV);
            transform.localPosition += Vector3.forward * deltaValue;
            UpdateValue(deltaValue);
        }
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        Vector3 pos = transform.localPosition;
        float posV;
        float deltaValue;
        if(axis == Axis.Y)
            deltaValue = eventData.CumulativeDelta.y/4;
        else
            deltaValue = eventData.CumulativeDelta.x/4;
        posV = pos.z + deltaValue;
        if (Mathf.Abs(posV) >= 0.05 && Mathf.Abs(posV) <= 0.4)
        {
            transform.localPosition += Vector3.forward * deltaValue;
            UpdateValue(deltaValue);
        }

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
        this.GetComponent<Renderer>().material.color = Color.white;
        InputManager.Instance.PopModalInputHandler();
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
        this.GetComponent<Renderer>().material.color = Color.white;
        InputManager.Instance.PopModalInputHandler();
    }







    ////////////////////////////////////////////////////////////////////////////////////////////////////////////








    public void OnInputUp(InputEventData eventData)
    {
        Debug.LogFormat("OnInputUp Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
    }

    public void OnInputDown(InputEventData eventData)
    {
        Debug.LogFormat("OnInputDown Source: {0} SourceId: {1}", eventData.InputSource, eventData.SourceId);
        //ChangeThreshold(eventData);
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

    }




    public void UpdateValue(float deltaValue)
    {
        switch (axis)
        {
            case Axis.X:
                if (min)
                    dicom_cube.SetXMin(dicom_cube.GetXMin() + deltaValue);
                else
                    dicom_cube.SetXMax(dicom_cube.GetXMax() + deltaValue);
                break;
            case Axis.Y:
                if (min)
                    dicom_cube.SetYMin(dicom_cube.GetYMin() + deltaValue);
                else
                    dicom_cube.SetYMax(dicom_cube.GetYMax() + deltaValue);
                break;
            case Axis.Z:
                if (min)
                    dicom_cube.SetZMin(dicom_cube.GetZMin() + deltaValue);
                else
                    dicom_cube.SetZMax(dicom_cube.GetZMax() + deltaValue);
                break;
            case Axis.T:
                if (min)
                    dicom_cube.SetThresholdMin(dicom_cube.GetDMin() + deltaValue);
                else
                    dicom_cube.SetThresholdMax(dicom_cube.GetDMax() + deltaValue);
                break;
        }
    }

    //void Drag(Vector3 positon)
    //{
    //    var targetPosition = lastPosition + positon * DragScale;
    //    if (Vector3.Distance(lastPosition, targetPosition) <= MaxDragDistance)
    //    {
    //        transform.position = Vector3.Lerp(transform.position, targetPosition, DragSpeed);
    //    }
    //}



    public void log(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text = str.ToString() + " \n ";


    }
}
