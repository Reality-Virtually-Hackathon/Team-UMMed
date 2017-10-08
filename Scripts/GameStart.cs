using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;

public class GameStart : MonoBehaviour, IFocusable, IInputClickHandler
{
    private static int distance = 0;
    private bool gazing = false;
    System.Random rnd = new System.Random();
    private bool started = false;

    public AudioSource audioSource;
    public AudioClip dingSound;

    public Vector3 spawnPos;
    public Quaternion spawnRot;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();

        spawnPos = gameObject.transform.position;
        spawnRot = gameObject.transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnInputClicked(InputEventData eventData)
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("knee");
        if (!started)
        {
            
            foreach (GameObject go in list)
            {
                

                Vector3 pos = go.transform.position;
                //pos.x += 100 * distance;
                //pos.y += 20 * distance;

                pos.x = rnd.Next(0, 200)-100;
                pos.y = rnd.Next(10, 50);
                pos.z = rnd.Next(0, 10);

                    go.transform.position = Vector3.Lerp(go.transform.position, pos, Time.deltaTime / 4f);
                //go.transform.position = Vector3.MoveTowards(go.transform.position, pos, 1.5f / 150);


                // need update the code, all parts will only rotate 45 or 90 for easy control
                //int xx = rnd.Next(1, 180);
                //int yy = rnd.Next(1, 180);
                //int zz = rnd.Next(1, 180);
                //go.transform.Rotate(xx, yy, zz);

                distance++;

            }

            audioSource.PlayOneShot(dingSound, 1.0f);
            started = true;
        } else
        {
            //foreach (GameObject go in list)
            //{
            //   go.transform.position = Vector3.Lerp(go.transform.position, spawnPos, Time.deltaTime / 4f);
               
            //}
            //audioSource.PlayOneShot(dingSound, 1.0f);
            //started = false;
        }

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
