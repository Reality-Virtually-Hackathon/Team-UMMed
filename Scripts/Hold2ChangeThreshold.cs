using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;

public class Hold2ChangeThreshold : MonoBehaviour, IInputHandler, IInputClickHandler, IFocusable, IManipulationHandler
{
    public float Speed = 10;
    public int size_counter = 3;

    public SpriteLoader dicom_cube;

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
        //lastScale = transform.parent.transform.localScale;
        //InputManager.Instance.PushModalInputHandler(gameObject);
        lastPosition = transform.position;


    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {

        if (resizingEnabled)
        {
            ChangeThreshold(eventData.CumulativeDelta);
            //Resize(eventData.CumulativeDelta);

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
        
        // 0.7 min to 2.7 max on x axis
        // threshold is between 0 to 1
        // tach tap increase 0.2 and 0.1
        //gameObject.transform.parent.parent.GetComponent<setThreshold>();

        //GameObject t = gameObject;
        Transform t = gameObject.transform.parent.parent;

        float v =  gameObject.transform.localPosition.x;
        if ( v <= 2.7f )
            gameObject.transform.localPosition = new Vector3(v + 0.2f, -3.5f, -2.03f);
        else
            gameObject.transform.localPosition = new Vector3(0.7f, -3.5f, -2.03f);


        //var dicom = GameObject.Find("SpriteHolder");
        //dicom.GetComponent<setThreshold>();
        dicom_cube.setThreshold(gameObject.transform.localPosition.x/2.7f);
        // log("clicked");

        log(lastPosition.x.ToString());
    }




    public void ChangeThreshold(Vector3 positon)
    {
        // move ball x only 
        positon.x = positon.x * DragScale;
        positon.y = 0f;
        positon.z = 0f;

        var targetPosition = lastPosition + positon;
        // set min and max 
        if ( targetPosition.x >= 0.65f && targetPosition.x <= 1.3f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, DragSpeed);
            // change DICOM threshold
            dicom_cube.setThreshold((transform.localPosition.x - 0.65f) / 0.65f);
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
