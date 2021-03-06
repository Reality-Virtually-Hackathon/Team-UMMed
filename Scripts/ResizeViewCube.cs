﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;
using System;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

/// <summary>
/// Component that allows dragging an object with your hand on HoloLens.
/// Dragging is done by calculating the angular delta and z-delta between the current and previous hand positions,
/// and then repositioning the object based on that.
/// </summary>
public class ResizeViewCube : MonoBehaviour,
                                 IFocusable,
                                 IInputHandler,
                                 ISourceStateHandler,
                                 IInputClickHandler,
                                 IManipulationHandler, ISpeechHandler
{
    /// <summary>
    /// Event triggered when dragging starts.
    /// </summary>
    public event Action StartedDragging;

    /// <summary>
    /// Event triggered when dragging stops.
    /// </summary>
    public event Action StoppedDragging;

    [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
    public Transform HostTransform;

    [Tooltip("Scale by which hand movement in z is multipled to move the dragged object.")]
    public float DistanceScale = 2f;

    [Tooltip("Should the object be kept upright as it is being dragged?")]
    public bool IsKeepUpright = false;

    [Tooltip("Should the object be oriented towards the user as it is being dragged?")]
    public bool IsOrientTowardsUser = true;

    public bool IsDraggingEnabled = true;

    private Camera mainCamera;
    private bool isDragging;
    private bool isGazed;
    private Vector3 objRefForward;
    private float objRefDistance;
    private Quaternion gazeAngularOffset;
    private float handRefDistance;
    private Vector3 objRefGrabPoint;

    private Vector3 draggingPosition;
    private Quaternion draggingRotation;

    private IInputSource currentInputSource = null;
    private uint currentInputSourceId;

    private GameObject hit;
    public GameObject ControllerObject;
    public Bounds selectedObjectBounds;
    private float ScaleIncrement = 0.1f;
    public Color boxColor = new Color32(98, 99, 255, 128);
    public static Material boxMat;
    private Renderer rend;

    bool Draw = false;
    //public bool GotTransform { get; private set; }

    //private bool gazing = false;


    [SerializeField]
    float DragSpeed = 1.5f;

    [SerializeField]
    float DragScale = 1.5f;

    [SerializeField]
    float MaxDragDistance = 3f;

    Vector3 lastPosition;


    private void Start()
    {
        if (HostTransform == null)
        {
            HostTransform = transform;
        }

        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        if (isDragging)
        {
            StopDragging();
        }

        if (isGazed)
        {
            OnFocusExit();
        }
    }

    private void Update()
    {
        if (IsDraggingEnabled && isDragging)
        {
            UpdateDragging();
        }
    }

    /// <summary>
    /// Starts dragging the object.
    /// </summary>
    public void StartDragging()
    {
        if (!IsDraggingEnabled)
        {
            return;
        }

        if (isDragging)
        {
            return;
        }

        // Add self as a modal input handler, to get all inputs during the manipulation
        InputManager.Instance.PushModalInputHandler(gameObject);

        isDragging = true;
        //GazeCursor.Instance.SetState(GazeCursor.State.Move);
        //GazeCursor.Instance.SetTargetObject(HostTransform);

        Vector3 gazeHitPosition = GazeManager.Instance.HitInfo.point;
        Vector3 handPosition;
        currentInputSource.TryGetPosition(currentInputSourceId, out handPosition);

        Vector3 pivotPosition = GetHandPivotPosition();
        handRefDistance = Vector3.Magnitude(handPosition - pivotPosition);
        objRefDistance = Vector3.Magnitude(gazeHitPosition - pivotPosition);

        Vector3 objForward = HostTransform.forward;

        // Store where the object was grabbed from
        objRefGrabPoint = mainCamera.transform.InverseTransformDirection(HostTransform.position - gazeHitPosition);

        Vector3 objDirection = Vector3.Normalize(gazeHitPosition - pivotPosition);
        Vector3 handDirection = Vector3.Normalize(handPosition - pivotPosition);

        objForward = mainCamera.transform.InverseTransformDirection(objForward);       // in camera space
        objDirection = mainCamera.transform.InverseTransformDirection(objDirection);   // in camera space
        handDirection = mainCamera.transform.InverseTransformDirection(handDirection); // in camera space

        objRefForward = objForward;

        // Store the initial offset between the hand and the object, so that we can consider it when dragging
        gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
        draggingPosition = gazeHitPosition;

        StartedDragging.RaiseEvent();
    }

    /// <summary>
    /// Gets the pivot position for the hand, which is approximated to the base of the neck.
    /// </summary>
    /// <returns>Pivot position for the hand.</returns>
    private Vector3 GetHandPivotPosition()
    {
        Vector3 pivot = Camera.main.transform.position + new Vector3(0, -0.2f, 0) - Camera.main.transform.forward * 0.2f; // a bit lower and behind
        return pivot;
    }

    /// <summary>
    /// Enables or disables dragging.
    /// </summary>
    /// <param name="isEnabled">Indicates whether dragging shoudl be enabled or disabled.</param>
    public void SetDragging(bool isEnabled)
    {
        if (IsDraggingEnabled == isEnabled)
        {
            return;
        }

        IsDraggingEnabled = isEnabled;

        if (isDragging)
        {
            StopDragging();
        }
    }

    /// <summary>
    /// Update the position of the object being dragged.
    /// </summary>
    private void UpdateDragging()
    {
        Vector3 newHandPosition;
        currentInputSource.TryGetPosition(currentInputSourceId, out newHandPosition);

        Vector3 pivotPosition = GetHandPivotPosition();

        Vector3 newHandDirection = Vector3.Normalize(newHandPosition - pivotPosition);

        newHandDirection = mainCamera.transform.InverseTransformDirection(newHandDirection); // in camera space
        Vector3 targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
        targetDirection = mainCamera.transform.TransformDirection(targetDirection); // back to world space

        float currenthandDistance = Vector3.Magnitude(newHandPosition - pivotPosition);

        float distanceRatio = currenthandDistance / handRefDistance;
        float distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
        float targetDistance = objRefDistance + distanceOffset;

        draggingPosition = pivotPosition + (targetDirection * targetDistance);

        if (IsOrientTowardsUser)
        {
            draggingRotation = Quaternion.LookRotation(HostTransform.position - pivotPosition);
        }
        else
        {
            Vector3 objForward = mainCamera.transform.TransformDirection(objRefForward); // in world space
            draggingRotation = Quaternion.LookRotation(objForward);
        }

        // Apply Final Position
        HostTransform.position = draggingPosition + mainCamera.transform.TransformDirection(objRefGrabPoint);
        //HostTransform.rotation = draggingRotation; // comment out to stop change rotation during drag

        if (IsKeepUpright)
        {
            Quaternion upRotation = Quaternion.FromToRotation(HostTransform.up, Vector3.up);
            HostTransform.rotation = upRotation * HostTransform.rotation;
        }
    }

    /// <summary>
    /// Stops dragging the object.
    /// </summary>
    public void StopDragging()
    {
        if (!isDragging)
        {
            return;
        }

        // Remove self as a modal input handler
        InputManager.Instance.PopModalInputHandler();

        isDragging = false;
        currentInputSource = null;
        StoppedDragging.RaiseEvent();
    }

    public void OnFocusEnter()
    {
        if (!IsDraggingEnabled)
        {
            return;
        }

        if (isGazed)
        {
            return;
        }

        isGazed = true;
    }

    public void OnFocusExit()
    {
        if (!IsDraggingEnabled)
        {
            return;
        }

        if (!isGazed)
        {
            return;
        }

        isGazed = false;
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (currentInputSource != null &&
            eventData.SourceId == currentInputSourceId)
        {
            StopDragging();
        }
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (isDragging)
        {
            // We're already handling drag input, so we can't start a new drag operation.
            return;
        }

        if (!eventData.InputSource.SupportsInputInfo(eventData.SourceId, SupportedInputInfo.Position))
        {
            // The input source must provide positional data for this script to be usable
            return;
        }

        currentInputSource = eventData.InputSource;
        currentInputSourceId = eventData.SourceId;
        //StartDragging();  //----------------------------------------------------------------------------------------
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        // Nothing to do
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        if (currentInputSource != null && eventData.SourceId == currentInputSourceId)
        {
            StopDragging();
        }
    }

    public void OnInputClicked(InputEventData eventData)
    {
        hit = GazeManager.Instance.HitInfo.transform.gameObject;

        if (!Draw || GameObject.FindGameObjectsWithTag("ControllerObject").Length == 0)
        {
            //DeleteAllControllerBox();
            //DrawSelectionBox();
        }
        else if (Draw && gameObject.transform.childCount != 0 && gameObject.tag != "Cursor")
        {
            //DeleteAllControllerBox();
        }


        if (hit.tag == "room")
        {
            //addRigidbody()
            //Rigidbody gameObjectsRigidBody = hit.AddComponent<Rigidbody>();
            //gameObjectsRigidBody.mass = 0.5f;
            //gameObjectsRigidBody.drag = 1;
        }
    }

    

    
    


    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        Debug.Log(eventData.RecognizedText.ToLower());
    }


    public void log(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        //GameObject.Find("StatusTxt").GetComponent<Text>().text += str.ToString() + " \n ";

    }

    //-----------------------------------------------------------------------------------------//

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        InputManager.Instance.PushModalInputHandler(gameObject);
        lastPosition = transform.position;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        if (true)
        {
            Drag(eventData.CumulativeDelta);

            //sharing & messaging
            //SharingMessages.Instance.SendDragging(Id, eventData.CumulativeDelta);
        }
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        InputManager.Instance.PopModalInputHandler();
    }

    void Drag(Vector3 positon)
    {
        var targetPosition = lastPosition + positon * DragScale;
        if (Vector3.Distance(lastPosition, targetPosition) <= MaxDragDistance)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, DragSpeed);
        }
    }

    //-----------------------------------------------------------------------------------------//

}

