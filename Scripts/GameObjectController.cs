using UnityEngine;
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
[RequireComponent(typeof(AudioSource))]
public class GameObjectController : MonoBehaviour,
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

    public AudioSource audioSource;
    public AudioClip dingSound;

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
        spawnPos = gameObject.transform.position;
        spawnRot = gameObject.transform.rotation;
        //SoundFile = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();

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
            if (comparePos())
            {
                // change gameObject color to green, and play ding sound clip
                // gameObject.GetComponent<Renderer>().material.color = new Color32(0, 254, 111, 1);
                if (gameObject.transform.childCount > 1)
                {
                    foreach (Transform child in gameObject.transform)
                    {
                        child.GetComponent<Renderer>().material.color = new Color32(0, 254, 111, 1);
                    }
                }
                else
                {
                    gameObject.GetComponent<Renderer>().material.color = new Color32(0, 254, 111, 1);
                }

                audioSource.PlayOneShot(dingSound, 0.1f);
                //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, spawnPos, 1.5f/150);
                //float step = speed * Time.deltaTime;
                //transform.position = Vector3.MoveTowards(transform.position, spawnPos, 1.5f / 150);
                //gameObject.transform.position.x = spawnPos.x;
                transform.position = new Vector3( spawnPos.x, spawnPos.y, spawnPos.z);
            }

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
            DeleteAllControllerBox();
            DrawSelectionBox();
            if (hit.tag == "dicom") DrawSizeBox();

        }
        else if (Draw && gameObject.transform.childCount != 0 && gameObject.tag != "Cursor")
        {
            DeleteAllControllerBox();
        }


        if (hit.tag == "room")
        {
            //addRigidbody()
            //Rigidbody gameObjectsRigidBody = hit.AddComponent<Rigidbody>();
            //gameObjectsRigidBody.mass = 0.5f;
            //gameObjectsRigidBody.drag = 1;
        }

       


    }

    public void DrawSelectionBox()
    {
        selectedObjectBounds = hit.GetComponent<BoxCollider>().bounds; // getting bounds

        ControllerObject = GameObject.CreatePrimitive(PrimitiveType.Cube); // create cube
        ControllerObject.transform.position = selectedObjectBounds.center; // move to position
        ControllerObject.transform.parent = hit.transform; // defind patent
                                                           //selectedObjectBounds = hit.GetComponent<MeshFilter>().mesh.bounds; // getting bounds

        // Assigns a material named "Assets/Resources/transparent" to the object.
        //BoxCollider pos = hit.GetComponent<BoxCollider>();
        Material transparentMaterial = (Material)Resources.Load("transparent", typeof(Material)) as Material;
        ControllerObject.GetComponent<Renderer>().sharedMaterial = transparentMaterial;
        Destroy(ControllerObject.GetComponent<Collider>());
        float length = selectedObjectBounds.size.x;
        float width = selectedObjectBounds.size.y;
        float height = selectedObjectBounds.size.z;
        ControllerObject.transform.localScale = new Vector3(length, width, height); // set scale of box
                                                                                    //ControllerObject.transform.localScale = new Vector3(length * 0.4f, width * 0.4f, height * 0.4f); // set scale of box
        ControllerObject.tag = "ControllerObject";
        ControllerObject.name = "transparent_cube";


        Vector3 sarPoint0 = selectedObjectBounds.min * 1.01f;
        Vector3 sarPoint1 = selectedObjectBounds.max * 1.01f;
        Vector3 sarPoint2 = new Vector3(sarPoint0.x, sarPoint0.y, sarPoint1.z);
        Vector3 sarPoint3 = new Vector3(sarPoint0.x, sarPoint1.y, sarPoint0.z);
        Vector3 sarPoint4 = new Vector3(sarPoint1.x, sarPoint0.y, sarPoint0.z);
        Vector3 sarPoint5 = new Vector3(sarPoint0.x, sarPoint1.y, sarPoint1.z);
        Vector3 sarPoint6 = new Vector3(sarPoint1.x, sarPoint0.y, sarPoint1.z);
        Vector3 sarPoint7 = new Vector3(sarPoint1.x, sarPoint1.y, sarPoint0.z);

        CreateEndPointsSphere(sarPoint0, transparentMaterial);
        //CreateEndPointsCube(sarPoint7, transparentMaterial);
        //CreateEndPointsCube(sarPoint2 + ControllerObject.transform.localPosition, transparentMaterial);
        //CreateEndPointsCube(sarPoint3 + ControllerObject.transform.localPosition, transparentMaterial);
        //CreateEndPointsSphere(sarPoint4 + ControllerObject.transform.localPosition, transparentMaterial);
        //CreateEndPointsCube(sarPoint5 + ControllerObject.transform.localPosition, transparentMaterial);
        //CreateEndPointsCube(sarPoint4 + ControllerObject.transform.localPosition, transparentMaterial);
        //CreateEndPointDCube(sarPoint7, transparentMaterial);

        //audioSource.PlayOneShot(dingSound, 0.1f);

        Draw = true;
        log("add controller");

    }

    public void DeleteControllerBox()
    {

        Transform[] children = hit.GetComponentsInChildren<Transform>(true);
        foreach (Transform item in children)
        {
            if (item.gameObject.tag == "ControllerObject")
            {
                Destroy(item.gameObject);
            }
        }
        Draw = false;
        log("delete controller");
    }


    public void DrawSizeBox()
    {
        selectedObjectBounds = hit.GetComponent<BoxCollider>().bounds; // getting bounds



        Vector3 sarPoint0 = selectedObjectBounds.min * 1.01f;
        Vector3 sarPoint1 = selectedObjectBounds.max * 1.01f;
        Vector3 sarPoint2 = new Vector3(sarPoint0.x, 0, sarPoint1.z / 2);
        Vector3 sarPoint3 = new Vector3(sarPoint0.x / 2, sarPoint1.y, sarPoint0.z / 2);
        Vector3 sarPoint4 = new Vector3(sarPoint1.x, sarPoint0.y, sarPoint0.z);
        Vector3 sarPoint5 = new Vector3(sarPoint0.x, sarPoint1.y, sarPoint1.z);
        Vector3 sarPoint6 = new Vector3(sarPoint1.x, sarPoint0.y, sarPoint1.z);
        Vector3 sarPoint7 = new Vector3(sarPoint1.x, sarPoint1.y, sarPoint0.z);


        //CreateCenterPointCube(sarPoint2, "R-Sagittal");
        //CreateCenterPointCube(sarPoint3, "L-Sagittal");
        CreateCenterPointCube(sarPoint4, "A-Axial");
        CreateCenterPointCube(sarPoint5, "P-Axial");
        //CreateCenterPointCube(sarPoint6, "S-Coronal");
        //CreateCenterPointCube(sarPoint7, "I-Coronal");
        //CreateCenterPointShpere(sarPoint3);

        Draw = true;
        log("add controller");
    }



    public void DeleteAllControllerBox()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("ControllerObject");
        foreach (GameObject item in list)
        {
            Destroy(item);
        }
    }


    void CreateEndPointsSphere(Vector3 position, Material transparentMaterial)
    {
        GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sph.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        sph.transform.localPosition = position;
        sph.transform.parent = this.transform;
        sph.tag = "ControllerObject";
        //sph.GetComponent<Renderer>().material.color = new Color32(98, 99, 255, 128); //transparentMaterial;
        Material RotateMaterial = (Material)Resources.Load("rotate360anticlockwise2red", typeof(Material)) as Material;
        sph.GetComponent<Renderer>().sharedMaterial = RotateMaterial;
        //sph.AddComponent<Hold2RotateParent>();
        sph.AddComponent<Hold2Rotate>();
    }

    void CreateCenterPointCube(Vector3 position, String name)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        cube.transform.position = position;
        cube.transform.parent = this.transform;
        cube.tag = "ControllerObject";
        cube.name = name;
        Material MoveMaterial = (Material)Resources.Load("move2red", typeof(Material)) as Material;
        cube.AddComponent<Hold2ChangeSAC>();
        cube.GetComponent<Renderer>().sharedMaterial = MoveMaterial;
    }


    void CreateEndPointsCube(Vector3 position, Material transparentMaterial)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cube.transform.position = position;
        cube.transform.parent = this.transform;
        //cube.transform.localRotation = Quaternion.Euler(45.0f, 45.0f, 45.0f);
        cube.tag = "ControllerObject";
        //sph.GetComponent<Renderer>().material.color = new Color32(98, 99, 255, 128); //transparentMaterial;
        Material MoveMaterial = (Material)Resources.Load("move2red", typeof(Material)) as Material;
        cube.GetComponent<Renderer>().sharedMaterial = MoveMaterial;
        //cube.AddComponent<Hold2Move>();
        //cube.AddComponent<InputTest>();
    }

    void CreateEndPointsCylinder(Vector3 position, Material transparentMaterial)
    {
        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cyl.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cyl.transform.position = position;
        cyl.transform.parent = this.transform;
        cyl.transform.localRotation = Quaternion.Euler(45.0f, 45.0f, 45.0f);
        cyl.tag = "tag_scale";
        cyl.GetComponent<Renderer>().material = transparentMaterial;

    }

    void CreateEndPointDCube(Vector3 position, Material transparentMaterial)
    {
        // add bigger cube
        GameObject cubeB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeB.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        cubeB.transform.position = position;
        cubeB.transform.parent = this.transform;
        cubeB.tag = "ControllerObject";
        Material ResizeMaterial = (Material)Resources.Load("resize-icon-27798", typeof(Material)) as Material;
        cubeB.GetComponent<Renderer>().sharedMaterial = ResizeMaterial;
        // add smaller cube
        GameObject cubeS = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeS.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        cubeS.transform.position = new Vector3(cubeB.transform.position.x + cubeS.transform.localScale.x,
            cubeB.transform.position.y + cubeS.transform.localScale.y,
            cubeB.transform.position.z + cubeS.transform.localScale.z
            );
        //cubeS.transform.position = position*0.5f;
        cubeS.transform.parent = this.transform;
        cubeS.tag = "ControllerObject";
        cubeS.GetComponent<Renderer>().sharedMaterial = ResizeMaterial;
        // add controller script
        //cubeB.AddComponent<Hold2ScaleParent>();
        //cubeS.AddComponent<Hold2ScaleParent>();
        cubeB.AddComponent<Hold2Scale>();
        //cubeS.AddComponent<Hold2Scale>();
        //cubeB.AddComponent<HandResize>();
        //cubeS.AddComponent<HandResize>();
    }


    public void OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
    {
        Debug.Log(eventData.RecognizedText.ToLower());
    }


    public void log(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text += str.ToString() + " \n ";

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

    public Vector3 spawnPos;
    public Quaternion spawnRot;
         

    // compare pos and rot 
    public bool comparePos()
    {
        var dist = Vector3.Distance(spawnPos, gameObject.transform.position);

        if (dist <= 0.15f)
            return true;

        return false;
    }

    public bool compareRot()
    {
        var angle = Quaternion.Angle(spawnRot, gameObject.transform.rotation);
        return false;
    }


    public void setPos()
    {
        spawnPos = gameObject.transform.position;
    }

    public void setRot()
    {
        spawnRot = gameObject.transform.rotation;
    }

    // save pos and rot 
    private void Awake()
    {
        spawnPos = gameObject.transform.position;
        spawnRot = gameObject.transform.rotation;
    }

    // return game object pos and rot 
    public Vector3 getPos()
    {
        return spawnPos;
    }

    public Quaternion getRot()
    {
        return spawnRot;

    }





    // reset game object pos and rot 
    public void ResetToSpawn()
    {
        gameObject.transform.position = spawnPos;
        gameObject.transform.rotation = spawnRot;
    }

}

