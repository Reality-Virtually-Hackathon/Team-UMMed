using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;


public class SelectObject : MonoBehaviour, IFocusable, IInputClickHandler
{
    private static int distance = 0;
    private static bool kneeExpended = false;
    private bool organExpended = false;
    //public GameObject prefab;
    //public Vector3[] KneeOrig;
    //public Vector3[] OrganOrig;
    //private List<Vector3> KneeOrig = new List<Vector3>();
    public static List<Vector3> KneeOrig { get; set; }
    public List<Vector3> OrganOrig { get; set; }

    //public bool GotTransform { get; private set; }

    private bool gazing = false;


    // Use this for initialization
    void Start()
    {
        KneeOrig = new List<Vector3>();
        GameObject[] list = GameObject.FindGameObjectsWithTag("knee");
        foreach (GameObject go in list)
        {
            Vector3 pos = go.transform.localPosition;
            KneeOrig.Add(pos);
        }

    }

    public void OnInputClicked(InputEventData eventData)
    {
        /*
        GameObject projectile = Instantiate(prefab) as GameObject;
        projectile.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.85f;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = Camera.main.transform.forward * 10;
        */


        // Note that we have a transform.
        GameObject hit = GazeManager.Instance.HitInfo.transform.gameObject;
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text = hit.tag;

        if(hit.tag == "KneeExpandController") { KneeExpend(); log("clicked on knee expend controller"); }
        else if ( hit.tag == "OrganExpandController") { OrganExpend(); log("clicked on organ expend controller"); }


        //KneeExpandController
    }


    public void OnFocusEnter()
    {
        gazing = true;
    }

    public void OnFocusExit()
    {
        gazing = false;
    }


    // expand knee then user click on controller
    public static void KneeExpend()
    {
        if (!kneeExpended)
        {
            //KneeOrig = new List<Vector3>();
            distance = 0;
            //foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            //{
            //    if (go.tag == "knee")
            GameObject[] list = GameObject.FindGameObjectsWithTag("knee");
            foreach (GameObject go in list)
            {
                    Vector3 pos = go.transform.localPosition;
                    //KneeOrig.Add(pos);
                //pos.z += 0.2f * distance;
                //pos.x += (distance * 50) - 150;
                //if (distance % 2 == 0) { pos.x += 170; } else { pos.x -= 170; }
                pos.x += 150 * distance;
                pos.y += 10 * distance;
                    //go.transform.position = pos;
                    go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, pos, Time.deltaTime / 4f);
                    distance++;
                //}
            }
            //log("knee expand");
            kneeExpended = true;
        }
        else
        {
            // reset world if user click on expend controller again
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            int counter = 0;
            //foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            //{
            //    if (go.tag == "knee")
            //    {
                    
            //        go.transform.localPosition = KneeOrig[counter++];
                    
            //    }
            //}
            GameObject[] list = GameObject.FindGameObjectsWithTag("knee");
            foreach (GameObject item in list)
            {
                //item.transform = KneeOrig[counter++] as Transform;
                //Vector3 tr = KneeOrig[counter++];
                item.transform.localPosition = KneeOrig[counter++]; //Variables.KneePosOrig[counter++];//KneeOrig[counter++]; // new Vector3(0, 0, 0); // KneeOrig[counter++];
                item.transform.rotation =Quaternion.Euler(0, 0, 0); //tr.rotation; // 
                item.transform.localScale =new Vector3(1f, 1f, 1f); //tr.localScale;// 
            }

            kneeExpended = false;
            DeleteAllControllerBox();
            //log("reset world");
        }
    }
    // expand knee then user click on controller
    public void OrganExpend()
    {
        
        if (!organExpended)
        {
            OrganOrig = new List<Vector3>();
            distance = 0;
            GameObject[] list = GameObject.FindGameObjectsWithTag("organs");
            foreach (GameObject go in list)
            //foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                //if (go.tag == "organs")
                //{
                    Vector3 pos = go.transform.localPosition;
                    OrganOrig.Add(pos);
                    //pos.z += 0.2f * distance;
                    pos.x += distance * 50 - 400;
                    pos.z = 60;
                    if (distance % 2 == 0) { pos.y += ( 2 * 30); }
                    //go.transform.position = pos;
                    go.transform.localPosition = Vector3.Lerp(go.transform.position, pos, Time.deltaTime / 4f);
                    distance++;
                    //log( pos.x.ToString() + " " + distance.ToString() +"\n");
                //}
            }
            log("organ expand");
            organExpended = true;
        }
        else
        {
            // reset world if user click on expend controller again
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            int counter = 0;
            //foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            //{
            //    if (go.tag == "organs")
            //    {
            //        go.transform.localPosition = OrganOrig[counter++];
            //    }
            //}

            GameObject[] list = GameObject.FindGameObjectsWithTag("organs");
            foreach (GameObject item in list)
            {
                item.transform.localPosition = new Vector3(0,0,0);//OrganOrig[counter++];
                item.transform.rotation = Quaternion.Euler(0, 0, 0);
                item.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            organExpended = false;
            DeleteAllControllerBox();
            log("reset organ");
        }
    }

    public static void DeleteAllControllerBox()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("ControllerObject");
        foreach (GameObject item in list)
        {
            Destroy(item);
        }
    }

    public void log(string str)
    {
        //GameObject.Find("StatusTxt").GetComponent<TextMesh>().text += str.ToString() + " \n ";
        GameObject.Find("StatusTxt").GetComponent<Text>().text += str.ToString() + " \n ";
    }


}
