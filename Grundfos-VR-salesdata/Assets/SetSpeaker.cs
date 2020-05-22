using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;

public class SetSpeaker : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        // gameObject.GetComponent<Recorder>().Init(VoiceConnection);

        // gameObject.GetComponent<Recorder>().RestartRecording();

        foreach (var item in Microphone.devices)
        {
            Debug.Log(item);
        }
        // gameObject.GetComponent<Recorder>().UnityMicrophoneDevice = Microphone.devices;
    }
    bool alreadyRestarted = false;
    // Update is called once per frame
    void Update()
    {
        if (!alreadyRestarted)
        {
            // Debug.Log(UnityEngine.XR.InputTrackingState);
            //  FindObjectOfType<SpawnPlotController>().  
        }
        //  gameObject.GetComponent<Recorder>().UnityMicrophoneDevice = Microphone.devices[0];

        // gameObject.GetComponent<Recorder>().TransmitEnabled = true;
        // gameObject.GetComponent<Recorder>().IsRecording = true;


    }
}
