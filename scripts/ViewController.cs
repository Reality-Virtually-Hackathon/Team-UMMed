using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class ViewController : MonoBehaviour {

    static ViewController _instance = null;

    float minW, maxW, minH, maxH, minD, maxD, threshold;
    GestureRecognizer recognizer;
    GameObject[] axis = new GameObject[4];
    public static ViewController instance
    {
        get
        {
            if (_instance == null)
                _instance = Object.FindObjectOfType<ViewController>();
            return _instance;
        }
    }
    [SerializeField]
    Material mat;
    [SerializeField]
    Shader shader;
	// Use this for initialization
	void Awake () {
        mat = gameObject.GetComponent<Renderer>().material;
        shader = gameObject.GetComponent<Renderer>().material.shader;
        //axis[0] = GameObject.Find("z_controller");
        //axis[1] = GameObject.Find("y_controller");
        //axis[2] = GameObject.Find("x_controller");
        //axis[3] = GameObject.Find("t_controller");
        //recognizer = new GestureRecognizer();
        //recognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
        //recognizer.ManipulationStartedEvent += Recognizer_ManipulationStartedEvent;
        //recognizer.ManipulationUpdatedEvent += Recognizer_ManipulationUpdatedEvent;
        //recognizer.ManipulationCompletedEvent += Recognizer_ManipulationCompletedEvent;
        //recognizer.ManipulationCanceledEvent += Recognizer_ManipulationCanceledEvent;
        //recognizer.StartCapturingGestures();
        //Debug.Log("wtf");
    }
    //private void OnDestroy()
    //{
    //    recognizer.StopCapturingGestures();
    //    recognizer.ManipulationStartedEvent -= Recognizer_ManipulationStartedEvent;
    //    recognizer.ManipulationUpdatedEvent -= Recognizer_ManipulationUpdatedEvent;
    //    recognizer.ManipulationCompletedEvent -= Recognizer_ManipulationCompletedEvent;
    //    recognizer.ManipulationCanceledEvent -= Recognizer_ManipulationCanceledEvent;
    //}

    //private void Recognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    //{
    //    Debug.Log("wtf");
    //}

    //private void Recognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    //{
    //    Debug.Log("wtf");
    //}

    //private void Recognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    //{
    //    foreach (GameObject go in axis)
    //        go.BroadcastMessage("UpdateDragging", cumulativeDelta);
    //}

    //private void Recognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    //{
    //    Debug.Log("wtf");
    //}

    // Update is called once per frame
    void Update () {
        //SetXMin(Input.GetAxis("Horizontal"));
		
	}




    public float GetXMin()
    {
        if (mat == null){mat = gameObject.GetComponent<Renderer>().material;}
        return mat.GetFloat("_SliceAxis1Min");
    }

    public float GetXMax()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_SliceAxis1Max");
    }

    public float GetYMin()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_SliceAxis2Min");
    }

    public float GetYMax()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_SliceAxis2Max");
    }
    public float GetZMin()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_SliceAxis3Min");
    }

    public float GetZMax()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_SliceAxis3Max");
    }

    public float GetDMin()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_DataMin");
    }

    public float GetDMax()
    {
        if (mat == null) { mat = gameObject.GetComponent<Renderer>().material; }
        return mat.GetFloat("_DataMax");
    }
    public void SetXMin(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_SliceAxis1Min", input);
        //print(instance.gameObject.name);
    }

    public void SetXMax(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_SliceAxis1Max", input);
    }

    public void SetYMin(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_SliceAxis2Min", input);
    }

    public void SetYMax(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_SliceAxis2Max", input);
    }

    public void SetZMin(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_SliceAxis3Min", input);
    }

    public void SetZMax(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_SliceAxis3Max", input);
    }

    public void SetThresholdMin(float input)
    {
        if (input > 1.0f || input < 0.0f)
        {
            return;
        }

        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }
        mat.SetFloat("_DataMin", input);
    }

    public void SetThresholdMax(float input)
    {
        if (input > 1.0f || input < 0.0f )
        {
            return;
        }

        if ( mat == null )
        {
            mat = gameObject.GetComponent<Renderer>().material;
        }

        mat.SetFloat("_DataMax", input);
    }

    public void SetNormalization(float input)
    {
        if(mat != null)
            mat.SetFloat("_Normalization", input);
    }

    public void ResetShader()
    {
        if (mat == null)
        {
            mat = gameObject.GetComponent<Renderer>().material;
            
            //return;
        }
        mat.SetFloat("_SliceAxis1Max", 0);
        mat.SetFloat("_SliceAxis1Max", 0.793f);
        mat.SetFloat("_SliceAxis2Max", 0);
        mat.SetFloat("_SliceAxis2Max", 1);
        mat.SetFloat("_SliceAxis3Max", 0);
        mat.SetFloat("_SliceAxis3Max", 1);
        mat.SetFloat("_DataMin", 0.157f);
        mat.SetFloat("_DataMax", 1);
        mat.SetFloat("_Normalization", 2);
    }
}
