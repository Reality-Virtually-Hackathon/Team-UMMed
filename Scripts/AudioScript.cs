using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    public AudioClip MusicClip;
    public AudioSource MusicSource;
   




	// Use this for initialization
	void Start () {
        MusicSource.clip = MusicClip;
	}
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetkeyDown(KeyCode.Space))
         //   MusicSource.PlayOneShot(MusicClip,1.0f);
	}
}
