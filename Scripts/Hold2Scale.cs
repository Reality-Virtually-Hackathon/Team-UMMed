using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;

public class Hold2Scale : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable, IManipulationHandler
{
    public float Speed = 10;
    public int size_counter = 3;

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

    public void SetResizing(bool enabled)
    {
        resizingEnabled = enabled;
    }

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
        this.GetComponent<Renderer>().material.color = Color.green;
        InputManager.Instance.PushModalInputHandler(gameObject);
        lastScale = transform.parent.transform.localScale;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {

        if (resizingEnabled)
        {
            Resize(eventData.CumulativeDelta);

            //sharing & messaging
            //SharingMessages.Instance.SendResizing(Id, eventData.CumulativeDelta);
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
        size_counter++;
        if (size_counter > 5) size_counter = 1;

        switch (size_counter)
        {
            case 1 :
                transform.parent.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                break;
            case 2:
                transform.parent.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                break;
            case 3:
                transform.parent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                break;
            case 4:
                transform.parent.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                break;
            case 5:
                transform.parent.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
                break;
        }


        log("clicked");
    }

    void Resize(Vector3 newScale)
    {
        float resizeX, resizeY, resizeZ;
        //if we are warping, honor axis delta, else take the x
        if (AllowResizeWarp)
        {
            resizeX = newScale.x * ResizeScaleFactor;
            resizeY = newScale.y * ResizeScaleFactor;
            resizeZ = newScale.z * ResizeScaleFactor;
        }
        else
        {
            resizeX = resizeY = resizeZ = newScale.x * ResizeScaleFactor;
        }

        resizeX = Mathf.Clamp(lastScale.x + resizeX, MinScale, MaxScale);
        resizeY = Mathf.Clamp(lastScale.y + resizeY, MinScale, MaxScale);
        resizeZ = Mathf.Clamp(lastScale.z + resizeZ, MinScale, MaxScale);

        transform.parent.transform.localScale = Vector3.Lerp(transform.localScale,
            new Vector3(resizeX, resizeY, resizeZ),
            ResizeSpeedFactor);
    }

    public void log(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text = str.ToString() + " \n ";


    }
}
