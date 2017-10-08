using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ChangeScene2_3 : MonoBehaviour, IFocusable, IInputClickHandler
{
    public int distance = 0;
    private bool kneeExpended = false;
    private bool organExpended = false;
    public GameObject prefab;


    //public bool GotTransform { get; private set; }

    private bool gazing = false;


    // Use this for initialization
    void Start()
    {

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

        Application.LoadLevel("scene_5");
        //UnityEngine.SceneManagement.SceneManager.LoadScene("scene_3");

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
    




}
