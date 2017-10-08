using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;

public class Variables : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private static List<Vector3> kneePosOrig;


    public static List<Vector3> KneePosOrig
    {
        get { return kneePosOrig; }
        set
        {
            GameObject[] list = GameObject.FindGameObjectsWithTag("knee");
            foreach (GameObject go in list)
            {
                Vector3 pos = go.transform.localPosition;
                kneePosOrig.Add(pos);
            }
        }
    }

    private static bool modelStatus;
    public static bool ModelStatus
    {
        get { return modelStatus; }
        set { modelStatus = false; }
    }


}
